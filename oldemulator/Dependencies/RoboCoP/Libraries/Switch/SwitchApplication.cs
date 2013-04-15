using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RoboCoP;
using RoboCoP.Helpers;
using RoboCoP.Internal;
using RoboCoP.Messages;
using RoboCoP.Protocols;
using RoboCoP.Protocols.TCP;

using Switch.Core;

namespace Switch
{
    public class SwitchApplication: IDisposable
    {

        MessageFactory Factory = new MessageFactory("Switch");

        public SwitchApplication(ISwitchSettings settings, ILogger logger)
        {
            if(settings == null)
                throw new ArgumentNullException("settings");
            if(logger == null)
                throw new ArgumentNullException("logger");

            Logger = logger;

            INetworkAddress address = new TCPAddress(new IPEndPoint(IPAddress.Any, settings.Port));
            SwitchService = new SwitchService(ProtocolProviderFactory.GetConnectionManager(address));
            SubscribeToEvents(settings.LogLevel);
        }

        public SwitchService SwitchService { get; private set; }
        public ILogger Logger { get; private set; }

        private void SubscribeToEvents(LogLevel logLevel)
        {
            if((logLevel & LogLevel.KnownSignalsToSwitch) != 0)
                SwitchService.KnownSignalReceived +=
                    signal =>
                        Logger.WriteLine("Known signal received by the switch: {0}",
                                         new MessageSerializer(signal).FormatForDebug());

            if((logLevel & LogLevel.UnknowSignalsToSwitch) != 0)
                SwitchService.UnknownSignalReceived +=
                    signal =>
                        Logger.WriteLine("Unknown signal received by the switch: {0}",
                                         new MessageSerializer(signal).FormatForDebug());

            if((logLevel & LogLevel.ErrorsToSwitch) != 0)
                SwitchService.ErrorReceived +=
                    error => Logger.WriteLine("Error received by the switch: {0}", new MessageSerializer(error).FormatForDebug());

            if((logLevel & LogLevel.MessageLost) != 0)
                SwitchService.MessageLost +=
                    message =>
                        Logger.WriteLine(
                            "Message was caught but was not resent because there were no subscribers to the mailslot {0}: {1}",
                            message.To, new MessageSerializer(message).FormatForDebug());

            if((logLevel & LogLevel.MessageSent) != 0)
                SwitchService.MessageSent +=
                    (message, safeConnection) =>
                        Logger.WriteLine("Message was sent to the {0}: {1}",
                                         SwitchService.ServicesName.GetOrDefault(safeConnection, "[unknown name]"),
                                         new MessageSerializer(message).FormatForDebug());

            if((logLevel & LogLevel.MessageCaught) != 0)
                SwitchService.MessageCaught +=
                    (message, safeConnection) =>
                        Logger.WriteLine("Switch caught the message from {0}: {1}",
                                         SwitchService.ServicesName.GetOrDefault(safeConnection, "[unknown name]"),
                                         new MessageSerializer(message).FormatForDebug());

            if((logLevel & LogLevel.ServiceSubscribed) != 0)
                SwitchService.ServiceSubscribed +=
                    (mailbox, safeConnection) =>
                        Logger.WriteLine("Service {0} has subscribed to mailbox {1}",
                                         SwitchService.ServicesName.GetOrDefault(safeConnection, "[unknown name]"), mailbox);

            if((logLevel & LogLevel.ServieDisconnected) != 0)
                SwitchService.BeforeDisconnected +=
                    safeConnection =>
                        Logger.WriteLine("Service {0} was disconnected by the switch",
                                         SwitchService.ServicesName.GetOrDefault(safeConnection, "[unknown name]"));

            if((logLevel & LogLevel.ConnectionAdded) != 0)
                SwitchService.ConnectionAdded +=
                    connection => Logger.WriteLine("Connection from {0} was added", connection.UnsafeConnection.RemoteEndpoint);

            if((logLevel & LogLevel.ConnectionRemoved) != 0)
                SwitchService.ConnectionRemoved +=
                    (logMessage, safeConnection) =>
                        Logger.WriteLine("Connection with {0} was removed because of: {1}",
                                         safeConnection.UnsafeConnection.RemoteEndpoint, logMessage);

            SwitchService.MessageCaught += RedirectToLogger;
            SwitchService.ServiceSubscribed += SubsribeRerdirect;
        }

        public void RedirectToLogger(AddressableMessage arg1, ISafeConnection arg2)
        {
            try
            {
                var loggerConnections = SwitchService
                    .ServicesName
                    .Where(pair => pair.Value == "Logger")
                    .Select(pair => pair.Key).ToList();
                if (loggerConnections.Count > 0)
                        SwitchService.senders[loggerConnections.First()].Send(arg1).Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\n\n{1}\n\n{2}", ex.GetType().ToString(), ex.Message, ex.StackTrace);
            }
        }

         public void SubsribeRerdirect(string mailbox, ISafeConnection connect)
         {
             try
             {
                 var loggerConnections = SwitchService
                     .ServicesName
                     .Where(pair => pair.Value == "Logger")
                     .Select(pair => pair.Key).ToList();
                 if (loggerConnections.Count > 0 && SwitchService.ServicesName.ContainsKey(connect))
                 {
                     string service = SwitchService.ServicesName[connect];
                     SwitchService 
                         .senders[loggerConnections.First()]
                         .Send(Factory.Signal("Logger", "Subscribe", "",
                         new Dictionary<string, string>
                             {
                                 {"subscriber", service},
                                 {"mail", mailbox}
                             }))
                         .Single();
                 }
                 
             }
             catch (Exception ex)
             {
                 Console.WriteLine("{0}\n\n{1}\n\n{2}", ex.GetType().ToString(), ex.Message, ex.StackTrace);
             }             
         }




        #region Implementation of IDisposable

        public void Dispose()
        {
            SwitchService.Dispose();
        }

        #endregion
    }
}