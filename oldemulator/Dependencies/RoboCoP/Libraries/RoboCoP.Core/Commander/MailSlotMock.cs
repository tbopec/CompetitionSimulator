using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace RoboCoP.Implementation
{
    public class MailSlotMock : IMailSlot
    {
        public string Name
        {
            get; private set;
        }

        public void SendSignal(string signalName)
        {
            return;
        }

        public void SendSignal(string signalName, string data)
        {
            return;
        }

        public string ReceiveSignal(string signalName)
        {
            (new EventWaitHandle(false, EventResetMode.ManualReset)).WaitOne();
            return "";
        }

        public void AddSignalListener(string signalName, Action onReceive)
        {
            return;
        }

        public void AddSignalListener(string signalName, Action<string> onReceive)
        {
            return;
        }

        public MailSlotMock(string name)
        {
            if (ReferenceEquals(name, null))
                throw new ArgumentNullException("name");
            if (name == "")
                throw new ArgumentException("name");

            Name = name;
        }

    }
}
