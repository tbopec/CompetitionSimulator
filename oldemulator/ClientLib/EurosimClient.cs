using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Eurosim.ClientLib
{
	public class EurosimClient : IDisposable
	{
		public EurosimClient(EndPoint serverAdress, TextWriter debugStream, string replayFileName)
		{
			this.debugStream = debugStream;
			this.replayFileName = replayFileName;
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			int attempts = 1;
			do
			{
				try
				{
					socket.Connect(serverAdress);
					break;
				}
				catch
				{
					Thread.Sleep(100);
					LogInfo("Attempt {0}: failed to connect to server {1}", attempts, serverAdress);
				}
			} while(attempts++<attemptLimit);
			if(!socket.Connected)
				throw new Exception("Could not connect to server");
		}

		public EurosimClient(EndPoint serverAdress, string replayFileName)
			: this(serverAdress, Console.Out, replayFileName)
		{
		}

		public void Dispose()
		{
			try
			{
				socket.Dispose();
			}
			catch
			{
			}
		}

		public HelloReply Hello(HelloRequest request, out string serializedSensorInfo)
		{
			Send(RequestFactory.HelloPacket(request));
			LogInfo("Sent hello request");
			string str = Receive();
			string reqPosition = SimpleXmlParser.GetTagContent(str, "Position");
			var reply = new HelloReply
			            	{
			            		Position = (Position)Enum.Parse(typeof(Position), reqPosition),
			            		SessionNumber = int.Parse(SimpleXmlParser.GetTagContent(str, "SessionNumber"))
			            	};
			LogInfo("Received hello reply: {0}", reply);
			serializedSensorInfo = Receive();
			LogInfo("Recieived initial sensor data: {0}", serializedSensorInfo);
			return reply;
		}

		public string Move(MovementCommand movement)
		{
			Send(RequestFactory.CommandPacket(movement));
			LogInfo("Sent command: {0}", movement);
			string str = Receive();
			LogInfo("Received sensor data: {0}", str);
			return str;
		}

		public void End()
		{
			Send(RequestFactory.EndPacket());
			Receive();
		}

		protected string Receive()
		{
			int length = socket.Receive(buffer);
			string str = Encoding.UTF8.GetString(buffer, 0, length);
			string header = SimpleXmlParser.GetRootTagName(str);
			if(header.Equals("Error", StringComparison.OrdinalIgnoreCase))
			{
				LogError(str);
				throw new EurosimClientException(SimpleXmlParser.GetTagContent(str, header));
			}
			if(header.Equals("Feedback", StringComparison.OrdinalIgnoreCase))
				File.WriteAllText(replayFileName, str);
			return str;
		}

		protected void Send(string str)
		{
			socket.Send(Encoding.UTF8.GetBytes(str));
		}

		private void LogInfo(string format, params object[] args)
		{
			debugStream.WriteLine(format, args);
		}

		private void LogError(string format, params object[] args)
		{
			debugStream.WriteLine(string.Format("ERROR: " + format, args));
		}

		private readonly byte[] buffer = new byte[1000000];
		private readonly Socket socket;
		private readonly TextWriter debugStream;
		private readonly string replayFileName;
		private const int attemptLimit = 3;
	}
}