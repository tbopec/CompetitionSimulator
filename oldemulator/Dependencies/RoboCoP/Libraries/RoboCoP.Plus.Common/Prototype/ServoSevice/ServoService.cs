using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;

namespace RoboCoP.Plus.Common {
    class ServoRequest {
        public readonly int Requester;
        public readonly string RequestedAction;

        public ServoRequest(int requster, string data) {
            Requester = requster;
            RequestedAction = data;
        }

        public ServoRequest(int request, ServoData data) {
            Requester = request;
            RequestedAction = IniIO<ServoData>.Object.WriteToString(data);
        }
    }

    public abstract class ServoService {
        protected TerminalServiceApp<ServoServiceSettings> app;
        private Queue<ServoRequest> requests;
        
        protected ServoService(string ServiceName, string[] args) {
            app = new TerminalServiceApp<ServoServiceSettings>(ServiceName, args);

            app.RegisterKey(ConsoleKey.T, "Enter test mode (repeatedly sent commands to all servos)");
            app.RegisterKey(ConsoleKey.O, "Try to reopen port (usable when service was start before controller was)");

            requests = new Queue<ServoRequest>();

            openComPort();
            setInitialSignals();
        }

        public void Run() {
            app.KeyPressed +=new Action<ConsoleKeyInfo>(app_KeyPressed);
            for (int i = 0; i < app.Service.In.Count; ++i) {
                if (app.Service.In[i] != null) {
                    app.Service.In[i].ReceiveObjectAll<ServoData>(ReceiveServoData);
                }
            }

            while (true) {
                if (requests.Count == 0) {
                    System.Threading.Thread.Sleep(1);
                }
                else {
                    ServoRequest request = null;
                    lock (requests) {
                        request = requests.Dequeue();
                    }
                    makeIterration(request);
                }
            }
        }

        /// <summary>
        /// Receive servo data
        /// </summary>
        /// <param name="servoData"></param>
        private void ReceiveServoData(ServoData servoData) {
            lock (requests) {
                requests.Enqueue(new ServoRequest(
                    //TODO: how get number of receiver?
                    0,
                    servoData
                    ));
            }
        }

        /// <summary>
        /// Male one iterration
        ///     Executes another command from the queue.
        ///     Performs all waiting and confirmations
        /// </summary>
        /// <param name="request"></param>
        private void makeIterration( ServoRequest request ) {
            var data    = new ServoData();
            var status  = true;

            IniIO<ServoData>.Object.ParseFromString(data, request.RequestedAction);

            Before();

            app.Log("Execute command");
            app.Debug(request.RequestedAction);

            if (data.DOF.Count == 0 || data.IsInvalid()) {
                status = false;
                app.Log("Data is invalid or empty");
            }
            else {
                Execute(data);
                app.Log("Command sent to controller for execution");

                System.Threading.Thread.Sleep(data.SeekTime);
                app.Log("SeekTime expired (waiting-1 completed)");
            }

            if (request.Requester >= 0 &&
                 request.Requester < app.Settings.Confirmations.Count &&
                 app.Settings.Confirmations[request.Requester].SendImmediateConfirmation
               ) {
                app.Service.Com[app.Settings.Confirmations[request.Requester].Address].SendSignal("CommandComplete");
                app.Log("Immediate confirmation sent");
            }
            else {
                app.Log("Immediate confirmation is not required");
            }

            if (status) {
                System.Threading.Thread.Sleep(data.WaitTime);
                app.Log("WaitTime expired (waiting-2 completed)");
            }

            if (
                request.Requester >= 0 &&
                request.Requester < app.Settings.Confirmations.Count &&
                app.Settings.Confirmations[request.Requester].SendDelayedConfirmation) {
                app.Service.Com[app.Settings.Confirmations[request.Requester].Address].SendSignal("WaitComplete");
                app.Log("Delayed confirmation sent");
            }
            else {
                app.Log("Delayed confirmation is not required");
            }

            app.EndCycle();
            After();
        }

        /// <summary>
        /// Opening com port.
        ///     Lists all ports from 0 to app.Settings.MaxComPort.
        ///     If the response from current port, and could set the current port,
        ///     enumeration stops. Current Port declared required.
        /// </summary>
        private void openComPort() {
            app.Log("Opening port...");

            if (app.Settings.FindComPort) {
                int currentPort = -1;
                for (int i = 0; i < app.Settings.MaxComPort; ++i) {
                    if (EchoController(i)) {
                        currentPort = SetController(i) ? i : -1;
                        break;
                    }
                }
                if (currentPort == -1) {
                    app.Error("NOT FOUND: com port not found");
                }
                else {
                    app.Log("FOUND: " + currentPort);
                }
            }
            else if (app.Settings.ComPort != null) {
                
            }
            else {
                app.Error("Set com port number or select 'FindComPort' in settings");
            }
        }

        /// <summary>
        /// Setting initialization signals.
        ///     Setting the initialization signal from the service configuration.
        /// </summary>
        private void setInitialSignals() {
            if (app.Settings.InitialSignals != null) {
                var initialData = new ServoData() {
                    IsRoot = false,
                    SeekTime = 1000,
                    WaitTime = 1000,
                    DOF = app.Settings.InitialSignals
                };

                Execute(initialData);
            }
            else {
                app.Log("Initial signals are not set");
            }
        }

        /// <summary>
        /// Execution receive data
        /// </summary>
        /// <param name="data">receive data</param>
        public abstract void Execute(ServoData data);

        /// <summary>
        /// Check presence the controller on the port number
        /// </summary>
        /// <param name="number">port number</param>
        /// <returns>True, if the controller exists</returns>
        public abstract bool EchoController(int number);

        /// <summary>
        /// Opening a connection to the controller on the port number
        /// </summary>
        /// <param name="number">port number</param>
        /// <returns>True, if the established connection</returns>
        public abstract bool SetController(int number);

        /// <summary>
        /// Action before the execution command
        /// </summary>
        public virtual void Before() {}

        /// <summary>
        /// Action after the execution command
        /// </summary>
        public virtual void After() {}

        /// <summary>
        /// Permission to manage multiple degrees of freedom at the same time
        /// </summary>
        protected virtual bool EnableMultiDOFSend { get {return true;} }

        /// <summary>
        /// Testing of DOF
        ///     Deviation from the mean position (initialization signal) in both directions
        /// </summary>
        private void testDOF() {
            #region Create position
            var firstPosition   = new ServoData() {
                IsRoot      = true,
                SeekTime    = 1000,
                WaitTime    = 1000,
                DOF         = new List<ServoDOF>()
            };
            var secondPosition  = new ServoData() {
                IsRoot      = true,
                SeekTime    = 1000,
                WaitTime    = 1000,
                DOF         = new List<ServoDOF>()
            };
            #endregion

            #region Initialize position
            for (int i = 0 ; i < this.app.Settings.TestDOFCount ; ++i) {
                var midle   = 0.5;
                var name    =  "";
                
                // correct midle signal by initial signal
                foreach (var dof in this.app.Settings.InitialSignals) {
                    if (dof.ServoNumber == i) {
                        midle   = dof.Signal;
                        name    = dof.ServoName;
                    }
                }
                
                firstPosition.DOF.Add( new ServoDOF() {
                    ServoName = name,
                    ServoNumber = i,
                    Signal = midle - this.app.Settings.TestAmplitude
                });
                secondPosition.DOF.Add(new ServoDOF() {
                    ServoName = name,
                    ServoNumber = i,
                    Signal = midle + this.app.Settings.TestAmplitude
                });
            }
            #endregion

            #region Adding position in queue
            lock (requests) {
                requests.Enqueue(new ServoRequest(-1, firstPosition));
                requests.Enqueue(new ServoRequest(-1, secondPosition));
            }
            #endregion

            #region Return in initialize position
            setInitialSignals();
            #endregion
        }

        /// <summary>
        /// Keyboard processing
        /// </summary>
        /// <param name="keyInfo">Information about pressed key</param>
        private void app_KeyPressed(ConsoleKeyInfo keyInfo) {
            switch(keyInfo.Key) {
                case ConsoleKey.O:
                    openComPort();
                    break;
                case ConsoleKey.T:
                    testDOF();
                    break;
                default:
                    app.Log("Key " + keyInfo.KeyChar.ToString() + " is ignored");
                    break;

            }
        }
    }
}
