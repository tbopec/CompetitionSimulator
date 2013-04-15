using System;
using System.IO;

namespace Switch
{
    public class TextLogger: ILogger
    {
        private readonly TextWriter writer;

        public TextLogger(TextWriter log)
        {
            if(log == null)
                throw new ArgumentNullException("log");
            writer = log;
        }

        #region ILogger Members

        public void Write(string message)
        {
            writer.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            writer.WriteLine(format, args);
        }

        #endregion
    }
}