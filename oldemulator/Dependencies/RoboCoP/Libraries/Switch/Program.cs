using System;
using System.IO;
using System.Threading;
using AIRLab;
using AIRLab.Thornado;

namespace Switch
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CommandLineData cmd = CommandLineData.Parse(args, new[] { "config_file" });
            if(!cmd.HasKey("config_file")) {
                Console.WriteLine("First argument must be name of config file.");
                return;
            }

            try
            {
                string config = (new FileInfo(cmd["config_file"])).FullName;


                var settings = IO.INI.ParseFile<SwitchSettings>(config, "Switch");
                
                using (new SwitchApplication(settings, new TextLogger(Console.Out)))
                    new AutoResetEvent(false).WaitOne(); // block
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}