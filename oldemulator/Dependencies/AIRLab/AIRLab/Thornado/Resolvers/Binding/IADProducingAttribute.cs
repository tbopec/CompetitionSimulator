using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{

    public interface IADProducingAttribute
    {
        IEnumerable<object> GenerateAssotiatedData(Type type);
    }
}
