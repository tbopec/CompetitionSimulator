using AIRLab.Thornado;
using System;


namespace RoboCoP.Plus
{
    ///<summary>
    ///Настройки сервиса робокоп
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    [Serializable]
    public class ServiceSettings: IServiceSettings
    {
        ///<summary>
        ///Создает ServiceSettings
        ///</summary>
        public ServiceSettings()
        {
            Name = "";
            In = new INetworkAddress[0];
            Out = new INetworkAddress[0];
        }

        #region IServiceSettings Members

        ///<summary>
        ///Имя сервиса по умолчанию
        ///</summary>
        [Thornado("Имя сервиса по умолчанию")]
        public string Name { get; set; }

        ///<summary>
        ///Список ip-адресов и портов для портов приема данных
        ///</summary>
        [Thornado("Список ip-адресов и портов для портов приема данных")]
        public INetworkAddress[] In { get; set; }

        ///<summary>
        ///Список ip-адресов и портов для портов отдачи данных
        ///</summary>
        [Thornado("Список ip-адресов и портов для портов отдачи данных")]
        public INetworkAddress[] Out { get; set; }

        ///<summary>
        ///IP-адрес и порт коммуникатора
        ///</summary>
        [Thornado("IP-адрес и порт коммуникатора")]
        public INetworkAddress Switch { get; set; }

        #endregion

        internal void CheckStreamCount(int? inCount, int? outCount)
        {
            var list = new LogicErrorList(LogicErrorType.Internal);
            CheckStreamCount(inCount, outCount, list);
            list.ThrowException(LogicErrorLevel.Error);
        }

        /// <summary>
        /// Проверяет, что число In и Out совпадает с запрашиваемым. null вместо аргумента отключает проверку. Рекомендуется проверять только в случае, когда данный сервис действительно жизненно необходим, и отсутствие подключение сделает работу программы невозможной
        /// </summary>
        /// <param name="inCount">Количество необходимых проиемников этого сервиса.</param>
        /// <param name="outCount">Количество необходимых источников этого сервиса.</param>
        public void CheckStreamCount(int? inCount, int? outCount, LogicErrorList list)
        {
            if(inCount != null)
                if(inCount != (In == null ? 0 : In.Length))
                    list.Add(LogicErrorLevel.Error, "Неверное число входных потоков (In). Ожидается " + inCount);

            if(outCount != null)
                if(outCount != (Out == null ? 0 : Out.Length))
                    list.Add(LogicErrorLevel.Error, "Неверное число выходных потоков (Out). Ожидается " + outCount);
        }
    }
}
