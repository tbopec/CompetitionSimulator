using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace RoboCoP.Internal
{
    /// <summary>
    /// Repsent internal sending methods, that allow to set custom fields in a message header. 
    /// </summary>
    public interface IDataOut : IOut
    {

        IObservable<Unit> SendBase(string data, IDictionary<string, string> additionalHeaders);

        IObservable<Unit> SendBase(byte[] data, IDictionary<string, string> additionalHeaders);

    }
}
