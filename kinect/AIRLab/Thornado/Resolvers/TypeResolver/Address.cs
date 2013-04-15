using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    [PrimaryFormat]
    public class FieldAddressFormat : BasicTypeFormat<FieldAddress>
    {
        public FieldAddressFormat()
            : base(z => z.ToString(), z => FieldAddress.FromString(z), "Адрес")
        { }
    }

    public static class StringExtension
    {
        public static int FirstIndexOfAfter(this string val, int position, char a)
        {
            for (int i = position + 1; i < val.Length; i++)
                if (val[i] == a) return i;
            return -1;
        }

        public static int LastIndexOfBefore(this string val, int position, char a)
        {
            for (int i = position - 1; i >= 0; i--)
                if (val[i] == a) return i;
            return -1;
        }
    }

    public class FieldAddress
    {
        public string Address { get { return address; } }

        public string GetDottedString() { return address; } 

        string address;
        private FieldAddress()
        {
            address = "";
        }

        public static string GetDottedString(IEnumerable<string> subaddresses)
        {
            string res = "";
            foreach (var s in subaddresses)
            {
                if (res != "") res += ".";
                res += s;
            }
            return res;
        }


        public FieldAddress Remaining(FieldAddress child)
        {
            if (Address == child.Address) return new FieldAddress();
            if (Address.Length > child.Address.Length) throw new Exception("");
            if (child.Address.Substring(0, Address.Length) != Address) throw new Exception("");
            return FieldAddress.FromString(child.Address.Substring(Address.Length , child.Address.Length - Address.Length ));
        }


        public static readonly FieldAddress Root = new FieldAddress();
        

        public FieldAddress(params string[] subaddresses)
        {
            address = GetDottedString(subaddresses);
        }

        public FieldAddress Child(params string[] childAddress)
        {
            var a = new FieldAddress();
            if (address == "")
                a.address = GetDottedString(childAddress);
            else
                if (childAddress.Length == 0)
                    a.address = address;
                else
                    a.address = address + "." + GetDottedString(childAddress);
            return a;
        }

        public FieldAddress Child(FieldAddress subAddress)
        {
            var f = new FieldAddress();
            if (address == "") f.address = subAddress.Address;
            else
            {
                if (subAddress.IsRoot) f.address = address;
                else f.address = address + "." + subAddress.Address;
            }
            return f;
        }

        public FieldAddress ShiftRight(string subAddress)
        {
            if (address == "") return new FieldAddress(subAddress);
            return FieldAddress.FromString(subAddress + "." + address);
        }

        public FieldAddress Parent()
        {
            if (address == "")
                throw new Exception("Trying to access parent of root address");
            var a = new FieldAddress();
            int ind = address.LastIndexOf('.');
            if (ind != -1)
                a.address = address.Substring(0, ind);
            return a;
        }

        public FieldAddress CropFirstAddress()
        {
            if (address == "")
                throw new Exception("Trying to access parent of root address");
            var a = new FieldAddress();
            int ind = address.IndexOf('.');
            if (ind != -1)
                a.address = address.Substring(ind+1,address.Length-ind-1);
            return a;
        }

        public IEnumerable<string> Elements
        {
            get
            {
                if (address == "") yield break;
                int first = -1;
                while (true)
                {
                    int ind = address.FirstIndexOfAfter(first,'.');
                    if (ind == -1)
                    {
                        var res=address.Substring(first+1,address.Length-first-1);
                        yield return res;
                        yield break;
                    }
                    else
                    {
                        var res=address.Substring(first + 1, ind - first- 1);
                        first = ind;
                        yield return res;
                    }
                }
            }
        }

        public bool IsRoot
        {
            get
            {
                return address == "";
            }
        }

        public IEnumerable<string> ParentStrings
        {
            get
            {
                return Parent().Elements;
            }
        }

        public string LastElement
        {
            get
            {
                if (address == "") throw new Exception("Trying to get last element from root address");
                int ind = address.LastIndexOf('.');
                if (ind == -1) return address;
                return address.Substring(ind + 1, address.Length - ind - 1);
            }
        }

        public string FirstElement
        {
            get
            {
                if (address == "") throw new Exception("Trying to get first element from root address");
                int ind = address.IndexOf('.');
                if (ind == -1) return address;
                return address.Substring(0, ind);
            }
        }

        public static FieldAddress NewRoot()
        {
            return new FieldAddress();
        }

        public List<string> GetDifference(FieldAddress from = null)
        {
            from = from ?? FieldAddress.NewRoot();
            var addr = new List<string>();
            if (address.Count() <= from.Elements.Count())
                return new List<string>();
            for (int i = from.Elements.Count(); i < this.Elements.Count(); ++i)
                addr.Add(Elements.ToList()[i]);
            return addr;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is FieldAddress)) return false;
            var fa = obj as FieldAddress;
            return address.Equals(fa.address);
        }

        public override int GetHashCode()
        {
            return address.GetHashCode();
        }

        public override string ToString()
        {
            return address;
        }

        public bool StartsWith(FieldAddress key)
        {
            return this.ToString().StartsWith(key.ToString());
        }

        public static FieldAddress FromString(string str)
        {
            if (str == "") return new FieldAddress();
            return new FieldAddress(str.Split('.'));
        }
    }
}
