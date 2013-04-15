using System;
using System.Collections.Generic;
using System.Text;
using RoboCoP.Plus;
using System.Threading;
using AIRLab.Mathematics;
using Eurosim.Core;
using AIRLab.Thornado;

namespace EmulatorBasicTest
{
    class EmulatorBasicTest
    {
        static ServiceApp<EmulatorBasicTestSettings> app;
        static DateTime begin;
        static void Input()
        {
            while (true)
            {
                var data = app.Service.In[0].ReceiveObject<NavigatorData>();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0:0.00}\t{1:0.00}\t{2:0.00}\t{3:0.00}", (data.Time-begin).TotalSeconds, data.Location.X, data.Location.Y, data.Location.Angle.Grad);
            }
        }


        static void Send(TrivialPlaneMovement mov)
        {
            app.Service.Out[0].SendObject(mov);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(IO.String.WriteToString(mov));
            System.Threading.Thread.Sleep((int)(1000 * mov.TotalTime));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Now it should stop");
            System.Threading.Thread.Sleep((int)(1000 * mov.TotalTime));
         }

        static void PrintSignal()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("MovementComplete signal received\n");
        }
        public static void Main(string[] args)
        {
            app = new TerminalServiceApp<EmulatorBasicTestSettings>("EmulatorBasicTest", args);
            app.Service.Com["EmulatorBasicTest"].AddSignalListener("MovementComplete",PrintSignal);
            begin = DateTime.Now;
            
            int ptr = 0;
            new Thread(new ThreadStart(Input)).Start();
            while (true)
            {
                if (app.Settings.CustomMovements != null && app.Settings.CustomMovements.Count != 0)
                {
                    var fr = app.Settings.CustomMovements[ptr % app.Settings.CustomMovements.Count];
                    ptr++;
                    Send(fr);
                }
                else
                    Send(new TrivialPlaneMovement
                    {
                        Offset = new Frame2D(20, 20, Angle.FromGrad(90)),
                        TotalTime = app.Settings.MovementTime
                    });
           }
        }
    }
}
