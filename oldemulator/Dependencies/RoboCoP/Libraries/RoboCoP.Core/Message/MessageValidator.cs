using System.Collections.Generic;
using System.Linq;
using RoboCoP.Messages;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Class which provides static methods for checking validity of a <see cref="Message"/>.
    /// </summary>
    public static class MessageValidator
    {
        /// <summary>
        /// List of fields that are necessary for different <see cref="MessageType"/>s of the <see cref="Message"/>.
        /// </summary>
        private static readonly Dictionary<string, IList<string>> necessaryFileds =
            new Dictionary<string, IList<string>>
            {
                { MessageType.Data.ToString().ToLower(), new List<string>() },
                { MessageType.Error.ToString().ToLower(), new List<string> { "from", "to", "error-type" } },
                { MessageType.Signal.ToString().ToLower(), new List<string> { "from", "to", "pack-id", "cmd-name" } }
            };

        /// <summary>
        /// Checks if the <see cref="Message.Fields"/> is valid.
        /// If it doesn't then return false and store some error explanation text in <paramref name="error"/>.
        /// </summary>
        public static bool CheckFields(IDictionary<string, string> fields, out string error)
        {
            if(!fields.All(
                pair => !string.IsNullOrEmpty(pair.Key)
                    && pair.Key.All(c => char.IsLetterOrDigit(c)
                        || c == '_'
                            || c == '-'))) {
                error = "Message with incorrect header lines";
                return false;
            }

            string type;
            if(!fields.TryGetValue("type", out type)) {
                error = "Necessary field 'type' absents.";
                return false;
            }
            if(!necessaryFileds.ContainsKey(type)) {
                error = string.Format("Message with unknown type '{0}' found.", type);
                return false;
            }

            string length;
            if(!fields.TryGetValue("length", out length)) {
                error = "Necessary field 'length' absents.";
                return false;
            }
            int intLength;
            if(!int.TryParse(length, out intLength)) {
                error = "Wrong value in 'length' field.";
                return false;
            }

            foreach(string b in necessaryFileds[type])
                if(!fields.ContainsKey(b)) {
                    error = string.Format("In message with type '{0}' necessary field '{1}' absents.", type, b);
                    return false;
                }

            error = null;
            return true;
        }

        /// <summary>
        /// Check if the <see cref="Message"/> is valid.
        /// If it doesn't then return false and store some error explanation text in <paramref name="error"/>.
        /// </summary>
        public static bool CheckMessage(Message message, out string error)
        {
            if(!CheckFields(message.Fields, out error))
                return false;
            if(message.Body.Length != int.Parse(message.Fields["length"])) {
                error = "Length of the message's body differs from declared in header.";
                return false;
            }
            error = null;
            return true;
        }
    }
}