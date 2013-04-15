using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AIRLab.Thornado;
using Eurosim.Core;
using System.Net;

namespace EurosimNetworkServer
{
    /// <summary>
    /// 
    /// </summary>
    public enum LogLevel { Error, Connection, Commands, Data }
    class Logger
    {
        LogLevel Level;
        List<LogEvent> EventLog = new List<LogEvent>();
        public Logger(LogLevel Loglevel)
        {
            Level = Loglevel;
        }

        public void Log(Exception e)
        {
            EventLog.Add(new ErrorEvent() { Exception = e });
        }
        public void Log(string recieved, ACMSensorInfo info)
        {
            var ev = new CommandEvent();
            if (Level >= LogLevel.Commands)
                ev.Recieved = recieved;
            if (Level >= LogLevel.Data)
                ev.SensorInfo = info;
            EventLog.Add(ev);
        }
        public void Log(NetworkServerHelloRequest req, IPAddress ip)
        {
            if (Level >= LogLevel.Connection)
                EventLog.Add(new ConnectionEvent() { Request = req, IP = ip });

        }
        public void Write()
        {
            var logname = string.Format("log-{0:yyyy-MM-dd_hh-mm-ss}.txt", DateTime.Now);
            IO.INI.WriteToFile(logname, EventLog);
        }

    }
    [Thornado]
    public class LogEvent
    {
        [Thornado]
        public DateTime Time;
        public LogEvent()
        {
            Time = DateTime.Now;
        }
    }
    public class ConnectionEvent : LogEvent
    {
        [Thornado]
        public NetworkServerHelloRequest Request;
        [Thornado]
        public IPAddress IP;
    }
    public class CommandEvent : LogEvent
    {
        //[Thornado]
        //public ACMCommand Command;
        [Thornado]
        public string Recieved;
        [Thornado]
        public ACMSensorInfo SensorInfo;
    }
    public class ErrorEvent : LogEvent
    {
        [Thornado]
        public Exception Exception;
    }

}
