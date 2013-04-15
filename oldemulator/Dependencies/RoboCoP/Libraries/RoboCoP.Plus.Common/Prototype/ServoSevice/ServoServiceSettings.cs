using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;
using AIRLab.Thornado.IOs;

namespace RoboCoP.Plus.Common {
    public class ServoServiceSettings : ServiceSettings {
        public ServoServiceSettings() {
            this.Confirmations = new List<ServoConfirmationSettings>();
            this.MaxComPort = 30;
            this.TestAmplitude = 0.1;
        }

        [ThornadoField("True if want to search number of com port", typeof(BoolIO))]
        public bool FindComPort { get; set; }

        [ThornadoField("Number of com port", typeof(IntIO))]
        public int ComPort { get; set; }

        [ThornadoField("Count of DOF for testing", typeof(IntIO))]
        public int TestDOFCount { get; set; }

        [ThornadoList("Settings of how confirmations are sent")]
        public List<ServoConfirmationSettings> Confirmations { get; private set; }

        [ThornadoList("Initial values of servocontrollers")]
        public List<ServoDOF> InitialSignals { get; set; }

        [ThornadoField("Maximum number of com port used for searching", typeof(IntIO))]
        public int MaxComPort { get; set; }

        [ThornadoField("Amplitude of the test motion", typeof(DoubleIO))]
        public double TestAmplitude { get; set; }
    }

    public class ServoConfirmationSettings {
     
        [ThornadoField("Address where confirmation have to be sent to")]
        public string Address { get; set; }
        
        [ThornadoField("True if confirmation is to be sent when command is complete")]
        public bool SendImmediateConfirmation { get; set; }
        
        [ThornadoField("True, if confirmation is to be send when command is complete, and waiting is done")]
        public bool SendDelayedConfirmation { get; set; }
    }
}
