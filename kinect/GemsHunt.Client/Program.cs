using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using AIRLab.Thornado;
using System.Threading;



namespace GemsHunt.Client
{

    class ClientResponse
    {
        [Thornado]
        public string Move;
    }

    class Client
    {
        [Thornado]
        public string move = "w";

        static void Main(string[] args)
        {

            TcpClient tcpClient;

            NetworkStream networkStream;

            StreamReader streamReader;

            StreamWriter streamWriter;

            try
            {
                var resp = new ClientResponse { Move = "w" };

                Console.WriteLine(resp.Move);

                var XMLCommand = IO.XML.WriteToString(resp);

                Console.WriteLine(XMLCommand);
                
                tcpClient = new TcpClient("localhost", 5555);

                networkStream = tcpClient.GetStream();

                streamReader = new StreamReader(networkStream);

                streamWriter = new StreamWriter(networkStream);

                
                for(int i = 0; i < 5; i++)
                //while (true)
                {
                    Thread.Sleep(1000);

                    //string str = Console.ReadLine();

                    streamWriter.WriteLine("<string>w</string>");                   

                    streamWriter.Flush();

                    string str2 = streamReader.ReadLine();

                    Console.WriteLine(str2);
                }
            }

            catch (SocketException ex)
            {

                Console.WriteLine(ex);

            }



        }
    }
}


