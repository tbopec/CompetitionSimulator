using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using AIRLab.Thornado;
using RoboCoP;

namespace RoboCoP.Plus
{
    /*
    internal static class HelpMaker<T, IO>
        where T: ServiceSettings, new()
        where IO: ClassIOProvider, new()
    {
        #region Создание HelpInfo

        private static void GetParameters(HelpInfo info, string prefix, ClassIOProvider provider)
        {
            foreach(AIRLab.Thornado.FieldInfo f in provider.FieldData.Fields) {
                if(prefix == "")
                    if(f.Name == "In" || f.Name == "Out" || f.Name == "Switch" || f.Name == "Name")
                        continue;
                info.Parameters[f.Name] = f.Caption;
            }

            foreach(AIRLab.Thornado.FieldInfo f in provider.FieldData.Nodes) {
                string p = prefix;
                if(p != "")
                    p += ".";
                p += f.Name;
                GetParameters(info, p, provider.GetClassIOProvider(f));
            }
        }


        private static HelpInfo GetHelpInfoFromAssembly(string serviceName)
        {
            var info = new HelpInfo { Name = serviceName };
            Assembly ass = Assembly.GetEntryAssembly();
            try
            {
                info.Copyright = ((AssemblyCopyrightAttribute)
                    ass.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
            catch { }

            try
            {
                info.Description = ((ServiceDescriptionAttribute)
                    ass.GetCustomAttributes(typeof(ServiceDescriptionAttribute), false)[0]).Description;
            }
            catch { }

            object[] attrs;

            try
            {
                attrs = ass.GetCustomAttributes(typeof(InDataAttribute), true);
                foreach (InDataAttribute att in attrs)
                {
                    while (info.Ins.Count <= att.Info.Number)
                        info.Ins.Add(new DataChannelInfo());
                    info.Ins[att.Info.Number] = att.Info;
                }
            }
            catch { }

            try
            {
                attrs = ass.GetCustomAttributes(typeof(OutDataAttribute), true);
                foreach (OutDataAttribute att in attrs)
                {
                    while (info.Outs.Count <= att.Info.Number)
                        info.Outs.Add(new DataChannelInfo());
                    info.Outs[att.Info.Number] = att.Info;
                }
            }
            catch { }

            try
            {
                attrs = ass.GetCustomAttributes(typeof(InSignalAttribute), true);
                foreach (InSignalAttribute att in attrs)
                    info.InSignals.Add(att.Info);
            }
            catch { }

            try
            {
                attrs = ass.GetCustomAttributes(typeof(OutSignalAttribute), true);
                foreach (OutSignalAttribute att in attrs)
                    info.OutSignals.Add(att.Info);
            }
            catch { }

            return info;           
        }

        private static HelpInfo GetHelpInfoFromClass(Type type, string serviceName)
        {
            var info = new HelpInfo { Name = serviceName };

            try
            {
                info.Description = ((ServiceDescriptionAttribute)
                    type.GetCustomAttributes(typeof(ServiceDescriptionAttribute), false)[0]).Description;
            }
            catch { }

            object[] attrs;

            try
            {
                attrs = type.GetCustomAttributes(typeof(InDataAttribute), true);
                foreach (InDataAttribute att in attrs)
                {
                    while (info.Ins.Count <= att.Info.Number)
                        info.Ins.Add(new DataChannelInfo());
                    info.Ins[att.Info.Number] = att.Info;
                }
            }
            catch { }

            try
            {
                attrs = type.GetCustomAttributes(typeof(OutDataAttribute), true);
                foreach (OutDataAttribute att in attrs)
                {
                    while (info.Outs.Count <= att.Info.Number)
                        info.Outs.Add(new DataChannelInfo());
                    info.Outs[att.Info.Number] = att.Info;
                }
            }
            catch { }

            try
            {
                attrs = type.GetCustomAttributes(typeof(InSignalAttribute), true);
                foreach (InSignalAttribute att in attrs)
                    info.InSignals.Add(att.Info);
            }
            catch { }

            try
            {
                attrs = type.GetCustomAttributes(typeof(OutSignalAttribute), true);
                foreach (OutSignalAttribute att in attrs)
                    info.OutSignals.Add(att.Info);
            }
            catch { }

            return info;                          
        }

        public static HelpInfo GetHelpInfo(string serviceName)
        {
            Assembly ass = Assembly.GetEntryAssembly();
            if (ass.GetCustomAttributes(typeof(ServiceDescriptionAttribute), false).Length != 0)
            {
                var info = GetHelpInfoFromAssembly(serviceName);
                GetParameters(info, "", new IO());
                return info;
            }
            foreach (var type in ass.GetTypes())
            {
                if (type.IsClass && type.GetCustomAttributes(typeof(ServiceDescriptionAttribute), false).Length != 0)
                {
                    var info = GetHelpInfoFromClass(type, serviceName);
                    GetParameters(info, "", new IO());
                    return info;                    
                }    
            }

            return new HelpInfo {Name = serviceName};
        }

        #endregion

        private static string GetFormat(DataChannelInfo info)
        {
            return (!ReferenceEquals(info.FormatType, null) ? info.FormatType.ToString() : "")
                 + ((info.Format != DataChannelFormat.Custom) ? info.Format.ToString() : "")
                 + (!string.IsNullOrEmpty(info.FormatUserDescription) ? " " + info.FormatUserDescription : "");
        }


        private static void PrintHelp(HelpInfo help, Action<string, int> Title, Action<string> Text, Action<TextTable> Table)
        {
            Title(help.Name, 1);
            Text(help.Copyright);
            Title("Описание", 2);
            Text(help.Description);

            if(help.Ins.Count != 0) {
                Title("Входные данные", 2);
                var table = new TextTable(3);
                table.SetCaptions("Канал", "Описание", "Формат");
                table.SetPercentWidthes(0.2, 0.4, 0.4);
                for(int i = 0; i < help.Ins.Count; i++)
                    table.AddRow("In[" + i + "]", help.Ins[i].Description, GetFormat(help.Ins[i]));
                Table(table);
            }
            if(help.Outs.Count != 0) {
                Title("Выходные данные", 2);
                var table = new TextTable(3);
                table.SetCaptions("Канал", "Описание", "Формат");
                table.SetPercentWidthes(0.2, 0.4, 0.4);
                for(int i = 0; i < help.Outs.Count; i++)
                    table.AddRow("Out[" + i + "]", help.Outs[i].Description, GetFormat(help.Outs[i]));
                Table(table);
            }

            if(help.InSignals.Count != 0) {
                Title("Входящие сигналы", 2);
                var table = new TextTable(3);
                table.SetCaptions("Mail", "Название", "Описание");
                table.SetPercentWidthes(0.2, 0.2, 0.6);
                foreach(InSignalInfo e in help.InSignals)
                    table.AddRow(e.Mail, e.Name, e.Description);
                Table(table);
            }

            if(help.OutSignals.Count != 0) {
                Title("Исходящие сигналы", 2);
                var table = new TextTable(3);
                table.SetCaptions("Mail", "Название", "Описание");
                table.SetPercentWidthes(0.2, 0.2, 0.6);
                foreach(OutSignalInfo e in help.OutSignals)
                    table.AddRow(e.Mail, e.Name, e.Description);
                Table(table);
            }

            if(help.Parameters.Count != 0) {
                Title("Параметры", 2);
                var table = new TextTable(2);
                table.SetCaptions("Параметр", "Описание");
                table.SetPercentWidthes(0.2, 0.8);

                foreach(var e in help.Parameters)
                    table.AddRow(e.Key, e.Value);
                Table(table);
            }
        }

        #region Console help

        private static void ConsoleTitle(string text, int level)
        {
            text += "\n";
            if(level == 2)
                text = "\n" + text;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void ConsoleTable(TextTable table)
        {
            table.SetWidthesFromPercents(79);
            Console.Write(table.PrintTableToConsole());
        }

        private static void ConsoleText(string text)
        {
            Console.WriteLine(text);
        }


        public static void PrintConsoleHelp(HelpInfo help)
        {
            PrintHelp(help, ConsoleTitle, ConsoleText, ConsoleTable);
        }

        #endregion

        #region Windows help

        private static string result;

        private static void RtfTable(TextTable table)
        {
            table.SetWidthesFromPercents(70);
            string str = table.PrintTableToRTF();
            result += str;
        }

        private static void RtfTitle(string s, int level)
        {
            result += "\\par \\ul " + s + "\\ul0 \\par";
        }

        private static void RtfText(string s)
        {
            result += "\\par " + s.Replace("\n", "\\par");
        }

        public static void PrintWindowsHelp(HelpInfo help)
        {
            result =
                @"{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset204{\*\fname Courier New;}Courier New CYR;}}\viewkind4\uc1\pard\lang1049\f0\fs20";
            PrintHelp(help, RtfTitle, RtfText, RtfTable);
            result += "}";
            var box = new RichTextBox { Rtf = result, ReadOnly = true };
            var form = new Form { Text = "Помощь" };
            form.Controls.Add(box);
            box.Dock = DockStyle.Fill;
            form.Size = new Size(640, 480);
            Application.Run(form);
        }

        #endregion

        #region Docuwiki help

        private static string wiki;

        public static void WikiTitle(string title, int level)
        {
            title = "==== " + title + " ====";
            if(level == 1)
                title = "=" + title + "=";
            wiki += title + "\n\n";
        }

        public static void WikiTable(TextTable table)
        {
            wiki += "^ ";
            for(int i = 0; i < table.ColumnCount; i++)
                wiki += table.Captions[i] + " ^";
            wiki += "\n";
            foreach(var t in table.Rows) {
                wiki += "| ";
                for(int i = 0; i < table.ColumnCount; i++)
                    wiki += "%%" + t[i] + "%% | ";
                wiki += "\n";
            }
        }

        public static void WikiText(string str)
        {
            wiki += "\n" + str + "\n\n";
        }

        public static string WikiHelp(HelpInfo info)
        {
            wiki = "";
            PrintHelp(info, WikiTitle, WikiText, WikiTable);
            return wiki;
        }

        #endregion
     * 
    }*/
}