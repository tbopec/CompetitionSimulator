using System;
using System.Collections.Generic;
using System.Linq;
using RoboCoP.Exceptions;
using RoboCoP.Messages;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Class for building <see cref="Signal"/>, <see cref="Error"/> and <see cref="DataMessage"/>
    /// from bytes representation.
    /// </summary>
    public class MessageDeserializer
    {
        private readonly IDictionary<string, string> header = new Dictionary<string, string>();
        private IEnumerable<byte> body;

        public MessageDeserializer()
        {
            RestBodyLength = -1;
        }

        /// <summary>
        /// True if all <see cref="Message.Fields"/> have been already read.
        /// </summary>
        public bool HeaderFinished { get; private set; }

        /// <summary>
        /// True if all <see cref="Message.Body"/> has been already read.
        /// </summary>
        public bool BodyFininshed
        {
            get { return RestBodyLength == 0; }
        }

        /// <summary>
        /// True if all <see cref="Message"/> has been already read.
        /// </summary>
        public bool MessageFinished
        {
            get { return HeaderFinished && BodyFininshed; }
        }

        /// <summary>
        /// Length of the part of the <see cref="Message.Body"/> which should be read.
        /// -1 if this value is undefined.
        /// </summary>
        public int RestBodyLength { get; private set; }

        /// <summary>
        /// Append one line of text to the <see cref="Message.Fields"/>.
        /// </summary>
        public void AddHeaderLine(string line)
        {
            if(line == null)
                throw new ArgumentNullException("line");
            if(HeaderFinished)
                throw new InvalidOperationException("Header has been already finished");
            line = line.TrimEnd('\n');
            if(line == string.Empty)
                FinalizeHeader();
            else {
                KeyValuePair<string, string> parsedLine = ParseHeaderLine(line);
                if(header.ContainsKey(parsedLine.Key))
                    throw new ProtocolFormatException("Multiple fields with the same name.");
                header.Add(parsedLine.Key, parsedLine.Value);
            }
        }

        private static KeyValuePair<string, string> ParseHeaderLine(string line)
        {
            int colonIndex = line.IndexOf(':');
            if(colonIndex < 0)
                throw new ProtocolFormatException("Colon is not found in message header line");
            string fieldName = line.Substring(0, colonIndex).Trim(' ', '\t');
            string fieldValue = line.Substring(colonIndex + 1).TrimStart(' ', '\t');
            return new KeyValuePair<string, string>(fieldName, fieldValue);
        }

        private void FinalizeHeader()
        {
            string error;
            if(!MessageValidator.CheckFields(header, out error))
                throw new ProtocolFormatException(error);
            RestBodyLength = int.Parse(header["length"]);
            body = Enumerable.Empty<byte>();
            HeaderFinished = true;
        }

        /// <summary>
        /// Append <paramref name="bytesToAdd"/> to the end of the <see cref="Message.Body"/>
        /// </summary>
        public void AddBytesToBody(IEnumerable<byte> bytesToAdd)
        {
            if(bytesToAdd == null)
                throw new ArgumentNullException("bytesToAdd");
            if(!HeaderFinished)
                throw new InvalidOperationException("Cannot read body until header is finished");
            if(BodyFininshed)
                throw new InvalidOperationException("Body is already finished");
            int bytesToAddLength = bytesToAdd.Count();
            if(bytesToAddLength > RestBodyLength)
                throw new ArgumentException("bytesToAdd length is too big");

            body = body.Concat(bytesToAdd);
            RestBodyLength -= bytesToAddLength;
        }

        /// <summary>
        /// Build the <see cref="Message"/> from the data which has been read (if it was enough data).
        /// Returns <see cref="DataMessage"/> or <see cref="Signal"/> or <see cref="Error"/>
        /// depending on type of the read <see cref="Message"/>.
        /// </summary>
        public Message ToMessage()
        {
            if(!MessageFinished)
                throw new InvalidOperationException("Message has not been finished yet");
            switch((MessageType) Enum.Parse(typeof(MessageType), header["type"], true)) {
            case MessageType.Signal:
                return new Signal(header, body.ToArray());
            case MessageType.Data:
                return new DataMessage(header, body.ToArray());
            case MessageType.Error:
                return new Error(header, body.ToArray());
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}