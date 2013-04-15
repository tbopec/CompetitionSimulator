using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class ThornadoAttribute : Attribute, IADProducingAttribute
    {
        ThornadoInfo info;

        public ThornadoAttribute(string capt = "")
        {
            info = new ThornadoInfo(capt, null);
        }

        public ThornadoAttribute(string capt, string desc)
        {
            info = new ThornadoInfo(capt, desc);
        }

        public IEnumerable<object> GenerateAssotiatedData(Type type)
        {
            yield return info;
        }
    }

    public class ThornadoInfo 
    {
        public ThornadoInfo(string capt = "", string desc = null)
        {
            desc = desc ?? capt;
            Description = desc;
            Caption = capt;
        }

        public ThornadoInfo()
        {
            Description = "";
            Caption = "";
        }

        [Thornado]
        public string Description { get; protected set; }
        [Thornado]
        public string Caption { get; protected set; }
    }


}
