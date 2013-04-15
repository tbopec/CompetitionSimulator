using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace AIRLab.Thornado
{

    public class ParsedInfo
    {
        public FieldAddress Address { get; private set; }
        public string Value { get; set; }
        public int CodeLine { get; private set; }
        public string TypeInfo { get; private set; }
        public ParsedInfo(FieldAddress addr, string value, int num = -1, string typeInfo = null) //вот сюда номер строки
        {
            Address = addr;
            Value = value;
            CodeLine = num;
            TypeInfo = typeInfo;
        }
    }


    public abstract class IOProvider
    {
        #region Сахар для парсинга

        public T ParseString<T>(string Text, FieldAddress Root) { return (T)Parse(Text, Root, typeof(T)); }
        public T ParseString<T>(string Text, string Root) { return (T)Parse(Text, new FieldAddress(Root), typeof(T)); }
        public T ParseString<T>(string Text) { return ParseString<T>(Text, FieldAddress.Root); }

        public T ParseFile<T>(string fileName, FieldAddress Root) { return ParseString<T>(TextFromFilename(fileName), Root); }
        public T ParseFile<T>(string fileName, string Root) { return ParseString<T>(TextFromFilename(fileName), new FieldAddress(Root)); }
        public T ParseFile<T>(string fileName) { return ParseFile<T>(fileName, FieldAddress.Root); }

        
        public object ParseFile(string fname, FieldAddress Root, Type type) { return Parse(TextFromFilename(fname), Root, type); }
        public object ParseFile(string fname, Type type) { return Parse(TextFromFilename(fname), FieldAddress.Root, type); }
        public object ParseString(string str, FieldAddress Root, Type type) { return Parse(str, Root, type); }
        public object ParseString(string str, Type type) { return Parse(str, FieldAddress.Root, type); } 


        protected char[] Separators = Environment.NewLine.ToArray();


        string TextFromFilename(string fname)
        {
            var sr = new StreamReader(fname);
            var text=sr.ReadToEnd();
            sr.Close();
            return text;
        }

        

        #endregion
        #region Собственно, парсинг

        protected abstract void ParseText(string Text, Action<ParsedInfo> callback);
        protected virtual void ContextParse(TextMold mold, Type type, LogicErrorList errors) { }
    

        private object Parse(string Text, FieldAddress Head, Type type)
        {
            this.Head = Head;
            mold = new TextMold();
            errors = new LogicErrorList(LogicErrorType.Reading);
            currentIndex = new Dictionary<FieldAddress, CollectionCurrentIndex>();
            ParseText(Text, ParseRecord);
            var result = TypeResolver.ParseMold(mold, type, errors, ContextParse);
            TypeResolver.CheckConsistancy(result, errors);
            errors.ThrowException(LogicErrorLevel.Error);
            return result;
        }

        protected TextMold GetMold(FieldAddress addr)
        {
            var res = mold;
            var ad = new FieldAddress();
            foreach (var e in addr.Elements)
            {
                if (!res.Nodes.ContainsKey(e))
                    res.Nodes[e] = new TextMold();
                res = res.Nodes[e];
                res.Address = ad = ad.Child(e);
            }
            return res;
        }
        static Regex intRegex = new Regex("^[0-9]+$");

        FieldAddress RemoveStars1(FieldAddress addr)
        {
            var newAddr = new FieldAddress();
            foreach (var e in addr.Elements)
            {
                if (e[0] == '*')
                {
                    CollectionCurrentIndex info = null;
                    if (!currentIndex.ContainsKey(newAddr))
                        currentIndex[newAddr] = info = new CollectionCurrentIndex { i = -1 };
                    else
                        info = currentIndex[newAddr];

                    if (info.LastStar == null || info.LastStar != e)
                        info.i++;
                    newAddr = newAddr.Child(info.i.ToString());
                    info.LastStar = e;
                }
                else
                {
                    if (intRegex.Match(e).Success)
                    {
                        try
                        {

                            var ind = int.Parse(e);
                            CollectionCurrentIndex info = null;
                            if (!currentIndex.ContainsKey(newAddr))
                                currentIndex[newAddr] = info = new CollectionCurrentIndex();
                            else
                                info = currentIndex[newAddr];
                            info.i = ind;
                        }
                        catch { }
                    }
                    newAddr = newAddr.Child(e);
                }
            }
            return newAddr;

        }

        void ParseRecord(ParsedInfo pi)
        {
            errors.Context = "Чтение файла, линия " + pi.CodeLine;

            var addr1 = Head.Elements.ToList();
            var addr2 = pi.Address.Elements.ToList();

            for (int i = 0; i < addr1.Count; ++i)
                if (addr1[i] != addr2[i])
                    return;
            addr2.RemoveRange(0, addr1.Count);
            var addr = new FieldAddress(addr2.ToArray());

            addr = RemoveStars1(addr);
            if (pi.TypeInfo != null)
            {
                if (!addr.IsRoot)
                {
                    switch (pi.TypeInfo)
                    {
                        case "Null":
                            GetMold(addr).IsNull = true;
                            break;
                        case "Type":
                            Type type = null;

                            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                if (type != null) break;
                                var types = asm.GetTypes();
                                foreach (var t in types)
                                    if (t.Name == pi.Value || t.FullName == pi.Value)
                                    {
                                        type = t;
                                        break;
                                    }
                            }

                            if (type == null)
                            {
                                errors.Add(LogicErrorLevel.Error, "Type " + pi.Value + " is not found in assemblies", pi.Address);
                                return;
                            }
                            GetMold(addr).CustomType = type;
                            break;
                    }
                }
            }
            else
            {
                GetMold(addr).Value = pi.Value;
            }
        }

        FieldAddress Head;
        TextMold mold;
        LogicErrorList errors;
        Dictionary<FieldAddress, CollectionCurrentIndex> currentIndex;


        #endregion
        #region Сахар для записи

        public void WriteToFile(string fname, object obj)
        {
            StreamWriter wr = new StreamWriter(fname, false, System.Text.Encoding.UTF8);
            Write(obj, s => wr.Write(s));
            wr.Close();
        }

        public string WriteToString(object obj)
        {
            var b = new StringBuilder();
            Write(obj, s => b.Append(s));
            return b.ToString();
        }

        #endregion
        #region Собственно, запись
        protected void Write(object obj, Action<string> Flusher)
        {
            LogicErrorList list=new LogicErrorList( LogicErrorType.Writing);
            TypeResolver.CheckConsistancy(obj, list);
            var resolver = TypeResolver.GetResolver(obj.GetType());
            var mold = resolver.WriteToMold(obj);
            WriteRecursive(FieldAddress.Root, mold, Flusher);
        }

        void WriteRecursive(FieldAddress address, TextMold mold, Action<string> Flusher)
        {
            WriteSectionStact(Flusher, address,mold.ActualType,mold.CustomType);
            foreach (var e in mold.Nodes)
            {
                if (e.Value.IsNull) WriteNullField(Flusher, address, e.Key);
                if (e.Value.Value != null && e.Value.Nodes.Count == 0) WriteField(Flusher, address, e.Key, e.Value.Value);
            }
            foreach (var e in mold.Nodes)
                if (e.Value.Nodes.Count != 0) WriteRecursive(address.Child(e.Key), e.Value, Flusher);
            WriteSectionEnd(Flusher, address, mold.ActualType, mold.CustomType);
        }

        protected abstract void WriteSectionStact(Action<string> Flusher, FieldAddress address, Type actualType, Type customType);
        protected abstract void WriteNullField(Action<string> Flusher, FieldAddress address, string sub);
        protected abstract void WriteField(Action<string> Flusher, FieldAddress address, string sub, string value);
        protected abstract void WriteSectionEnd(Action<string> Flusher, FieldAddress address, Type actualType, Type customType);

        #endregion
    }
}
