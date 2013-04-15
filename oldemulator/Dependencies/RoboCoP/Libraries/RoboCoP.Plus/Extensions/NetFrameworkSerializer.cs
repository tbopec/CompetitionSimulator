using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using AIRLab.Thornado;

namespace RoboCoP.Plus
{
    public class ThornadoSerialiser : ISerialiser
    {
        IFormatter formatter = new BinaryFormatter();

        public byte[] Serialize<T>(T value)
        {
            if (formatter != null)
            {
                var stream = new MemoryStream();
                formatter.Serialize(stream, value);
                stream.Flush();
                var result = stream.GetBuffer();
                return result;
            }

            var resstr = IO.INI.WriteToString(value);
            var res1=System.Text.Encoding.UTF8.GetBytes(resstr);
            return res1;
            
        }

        public T Deserialize<T>(byte[] data)
        {
            if (formatter != null)
            {
                var stream = new MemoryStream(data);
                var obj = formatter.Deserialize(stream);
                return (T)obj;
            }

            var str = System.Text.Encoding.UTF8.GetString(data);
            return IO.INI.ParseString<T>(str);
        }
    }
}
