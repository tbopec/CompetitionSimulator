using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class ImmutableInfo
    {
        public Dictionary<string, object> Data { get; private set; }
        public bool Raised { get; set; }
        public ImmutableInfo()
        {
            Data = new Dictionary<string, object>();
            Raised = false;
        }
        public bool UsedAndNotRaised { get { return Data.Count != 0 && !Raised; } }
    }
}
