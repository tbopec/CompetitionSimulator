using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoboCoP.Plus;
using System.Threading;

namespace RoboCoP.Common
{
    public abstract class SensorProvider<T> : ServiceProvider<T>
        where T: SensorSettings, new()
    {
        Func<object, byte[]> CustomSerializer;

        int interval;

        public SensorProvider(ServiceApp<T> app)
            : this(app, null, null) { }

        public void InitCustomSerializer(Func<object, byte[]> CustomSerializer)
        {
            this.CustomSerializer = CustomSerializer;
        }


        public SensorProvider(ServiceApp<T> app, string mailbox, string signalName)
            : base(app)
        {

            if (app.Settings.EnableTimer)
            {
                /*
                var timer = new System.Timers.Timer(app.Settings.TimerInterval * 1000);
                timer.Elapsed += (o,e) => ProduceData();
                timer.Start();
                 * */
                interval=(int)(app.Settings.TimerInterval*1000);
                var thread = new Thread(StartTimer);
                thread.IsBackground=true;
                thread.Start();
            }
            if (mailbox!=null && signalName!=null)
                app.Service.Com[mailbox].AddSignalListener(signalName, ProduceData);
        }

        void StartTimer()
        {
            while (true)
            {
                ProduceData();
                Thread.Sleep(interval);
            }
        }


        void ProduceData()
        {
            if (App.Service.Out.Count > 0)
            {
                var data = GetSensorData();
                if (data == null) return;
                if (CustomSerializer == null)
                    App.Service.Out[0].SendObject(data);
                else
                    App.Service.Out[0].SendBinary(CustomSerializer(data));
            }
        }

        //Этот метод должен выдавать, собственно, данные сенсора
        public abstract object GetSensorData();

        

    }
}
