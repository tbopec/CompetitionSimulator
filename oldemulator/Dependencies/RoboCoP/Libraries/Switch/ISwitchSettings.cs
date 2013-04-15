using RoboCoP;

namespace Switch
{
    public interface ISwitchSettings
    {

        int Port { get; }

        LogLevel LogLevel { get; }
    }
}