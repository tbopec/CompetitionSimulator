using System;
using System.Collections.Generic;
using System.Threading;
using AIRLab.Thornado;
using RoboCoP;

namespace RoboCoP.Plus
{
    public class TerminalServiceApp<T>: ConsoleServiceApp<T>, ITerminalServiceApp
        where T: ServiceSettings, new()
    {
        #region Инициализация

        private readonly bool enableTerminal;

        public TerminalServiceApp(string firstName, string[] args, bool DisableTerminal = false)
            : base(firstName, args)
        {
            enableTerminal = !DisableTerminal;
            if(enableTerminal)
                new Thread(KeyboardThread).Start();
            AddCycle();
        }

        #endregion

       
        #region ITerminalServiceApp Members

      

        public event Action<ConsoleKeyInfo> KeyPressed;

        #endregion

        #region Рисование

        private readonly Queue<Tuple<TerminalLineType, string>> memory = new Queue<Tuple<TerminalLineType, string>>();

        private bool addDebug = true;

        public override void Log(object log)
        {
            string str = log.ToString();
            base.Log(str);
            Add(TerminalLineType.Log, str);
        }

        public override void Error(object error)
        {
            string str = error.ToString();
            base.Error(str);
            Add(TerminalLineType.Error, str);
        }

        public override void Debug(object debug)
        {
            string str = debug.ToString();
            base.Debug(str);
            Add(TerminalLineType.Debug, str);
        }

        public override void Info(object info)
        {
            string str = info.ToString();
            base.Info(info);
            Add(TerminalLineType.Info, str);
        }

        public override void EndCycle()
        {
            base.EndCycle();
            AddCycle();
        }

        private void Add(TerminalLineType t, string m)
        {
            lock(memory) {
                if(!enableTerminal)
                    return;
                memory.Enqueue(new Tuple<TerminalLineType, string>(t, m));
                PrintLine(t, m);
                if(memory.Count > 500)
                    memory.Dequeue();
            }
        }

        private void AddCycle()
        {
            Add(TerminalLineType.Cycle, "───────────────────────────────────────────────────────────────────────────────");
        }

        private void PrintLine(TerminalLineType t, string m)
        {
            switch(t) {
            case TerminalLineType.Cycle:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case TerminalLineType.Log:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                break;
            case TerminalLineType.Info:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;
            case TerminalLineType.Debug:
                if(!addDebug)
                    return;
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case TerminalLineType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            }
            Console.WriteLine(m);
        }

        private void PrintAll()
        {
            Console.Clear();
            lock(memory) {
                foreach(var e in memory)
                    PrintLine(e.Item1, e.Item2);
            }
        }

        #endregion

        private void KeyboardThread()
        {
            while(true) {
                ConsoleKeyInfo c = Console.ReadKey(true);
                switch(c.Key) {
                case ConsoleKey.F1:
                    Console.ForegroundColor = ConsoleColor.White;
                    var table = new TextTable(2);
                    table.Captions[0] = "Key";
                    table.Captions[1] = "Mean";
                    table.Widthes[0] = 10;
                    table.Widthes[1] = 60;
                    foreach(var k in keys)
                        table.AddRow(k.Item1.ToString(), k.Item2);
                    Console.WriteLine(table.PrintTableToConsole());
                    break;
                case ConsoleKey.F12:
                    addDebug = !addDebug;
                    PrintAll();
                    break;
                default:
                    if(KeyPressed != null)
                        KeyPressed(c);
                    break;
                }
            }
        }
    }
}