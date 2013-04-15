using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{
    public class CommanderMock : ICommander
    {

        private IMailSlot personalMail;

        private IList<IMailSlot> slots = new List<IMailSlot>();

        public CommanderMock(string serviceName)
        {
            if (String.IsNullOrEmpty(serviceName))
                throw new ArgumentException("serviceName");

            personalMail = new MailSlotMock(serviceName);
            slots.Add(personalMail);
        }

        public IEnumerator<IMailSlot> GetEnumerator()
        {
            return slots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return slots.GetEnumerator();
        }

        public IMailSlot this[string name]
        {
            get
            {
                return new MailSlotMock(name);
            }
        }

        public IMailSlot PersonalMailSlot
        {
            get { return personalMail; }
        }

        public void RaiseError(Error error)
        {
            return;
        }

        public event Action<Error> OnErrorReceived;
        public event Action<Signal> OnUncatchedSignal;
    }
}
