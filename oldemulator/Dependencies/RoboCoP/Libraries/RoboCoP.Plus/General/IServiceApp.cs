using RoboCoP;

namespace RoboCoP.Plus
{
    public interface IServiceApp
    {
        void Log(object str);
        void Error(object str);
        void Debug(object str);
        void EndCycle();
        object ObjectSettings { get; }
        Service Service { get; }
        ServiceAppEnvironment Environment { get; }
    }
}