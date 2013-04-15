using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab;
using System.IO;
using AIRLab.Thornado;

namespace RoboCoP.Plus
{
    public class ServiceAppEnvironment
    {
        public string CfgFileEntry { get; private set; }
        public string CfgFileName { get; private set; }
        public string CfgFileFolder { get; private set; }
        public string BaseName { get; private set; }
        public string Suffix { get; private set; }
        public string ServiceName { get; private set; }

        public ServiceAppEnvironment(string firstName, string suffix, ServiceAppEnvironment oldEnv)
        {
            this.BaseName = firstName;
            this.Suffix = "." + suffix;
            this.ServiceName = firstName + suffix;
            this.CfgFileEntry = oldEnv.CfgFileEntry;
            this.CfgFileFolder = oldEnv.CfgFileFolder;
            this.CfgFileName = oldEnv.CfgFileName;
        }

        public ServiceAppEnvironment(string firstName, string[] commandLineArguments)
        {
            var cmd = CommandLineData.Parse(commandLineArguments, new string[] { "Config0", "Config1", "Config2", "Config3" });
            
            //устанавливаем имя нашего сервиса
            BaseName = firstName;
            ServiceName = BaseName;

            if (cmd.HasKey("Suffix"))
            {
                Suffix = "." + cmd["Suffix"];
                ServiceName += Suffix;
            }
            else
                Suffix = "";
            
            //создаем файловый объект основного конфига
            FileInfo cfginfo = null;
            try
            {
                cfginfo = new FileInfo(cmd["Config0"]);
            }
            catch
            {
                throw new Exception("Config file name is incorrect (" + cmd["Config0"] + ")");
            }
            CfgFileFolder = cfginfo.Directory.FullName;
            CfgFileName = cfginfo.FullName;



            //Собираем общий текст всех конфиг файлов
            CfgFileEntry = "";

            for (int i = 0; i <= 3; i++)
            {
                string file = null;
                if (i == 0) file = CfgFileName;
                else
                {
                    //читаем вспомогательный конфиг из той же папки, где лежит основной
                    var cfgKey = "Config" + i.ToString();
                    try
                    {
                        if (cmd[cfgKey] == "") break;
                    }
                    catch
                    {
                        break;
                    }
                    file = LocalPath(cmd[cfgKey]);
                }
                if (!File.Exists(file))
                    throw new Exception("Config file #" + i.ToString() + " is not found (path " + file + ")");
                try
                {
                    StreamReader rd = new StreamReader(file);
                    CfgFileEntry += "\n";
                    CfgFileEntry += "%Config file #" + i.ToString() + " " + file + "\n";
                    CfgFileEntry += rd.ReadToEnd();
                    rd.Close();
                }
                catch (Exception e)
                {
                    throw new Exception("There was an error when reading Config file #" + i.ToString() + " (path " + file + "), error: " + e.Message);
                }
            }
            //подцепляем аргументы командной строки
            cmd.ExcludeKeys("Config0", "Config1", "Config2", "Config3", "Suffix");
            CfgFileEntry += "\n%Command line arguments";
            CfgFileEntry += "\n[" + ServiceName + "]\n" + cmd.BuildIniFile();
        }

        public T ReadSettings<T>()
            where T : ServiceSettings,new()
        {

            var Settings = IO.INI.ParseString<T>(CfgFileEntry, ServiceName);
            Settings.Name = ServiceName;
            return Settings;
        }

        public string LocalPath(string filePath)
        {
            return CfgFileFolder + "\\" + filePath;
        }
        

    }
}
