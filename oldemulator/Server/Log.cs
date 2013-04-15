using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AIRLab.Thornado;
using Eurosim.Core;

namespace EurosimNetworkServer
{
	[Thornado]
	public class Log
	{
		[Thornado]
		public int SessionNumber;

		[Thornado]
		public DateTime StartTime;

		[Thornado]
		public IPAddress ClientIP;

		[Thornado]
		public string RawRequest;

		[Thornado]
		public NetworkServerHelloRequest Request;

		[Thornado]
		public List<LogEvent> Events = new List<LogEvent>();

		[Thornado]
		public List<string> Errors = new List<string>();
	}

	[Thornado]
	public class LogEvent
	{
		[Thornado]
		public string Recieved;

		//[Thornado]
		//public ACMCommand Command;
		[Thornado]
		public ACMSensorInfo SensorInfo;

		[Thornado]
		public double EmulatorTime;
	}

	internal class Logger : IDisposable
	{
		public Logger(EurosimNetworkServer server, string logFilePath)
		{
			this.server = server;
			_logFilePath = logFilePath;
			Log = new Log();
			if(Directory.Exists(_logFilePath))
				Log.SessionNumber = Directory.GetFiles(_logFilePath).Length;
			else
			{
				Directory.CreateDirectory(_logFilePath);
				Log.SessionNumber = 0;
			}
		}

		public void Dispose()
		{
			Flush();
		}

		public void LogConnection(string helloRq, IPAddress clientIp)
		{
			Log.RawRequest = helloRq;
			Log.ClientIP = clientIp;
			Log.StartTime = DateTime.Now;
		}
		public void LogHelloRequest(NetworkServerHelloRequest request)
		{
			Log.Request = request;
		}

		public void LogCommandCycle(string com, ACMSensorInfo info)
		{
			Log.Events.Add(new LogEvent
			               	{
			               		EmulatorTime = server.Emulator.LocalTime,
			               		Recieved = com,
			               		SensorInfo = info,
			               	});
		}

		public void LogError(string err)
		{
			Log.Errors.Add(err);
		}

		private void Flush()
		{
			if(string.IsNullOrWhiteSpace(Log.RawRequest)) //пустые сессии не пишем
				return;
			var logname =Path.Combine(_logFilePath, string.Format("log-{0}.txt", Log.SessionNumber));
			IO.INI.WriteToFile(logname, Log);
		}

		private readonly EurosimNetworkServer server;
		private readonly string _logFilePath;
		public Log Log { get; private set; }
	}
}