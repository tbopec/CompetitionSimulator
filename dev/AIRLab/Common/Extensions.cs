using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> en, Action<T> act)
        {
            foreach (var e in en)
                act(e);
        }
    }
}
