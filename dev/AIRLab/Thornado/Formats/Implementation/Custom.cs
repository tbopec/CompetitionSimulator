using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado.TypeFormats
{
    public class DateFormat : BasicTypeFormat<DateTime>
    {
        static DateTime Parse(string s)
        {
            if (s == "") return Empty;
            int[] f = new int[3];
            int ptr = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= '0' && s[i] <= '9')
                {
                    if (ptr > 2) throw new Exception("Wrong Data string Format");
                    f[ptr] = f[ptr] * 10 + ((int)(s[i] - '0'));
                    continue;
                }
                ptr++;
            }
            if (ptr == 1) f[2] = DateTime.Now.Year;
            if (f[2] < 50) f[2] += 2000;
            if (f[2] > 50 && f[2] < 100) f[2] += 1900;

            return new DateTime(f[2], f[1], f[0]);

        }

        static DateTime Empty = new DateTime();

        static string Write(DateTime time)
        {
            if (time == Empty) return "";
            return string.Format("{0}.{1}.{2}",time.Day,time.Month,time.Year);
        }

        public DateFormat() : base(Write, Parse, "Дата") { }
    }
}
