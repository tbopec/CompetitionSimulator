using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCoP.Plus
{
    public class KnownSignal
    {
        public string SignalName { get; private set; }
        public string DefaultMailslot { get; private set; }

        public KnownSignal(string signalName, string mailSlot)
        {
            SignalName = signalName;
            DefaultMailslot = mailSlot;
        }

        public void Send(IServiceApp app)
        {
            Send(app, "");
        }

        public void Send(IServiceApp app, string body)
        {
            app.Service.Com[DefaultMailslot].SendSignal(SignalName, body);
        }

        public void SendToMailslot(IServiceApp app, string mailslot)
        {
            app.Service.Com[mailslot].SendSignal(SignalName, "");
        }

        //TODO: сюда нужно добавить подписку на сигнал и получение одного сигнала. Ю.О.

        public static readonly KnownSignal MovementComplete = new KnownSignal("MovementComplete", "MotionPlanner");
        
    }
}
