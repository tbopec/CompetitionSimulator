using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoboCoP.Plus;

namespace RoboCoP.Common
{
    public class ServiceProvider<T>
        where T : ServiceSettings, new()
    {
        public ServiceApp<T> App { get; private set; }
        public ServiceProvider(ServiceApp<T> app)
        {
            App = app;
        }
    }
}
