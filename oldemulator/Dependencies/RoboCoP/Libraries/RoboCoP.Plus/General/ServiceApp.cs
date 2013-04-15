using System;
using System.Diagnostics;
using System.IO;
using AIRLab;
using AIRLab.Thornado;
using System.Collections.Generic;
using System.Linq;

namespace RoboCoP.Plus
{
    /// <summary>
    /// Приложение, основанное на сервисе.
    /// Сейчас оно только позволяет создать сервис при помощи одной строчки. В будущем, оно должно при создании сервиса вешаться на его Com, писать в консоль все что нужно, обрабатывать команды типа Show/Hide и так далее
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="IO"></typeparam>
    public class ServiceApp<T> : IServiceApp
        where T : ServiceSettings, new()
    {
        #region Данные сервисного приложения
       
        public Service Service { get; private set; }
        public T Settings { get; private set; }
        public object ObjectSettings { get { return Settings; } }
        public ServiceAppEnvironment Environment { get; private set; }

        #endregion

        #region Инициализация приложения

        public ServiceApp(string firstName, string[] args)
            : this(firstName, args, delegate { }) { }

        public ServiceApp(string firstName, string[] args, Action<ServiceStates> callback)
        {
            Init(firstName, args, callback);
        }

        protected ServiceApp() { }

        public ServiceApp(ServiceAppEnvironment env, T settings)
        {
            this.Environment = env;
            this.Settings = settings;
            Settings.Name = env.ServiceName;
            this.Service = new Service(settings);
        }

        protected void Init(string firstName, string[] args, Action<ServiceStates> callback)
        {
            var cmd = CommandLineData.Parse(args, new string[] { "Config0", "Config1", "Config2", "Config3" });

            //Проверяем, не нужно ли вывести помощь на экран
            if (cmd.Count == 0 || cmd.HasKey("Help"))
            {
                //var info = HelpMaker<T, UniIOProvider<T>>.GetHelpInfo(firstName);
                //ShowHelp(info);
                Process.GetCurrentProcess().Kill();
                return;
            }

            //проверяем, не нужно ли вывести помощь в вики
            if (cmd.HasKey("WikiHelp"))
            {
                var fname = cmd["WikiHelp"];

                if (string.IsNullOrEmpty(fname))
                {
                    Console.WriteLine("Ключ --wikihelp требует указания файла, в который будет выгружена справка");
                    Console.ReadLine();
                }
                else
                {
                    var wr = new StreamWriter(fname, true, System.Text.Encoding.UTF8);
                    //wr.WriteLine(HelpMaker<T, UniIOProvider<T>>.WikiHelp(HelpMaker<T, UniIOProvider<T>>.GetHelpInfo(firstName)));
                    wr.Close();
                }
                Process.GetCurrentProcess().Kill();
                return;
            }

            if (callback == null)
                throw new ArgumentNullException();

            callback(ServiceStates.ServiceAppInitStart);

            //проверяем наличие конфиг-файла
            if (!cmd.HasKey("Config0"))
                throw new Exception("Cfg file is not specified");

            //начинаем парсить конфиг
            callback(ServiceStates.ConfigParsing);

            Environment = new ServiceAppEnvironment(firstName, args);
            Settings = Environment.ReadSettings<T>();

            Service = new Service(Settings, callback);
        }

        #endregion

        #region Виртуальные методы

        protected virtual void ShowHelp(HelpInfo help) { }

        #endregion

        #region Человеческие сообщения о стадии

        private static readonly string[] messages =
            new[]
            {
                "Начало инициализации приложения",
                "Чтение конфигурационного файла",
                "Начало инициализации сервиса",
                "Инициализация выходных потоков",
                "Инициализация входных потоков",
                "Регистрация сервиса на свитче",
                "ОК"
            };

        protected string GetMessage(ServiceStates state)
        {
            return messages[(int)state];
        }

        #endregion

        #region Методы для отправки сообщений вовне (пока заглушки)

        public virtual void Log(object log) { }

        public virtual void Error(object error) { }

        public virtual void Debug(object debug) { }

        public virtual void Info(object info) { }

        public virtual void EndCycle() { }
        #endregion

        #region Клавиатура

        protected readonly List<Tuple<ConsoleKey, string>> keys = new List<Tuple<ConsoleKey, string>>();
        public event Action<ConsoleKeyInfo> KeyPressed;
        protected void RaiseKeyboardEvent(ConsoleKeyInfo key)
        {
            if (KeyPressed!=null && keys.Select(z => z.Item1).Where(z => z == key.Key).Count() != 0)
                KeyPressed(key);
        }
        public void RegisterKey(ConsoleKey key, string message)
        {
            keys.Add(new Tuple<ConsoleKey, string>(key, message));
        }

        #endregion

        public void MergeSettings()
        {
            //new IniIO<T>().MergeAndOverwrite(Settings, Environment.ServiceName, Environment.CfgFileName, false);
            Error("Merging is not working");
            Info("New settings are merged into cfg file");
        }

        public bool ReadLocalFile(string filename, string fileTarget, Action<FileStream> loadFile)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                Error(fileTarget + " is not specified");
                return false;
            }
            var fullName = Environment.CfgFileFolder + "\\" + filename;
            FileInfo info = null;
            try
            {
                info = new FileInfo(fullName);
            }
            catch
            {
                Error(fileTarget + " is invalid (" + fullName + ")");
                return false;
            }
            if (!info.Exists)
            {
                Error(fileTarget + " does not exist (" + fullName + ")");
                return false;
            }
            FileStream stream = null;
            try
            {
                stream = info.Open(FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Error("Can't open " + fileTarget + " (" + fullName + "). The exception was: " + e.Message);
                return false;
            }
            try
            {
                loadFile(stream);
            }
            catch (Exception e)
            {
                Error("Can't read " + fileTarget + " (" + fullName + "). The exception was: " + e.Message);
            }

            try
            {
                stream.Close();
            }
            catch { }

            return true;
        }

        public bool WriteLocalFile(string filename, string fileTarget, Action<FileStream> writeFile)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                Error(fileTarget + " is not specified");
                return false;
            }
            var fullName = Environment.CfgFileFolder + "\\" + filename;
            FileInfo info = null;
            try
            {
                info = new FileInfo(fullName);
            }
            catch
            {
                Error(fileTarget + " is invalid (" + fullName + ")");
                return false;
            }
            FileStream stream = null;
            try
            {
                stream = info.Open(FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch (Exception e)
            {
                Error("Can't open " + fileTarget + " (" + fullName + "). The exception was: " + e.Message);
                return false;
            }
            try
            {
                writeFile(stream);
            }
            catch (Exception e)
            {
                Error("Can't write " + fileTarget + " (" + fullName + "). The exception was: " + e.Message);
            }

            try
            {
                stream.Close();
            }
            catch { }

            return true;
        }

        public string LocalPath(string filePath)
        {
            return Environment.CfgFileFolder + "\\" + filePath;
        }
    }
}