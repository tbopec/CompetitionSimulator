using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using AIRLab.Thornado;

namespace EurosimNetworkServer
{
	internal class NetworkInterface : IDisposable
	{
		public NetworkInterface(EurosimNetworkServer server, int port, Logger logger)
		{
			this.server = server;
			Port = port;
			Logger = logger;
			Listener = new TcpListener(IPAddress.Any, Port);
			Listener.Start();
			EurosimNetworkServer.ConsoleLogInfo("Started TCP listener on port {0}", port);
		}

		public void Dispose()
		{
			try
			{
				connection.Close();
				Listener.Stop();
				EurosimNetworkServer.ConsoleLogInfo("Disposed network interface.");
			}
			catch
			{
			}
		}

		public string Read()
		{
			int length = connection.Receive(receiveBuffer);
			return enc.GetString(receiveBuffer, 0, length);
		}

		public void Write(string str)
		{
			connection.Send(enc.GetBytes(str));
		}

		public bool ValidHelloRequestReceived(out NetworkServerHelloRequest helloRequest)
		{
			helloRequest = null;
			connection = Listener.AcceptSocket();
			string rqString = Read();
			EurosimNetworkServer.ConsoleLogInfo("Received hello from {0}", ClientIP);
			Logger.LogConnection(rqString, ClientIP);
			try
			{
				var rq = IO.XML.ParseString<NetworkServerHelloRequest>(rqString);
				if (!IsValidRequest(rq))
				{
					SendError("Пожалуйста, заполните все поля пакета (название команды, вуз, город,email)");
					return false;
				}
				Logger.LogHelloRequest(rq);
				helloRequest = rq;
				return true;
			}
			catch(Exception e1)
			{
				SendError(e1.Message);
			}
			return false;
		}

		public void HelloReply(NetworkServerHelloReply reply)
		{
			Write(IO.XML.WriteToString(reply));
		}

		public void SendFeedback(string fb)
		{
			Write("<Feedback>" + fb + "</Feedback>");
		}

		public void SendError(string error)
		{
			var sb=new StringBuilder();
			using (var xmlWriter = XmlWriter.Create(sb))
			{
				xmlWriter.WriteStartElement("Error");
				xmlWriter.WriteString(error);
				xmlWriter.WriteEndElement();
			}
			Write(sb.ToString());
			Logger.LogError(error);
			Console.WriteLine("ERROR: " + error);
			server.StopServer();
		}

		public IPAddress ClientIP
		{
			get
			{
				if(connection != null && connection.RemoteEndPoint != null)
					return (connection.RemoteEndPoint as IPEndPoint).Address;
				return null;
			}
		}


		private static bool IsValidRequest(NetworkServerHelloRequest helloRq)
		{
			return !(String.IsNullOrWhiteSpace(helloRq.Affiliation) ||
			         String.IsNullOrWhiteSpace(helloRq.City) ||
			         String.IsNullOrWhiteSpace(helloRq.Name)) &&
			       helloRq.Email.Contains("@");
		}

		private readonly Encoding enc = Encoding.UTF8;
		private readonly Logger Logger;
		private readonly int Port;
		private Socket connection;
		private readonly TcpListener Listener;
		private readonly EurosimNetworkServer server;
		private readonly byte[] receiveBuffer = new byte[1000000];
	}
}