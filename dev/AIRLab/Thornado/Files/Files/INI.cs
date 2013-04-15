using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AIRLab.Thornado
{
    public class INI : IOProvider
    {


        protected override void WriteSectionStact(Action<string> Flusher, FieldAddress address, Type actualType, Type customType)
        {
            Flusher("[" + address + "]"+Environment.NewLine);
            if (customType != null) Flusher("@Type=" + customType.FullName+Environment.NewLine);
        }

        protected override void WriteField(Action<string> Flusher, FieldAddress address, string sub, string value)
        {
            Flusher(sub + "=" + value.Replace("\\","\\\\").Replace("\n","\\n")+Environment.NewLine);
        }

        protected override void WriteNullField(Action<string> Flusher, FieldAddress address, string sub)
        {
            Flusher(sub + "@Null" + Environment.NewLine);
        }

        protected override void WriteSectionEnd(Action<string> Flusher, FieldAddress address, Type actualType, Type customType)
        {
            
        }

        Regex fieldRegex = new Regex(@"^[^:]+:\s*[^\s]+(\s[^:]+:\s*[^\s]+)*\s*$");
     

        protected override void ContextParse(TextMold mold, Type type, LogicErrorList errors)
        {
            var cat = TypeResolver.GetResolver(type).Category;
            if (mold.Value == null) return;
            if (cat == Categories.Field || cat == Categories.Fiction) return;
            var p = mold.Value;
            switch (cat)
            {
                case Categories.Immutable:
                case Categories.Dictionary:
                case Categories.Node:
                    if (!fieldRegex.Match(mold.Value).Success)
                    {
                        errors.Add(LogicErrorLevel.Error, string.Format("Неправильное присвоение {0}. Строка: {1}", cat.ToString(), p));
                        return;
                    }
                    p.Split(' ').ToList().ForEach(
                        a =>
                        {
                            if (a.Length == 0) return;
                            var pairs = a.Split(':').Select(b => b.Trim());
                            if (pairs.Count() < 2)
                                errors.Add(LogicErrorLevel.Error, string.Format("Неверный формат описания.\nОбрабатываемая строка: {1}\nОшибка: {2}", mold.Address.GetDottedString(), p, a));
                            else
                            {
                                var newAddr = new FieldAddress(pairs.First());
                                newAddr = mold.Address.Child(newAddr);
                                GetMold(newAddr).Value = pairs.Last();
                            }
                        }
                    );
                    return;
                case Categories.Tensor:
                case Categories.List:
                    var parts = p.Split('|');
                    for (int i = 0; i < parts.Length; ++i)
                        GetMold(mold.Address.Child(i.ToString())).Value = parts[i];
                    return;
            }
        }
   
        
        #region Пашин парсинг


        static string[] GetAddress(string str)
        {
            if (str.Last() == '*')
                str += index++.ToString();
            return str.Split('.').Select(a => a.Trim()).Where(a => a.Length > 0).ToArray();
        }

         protected override void ParseText(string Text, Action<ParsedInfo> callback)
         {
            var data = Text.Split('\r', '\n');
            foreach (var pi in Parse(data))
            {
                if (!string.IsNullOrEmpty(pi.Value))
                    pi.Value = pi.Value.Replace("\\n", "\n").Replace("\\\\", "\\");
                callback(pi);
            }
        }
        bool comment = false;
        static int index = 0;
        protected IEnumerable<ParsedInfo> Parse(IEnumerable<string> data)
        {
            var currentAddress = new List<FieldAddress> { FieldAddress.NewRoot() };
            // [A.B]
            // [A, B, C]
            var re4Section = new Regex(@"^\[(?<addr>.*)\]\s*$");
            // A.B = asdf
            var re4Address = new Regex(@"^(?<addr>[^\s=]*)\s*=\s*(?<val>.*)\s*$");
            var re4Type = new Regex(@"^(?<addr>[^@]*)@(?<type>[^\s\=]*)\s*=\s*(?<val>.*)$");
            var re4TypeWithoutParametr = new Regex(@"^(?<addr>[^@]*)@(?<type>[^\s\$]*)\s*$");

            var re4SectionComment = new Regex(@"^\s*%\[.*\]\s*");
            var re4Comment = new Regex(@"^\s*%.*");
            int num = 0;
            foreach (var str in data)
            {
                num++;
                var typeMatch = re4Type.Match(str);
                var typeMatchWithoutParametr = re4TypeWithoutParametr.Match(str);

                var sectionMatch = re4Section.Match(str);
                var addressMatch = re4Address.Match(str);
                var commentMatch = re4Comment.Match(str);
                var sectionCommentMatch = re4SectionComment.Match(str);

                if (sectionCommentMatch.Success)
                {
                    comment = true;
                    continue;
                }
                if (commentMatch.Success)
                {
                    continue;
                }

                if (typeMatch.Success && !comment)
                {
                    foreach (var ca in currentAddress)
                        if (typeMatch.Groups["addr"].Value.Length > 0)
                            yield return new ParsedInfo(ca.Child(GetAddress(typeMatch.Groups["addr"].Value)), typeMatch.Groups["val"].Value, num, typeMatch.Groups["type"].Value);
                        else
                            yield return new ParsedInfo(ca, typeMatch.Groups["val"].Value, num, typeMatch.Groups["type"].Value);
                    continue;
                }

                if (typeMatchWithoutParametr.Success && !comment)
                {
                    foreach (var ca in currentAddress)
                        if (typeMatchWithoutParametr.Groups["addr"].Value.Length > 0)
                            yield return new ParsedInfo(ca.Child(GetAddress(typeMatchWithoutParametr.Groups["addr"].Value)), "", num, typeMatchWithoutParametr.Groups["type"].Value);
                        else
                            yield return new ParsedInfo(ca, "", num, typeMatchWithoutParametr.Groups["type"].Value);
                    continue;
                }
                if (sectionMatch.Success)
                {
                    var addr = sectionMatch.Groups["addr"].Value;
                    currentAddress.Clear();
                    if (addr.Length > 0)
                        foreach (var ad in addr.Split(' ', ',').Where(a => a.Length > 0))
                            currentAddress.Add(FieldAddress.NewRoot().Child(GetAddress(ad)));
                    else currentAddress.Add(FieldAddress.NewRoot());
                    for (int i = 0; i < currentAddress.Count; ++i)
                    {
                        var a = currentAddress[i];
                        if (!a.IsRoot && a.LastElement[0] == '*')
                            currentAddress[i] = a.Parent().Child(string.Format("*{0}", index++));
                    }
                    comment = false;
                    continue;
                }
                if (addressMatch.Success && !comment)
                {
                    foreach (var ca in currentAddress)
                        yield return new ParsedInfo(ca.Child(GetAddress(addressMatch.Groups["addr"].Value)), addressMatch.Groups["val"].Value, num);
                    continue;
                }
            }
        }

        #endregion
    }
}
