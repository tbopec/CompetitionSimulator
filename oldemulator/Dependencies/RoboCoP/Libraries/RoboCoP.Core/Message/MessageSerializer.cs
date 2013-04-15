using System;
using System.Linq;
using System.Text;
using RoboCoP.Exceptions;
using RoboCoP.Helpers;
using RoboCoP.Messages;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Class for building byte representation of the <see cref="Message"/>.
    /// </summary>
    public class MessageSerializer
    {
        private readonly Message msg;
        private string header;
        private byte[] package;

        public MessageSerializer(Message msg)
        {
            if(msg == null)
                throw new ArgumentNullException("msg");
            this.msg = msg;
            string error;
            if(!MessageValidator.CheckMessage(msg, out error))
                throw new MessageValidationFailed(error);
        }

        /// <summary>
        /// String representation of the <see cref="Message.Fields"/>.
        /// </summary>
        public string Header
        {
            get
            {
                return header ??
                    (header = String.Join("\n", msg.Fields.Select(k => k.Key + ": " + k.Value)) + "\n");
            }
        }

        /// <summary>
        /// Size of the whole <see cref="Message"/>. And size of the <see cref="Package"/> at the same time.
        /// </summary>
        public int Size
        {
            get { return Encoding.UTF8.GetByteCount(Header + "\n") + msg.Body.Length; }
        }

        /// <summary>
        /// Whole <see cref="Message"/> in binary format.
        /// </summary>
        public byte[] Package
        {
            get
            {
                if(package == null) {
                    string headerWithCr = Header + "\n";
                    var result = new byte[Encoding.UTF8.GetByteCount(headerWithCr) + msg.Body.Length];
                    Encoding.UTF8.GetBytes(headerWithCr, 0, headerWithCr.Length, result, 0);
                    Array.Copy(msg.Body, 0, result, result.Length - msg.Body.Length, msg.Body.Length);
                    package = result;
                }

                return package;
            }
        }

        /// <summary>
        /// Format <see cref="Message"/> for debugging output.
        /// </summary>
        public string FormatForDebug()
        {
            var sb = new StringBuilder();
            new[] { Header, msg.TextBody }
                .SelectMany(str => str.Split(new[] { '\n' }, StringSplitOptions.None))
                .ForEach(line => sb.Append("\t").AppendLine(line));
            return String.Format("Message {{\r\n{0}}}", sb.ToString());
        }
    }
}