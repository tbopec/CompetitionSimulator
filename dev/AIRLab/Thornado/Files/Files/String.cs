using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class StringIO : IOProvider
    {
        public StringIO()
        {
        }

        protected override void ParseText(string a, Action<ParsedInfo> callback)
        {
            foreach (var str in a.Split(' '))
            {
                var parts = str.Split('=');
                if (parts.Length != 2)
                    continue;
                callback(new ParsedInfo(new FieldAddress(parts[0]), parts[1]));
            }
        }

        protected override void WriteSectionStact(Action<string> Flusher, FieldAddress address, Type actualType, Type customType)
        {
        }

        protected override void WriteNullField(Action<string> Flusher, FieldAddress address, string sub)
        {
            Flusher(string.Format("{0}@Null", address.Child(sub).GetDottedString()));
        }

        protected override void WriteField(Action<string> Flusher, FieldAddress address, string sub, string value)
        {
            Flusher(string.Format("{0}={1}", address.Child(sub).GetDottedString(), value));
        }

        protected override void WriteSectionEnd(Action<string> Flusher, FieldAddress address, Type actualType, Type customType)
        {
            
        }
    }
}
