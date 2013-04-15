using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AIRLab.Thornado
{
    public class XtansiveXElement
    {
        public XElement Value { get; set; }
        public FieldAddress Address { get; set; }
        public Tuple<string, string> AdditionalInfo { get; set; }
    }

    static class XMLExt
    {
        public static IEnumerable<XtansiveXElement> GetEndChilds(this XtansiveXElement current)
        {
            var els = current.Value.Elements();
           /* if (current.Value.Name.LocalName == XML.Prefix)
            {
                if (!current.Address.IsRoot)
                {
                    var item = current.Value.Attribute(XName.Get("item")).Value;
                    current.Address = current.Address.Child(item);
                }
            }
            else*/
                current.Address = current.Address.Child(current.Value.Name.LocalName);
            foreach (var info in current.Value.Attributes().Select(a => new Tuple<string, string>(a.Name.LocalName, a.Value)))
                yield return new XtansiveXElement() { Address = current.Address, AdditionalInfo = info };
            if (els.Count() == 0) yield return current;
            var upd = els.Select(a =>
            {

                var v = new XtansiveXElement() { Value = a };
                //if (current.Value.Name.LocalName == XML.Prefix)
                //    v.Address = FieldAddress.NewRoot();
                //else
                v.Address = current.Address;
                return v;
            });
            foreach (var e in upd.SelectMany(a => a.GetEndChilds()))
                yield return e;
        }
    }

    public class XML : IOProvider
    {

        protected override void ParseText(string str, Action<ParsedInfo> callback)
        {
            var xml = new XtansiveXElement() { Value = XElement.Parse(str), Address = FieldAddress.NewRoot() };
            var childs = xml.GetEndChilds();
            foreach (var ch in childs)
            {
                string addr = "";
                foreach (var s in ch.Address.Elements.Skip(1))
                    addr += (addr == "" ? "" : ".") + Retag(s);
                
                if (ch.Value != null)
                    callback(new ParsedInfo(new FieldAddress(addr), ch.Value.Value));
                if (ch.AdditionalInfo != null)
                    callback(new ParsedInfo(new FieldAddress(addr), ch.AdditionalInfo.Item2, -1, ch.AdditionalInfo.Item1));
            }
        }


        string Tag(string t)
        {
            if (char.IsDigit(t[0])) return "item"+t;
            return t;
        }

        string Retag(string t)
        {
            if (t.Length >= 5 && t.Substring(0, 4) == "item" && char.IsDigit(t[4]))
                return t.Substring(4);
            return t;
        }

        protected override void WriteSectionStact(Action<string> Flusher, FieldAddress address, Type actualType, Type customType)
        {
            string name="";
            if (address.IsRoot) name = actualType.Name;
            else name = address.LastElement;
            string Type = "";
            if (customType!=null) Type=" Type=\""+customType.FullName+"\"";
            Flusher("<"+Tag(name)+Type+">");
        }

        protected override void WriteNullField(Action<string> Flusher, FieldAddress address, string sub)
        {
            Flusher("<" + Tag(sub) + " Null=\"\"/>");
        }

        protected override void WriteField(Action<string> Flusher, FieldAddress address, string sub, string value)
        {
            Flusher("<"+Tag(sub)+">"+value.Replace("&","&amp;").Replace("<","&lt;").Replace(">","&gt;")+"</"+Tag(sub)+">");
        }

        protected override void WriteSectionEnd(Action<string> Flusher, FieldAddress address, Type actualType, Type customType)
        {
            string name = "";
            if (address.IsRoot) name = actualType.Name;
            else name = address.LastElement;
            Flusher("</" + Tag(name) + ">");
        }
    }
}
