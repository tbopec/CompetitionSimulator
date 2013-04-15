using System;
using System.Collections.Generic;
using System.Text;

namespace AIRLab
{
    public class CommandLineData: Dictionary<string, string>
    {
        public bool HasKey(string key)
        {
            try {
                if(this[key] != null)
                    return true;
                return false;
            }
            catch(Exception) {
                return false;
            }
        }

        public static CommandLineData Parse(string[] args, string[] names)
        {
            var v = new CommandLineData();
            string currentKey = null;
            int currentName = 0;
            foreach(string t in args) {
                string e = t;
                if(e[0] == '-') {
                    currentKey = e.Substring(1, e.Length - 1);
                    v[currentKey] = "";
                } else if(currentKey == null)
                    if(currentName >= names.Length)
                        throw new Exception("Неименованный параметр командной строки `" + e
                            + "` не поддерживается: допустимо лишь " + names.Length + " параметров");
                    else {
                        v[names[currentName]] = e;
                        currentName++;
                    }
                else {
                    v[currentKey] = t;
                    currentKey = null;
                }
            }
            return v;
        }

        public void ExcludeKeys(params string[] exKeys)
        {
            foreach(string e in exKeys)
                try {
                    Remove(e);
                }
                catch {}
        }

        public string BuildIniFile()
        {
            var b = new StringBuilder();
            foreach(var e in this)
                b.Append(e.Key + "=" + e.Value + "\n");
            return b.ToString();
        }
    }
}