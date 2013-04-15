using System;
using System.Diagnostics;
using AIRLab.Thornado;
using RoboCoP;

namespace RoboCoP.Plus
{
    public class ConsoleServiceApp<T>: ServiceApp<T>
        where T: ServiceSettings, new()
    {
        private readonly string firstName;

        public ConsoleServiceApp(string firstName, string[] args, Action<ServiceStates> callback = null)
        {
            this.firstName = firstName;

            try {
                Init(firstName, args, (Action<ServiceStates>) Delegate.Combine(callback, new Action<ServiceStates>(OnStateChange)));
            }
            catch(Exception e) {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("FAIL");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Process.GetCurrentProcess().Kill();
            }
        }

        private void OnStateChange(ServiceStates state)
        {
            switch(state) {
            case ServiceStates.ServiceAppInitStart:
                Console.Title = firstName;
                break;
            case ServiceStates.ServiceInitStart:
                Console.Title = Settings.Name;
                break;
            }

            Console.ForegroundColor = state == ServiceStates.ServiceReady ? ConsoleColor.Green : ConsoleColor.DarkGreen;
            Console.WriteLine(GetMessage(state));
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        protected override void ShowHelp(HelpInfo help)
        {
            //HelpMaker<T, UniIOProvider<T>>.PrintConsoleHelp(help);
            //Console.ReadLine();
        }
    }
}