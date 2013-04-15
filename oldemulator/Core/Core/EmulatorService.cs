using RoboCoP.Plus;
using RoboCoP.Common;

namespace Eurosim.Core
{
    public class EmulatorService : ServiceProvider<EmulatorSettings>
    {
        public EmulatorService(Emulator emul, ServiceApp<EmulatorSettings> app)
            : base(app)
        {
            app.Service.Com["Eurosim"].AddSignalListener("Reset",
                () =>
                {
                    emul.ResetRequest = true;
                });
        }
    }
}
