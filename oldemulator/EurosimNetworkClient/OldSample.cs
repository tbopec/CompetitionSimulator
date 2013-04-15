using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace EurosimNetworkClient
{
    class ProgramItem
    {
        public double Distance;
        public double Angle;
        public double Time;
        public string ActuatorCommand;
        public string Exec()
        {
            if (ActuatorCommand == null)
                return
                "<Action>" +
                "<NextRequestInterval>" + Time + "</NextRequestInterval>" +
                "<ArcMovement><item0>" +

                "<Distance>" + Distance + "</Distance>" +
                "<Rotation>" + Angle + "</Rotation>" +
                "<TotalTime>" + Time + "</TotalTime>" +
                "</item0></ArcMovement>"+
                "</Action>";
            return
                "<Action>"+
                "<NextRequestInterval>" + Time +"</NextRequestInterval>"+
                "<ActuatorCommands><item0>" + ActuatorCommand +"</item0></ActuatorCommands>"+
                "</Action>";
        }
    }


    class EurosimNetworkClient
    {   
        #region Простейшая программа для робота 
        static readonly List<ProgramItem> program = new List<ProgramItem>();
        private const double linSpeed = 19.99;
        private const double andSpeed = 44;


        static void Mov(double distance)
        {
            program.Add(new ProgramItem { ActuatorCommand = null, Angle = 0, Distance = distance, Time = Math.Abs(distance / linSpeed )});
        }

        static void Rot(double angle)
        {
            program.Add(new ProgramItem { ActuatorCommand = null, Angle = angle, Distance = 0, Time = Math.Abs(angle / andSpeed) });
        }

        static void Arc(double distance, double angle)
        {
            if (distance == 0) Rot(angle);
            program.Add(new ProgramItem { ActuatorCommand = null, Angle = angle, Distance = distance, Time = Math.Abs(distance / linSpeed) });
        }

        static void Act(string cmd)
        {
            program.Add(new ProgramItem { ActuatorCommand = cmd, Angle = 0, Distance = 0, Time = 0.5 });
        }

        static void End()
        {
            program.Add(new ProgramItem { ActuatorCommand = null, Distance = 0, Angle = 0, Time = 10000 });
        }

        #endregion

        static Socket socket;

        public static string Read()
        {
            var buffer = new byte[1000000];
            var length = socket.Receive(buffer);
            var str = Encoding.UTF8.GetString(buffer, 0, length);
            return str;
        }

        public static void Write(string str)
        {
            socket.Send(Encoding.UTF8.GetBytes(str));
        }
    
        static void NotMainAnyMore(string[] args)
        {
            var add = "127.0.0.1";
            if (args.Length>0 && !string.IsNullOrEmpty(args[0]))
                add = args[0];
            Console.Write("Sending hello request... ");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (true)
            {
                try
                {
                    socket.Connect(add, 9876);
                    break;
                }
                catch 
                {
                    Thread.Sleep(100);
                }
            }
            Write(string.Format("<HelloEurosim>" +"<Name>{0}</Name>"+
                   "<Affiliation>{1}</Affiliation>"+"<Email>{2}</Email>"+"<City>{3}</City>"+
                   "<Position>Left</Position>" +"<Opponent>Nocturnal</Opponent>" +"</HelloEurosim>", 
                   "TestTeam", "urFUUU", "someone@microsoft.com", "Ekaterinburg"));
            Console.WriteLine("OK");

            var helloReply = Read();
            Console.WriteLine("Reply:\n" + helloReply);

            Mov(50);
            Rot(-90);
            Mov(56);
            Rot(-90);
            Mov(35);
            Act("Grip");
            Act("Raise");
            Mov(-40);
            Rot(90);
            Mov(10);
            Act("Release");
            End();

            int ptr = 0;
            while (true)
            {
                var str = Read();
                int begin = str.IndexOf("<");
                int end = str.IndexOf(">");
                var header = str.Substring(begin+1,end-begin-1);
               
                if (header == "Feedback")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    using (var fs = new StreamWriter("feedback.txt"))
                    {
                        fs.Write(str);
                    }
                    break;
                }

                if (header == "Error")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(str);
                    break;
                }

                Console.WriteLine(str);
               

                var command=program[ptr].Exec();
                Console.WriteLine(command);
                socket.Send(Encoding.UTF8.GetBytes(command));
                ptr++;
                if (ptr >= program.Count)
                    ptr--;
            }

            Console.ReadLine();
        }
    }
}
