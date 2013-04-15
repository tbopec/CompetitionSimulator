using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using AIRLab.Drawing;
using AIRLab.Thornado;
using System.Threading;
using GemsHunt.Library.ClientServer;

namespace GemsHunt.Client
{

    class Client
    {
        [Thornado]
        public string move = "w";

        static void Main(string[] args)
        {
            try
            {
                var tcpClient = new TcpClient("localhost", 10001);

                var streamReader = new StreamReader(tcpClient.GetStream());
                var streamWriter = new StreamWriter(tcpClient.GetStream());
                Team = streamReader.ReadLine();
                var rand = new Random();
                while(true)
                {
                    try
                    {
                        var resp = new ClientResponse
                            {
                                Team = Team,
                                Command = new Command
                                    {
                                        Move = rand.Next(0, 100),
                                        Angle = rand.Next(-90, 90)
                                    }
                            };
                        var xmlCommand = IO.XML.WriteToString(resp);
                        streamWriter.WriteLine(xmlCommand);
                        streamWriter.Flush();
                        var res = IO.XML.ParseString<ClientRequest>(streamReader.ReadLine());
                        var img = res.Camera.Split(';').Select(byte.Parse).ToArray();
                        var kinect = res.Kinect.Split('|').Select(a => a.Split(';').Select(b =>
                            {
                                double c;
                                if(!double.TryParse(b, out c))
                                    c = double.PositiveInfinity;
                                return c;
                            }).ToList()).ToList();
                        var bmp = Image.FromStream(new MemoryStream(img));
                        if(res.IsExit)
                            break;
                    }
                    catch
                    {
                        return;
                    }
                }
            }

            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected static string Team { get; set; }
    }
}


