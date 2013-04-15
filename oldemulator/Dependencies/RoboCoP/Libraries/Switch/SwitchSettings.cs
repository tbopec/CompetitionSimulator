using AIRLab.Thornado;
using RoboCoP;
using RoboCoP.Plus;

namespace Switch
{
    public class SwitchSettings: ISwitchSettings
    {
        #region ISwitchSettings Members

        [Thornado("Сетевой адрес сервиса свитча.")]
        public int Port { get; set; }

        [Thornado("Уровень логгирования сообщений.")]
        public LogLevel LogLevel { get; set; }

        #endregion
    }
}