using System;
using System.Collections.Generic;

namespace RoboCoP
{
#pragma warning disable 0105 

#pragma warning restore 0105

    ///<summary>
    ///Класс, содержащий информацию о входящих сигналах сервиса
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public class InSignalInfo
    {
        ///<summary>
        ///Создает InSignalInfo
        ///</summary>
        public InSignalInfo()
        {
            Mail = "";
            Name = "";
            Description = "";
        }



        public string Mail { get; set; }

        ///<summary>
        ///Имя сигнала
        ///</summary>
        public string Name { get; set; }


        ///<summary>
        ///Описание сигнала
        ///</summary>
        public string Description { get; set; }
    }
}

namespace RoboCoP
{
#pragma warning disable 0105 

#pragma warning restore 0105

    ///<summary>
    ///Класс, содержащий информация об исходящих сигналах сервиса
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public class OutSignalInfo
    {
        ///<summary>
        ///Создает OutSignalInfo
        ///</summary>
        public OutSignalInfo()
        {
            Name = "";
            Description = "";
            Mail = "";
        }

        public string Mail { get; set; }

        ///<summary>
        ///Имя сигнала
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Описание сигнала
        ///</summary>
        public string Description { get; set; }

    }
}

namespace RoboCoP
{
#pragma warning disable 0105 

#pragma warning restore 0105

    ///<summary>
    ///Класс, содержащий информацию о канале данных (входном или выходном)
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public class DataChannelInfo
    {
        ///<summary>
        ///Создает DataChannelInfo
        ///</summary>
        public DataChannelInfo()
        {
            Description = "";
            FormatUserDescription = "";
            FormatType = null;
            Format = DataChannelFormat.Custom;
        }

        ///<summary>
        ///Номер канала
        ///</summary>
        public int Number { get; set; }

        ///<summary>
        ///Описание входящих данных
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        ///Дополнительное описание формата данных
        ///</summary>
        public string FormatUserDescription { get; set; }

        ///<summary>
        /// 
        ///Тип, с которым имеет дело формат 
        ///</summary>
        public Type FormatType { get; set; }

        ///<summary>
        ///Формат данных (из соответствующего перечисления)
        ///</summary>
        public DataChannelFormat Format { get; set; }

    }
}

namespace RoboCoP
{
#pragma warning disable 0105 

#pragma warning restore 0105

    ///<summary>
    ///Класс с описанием всей справочной информации о сервисе
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public class HelpInfo
    {
        ///<summary>
        ///Входные данные
        ///</summary>
        private readonly List<DataChannelInfo> __ins;

        ///<summary>
        ///Входящие сигналы
        ///</summary>
        private readonly List<InSignalInfo> __insignals;

        ///<summary>
        ///Выходные данные
        ///</summary>
        private readonly List<DataChannelInfo> __outs;

        ///<summary>
        ///Исходящие сигналы
        ///</summary>
        private readonly List<OutSignalInfo> __outsignals;

        ///<summary>
        ///Параметры
        ///</summary>
        private readonly Dictionary<string, string> __parameters;

        ///<summary>
        ///Создает HelpInfo
        ///</summary>
        public HelpInfo()
        {
            Name = "";
            Copyright = "";
            Description = "";
            __ins = new List<DataChannelInfo>();
            __outs = new List<DataChannelInfo>();
            __insignals = new List<InSignalInfo>();
            __outsignals = new List<OutSignalInfo>();
            __parameters = new Dictionary<string, string>();
        }

        ///<summary>
        ///Имя сервиса
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Копирайт
        ///</summary>
        public string Copyright { get; set; }

        ///<summary>
        ///Описание
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        ///Входные данные
        ///</summary>
        public List<DataChannelInfo> Ins
        {
            get { return __ins; }
        }

        ///<summary>
        ///Выходные данные
        ///</summary>
        public List<DataChannelInfo> Outs
        {
            get { return __outs; }
        }

        ///<summary>
        ///Входящие сигналы
        ///</summary>
        public List<InSignalInfo> InSignals
        {
            get { return __insignals; }
        }

        ///<summary>
        ///Исходящие сигналы
        ///</summary>
        public List<OutSignalInfo> OutSignals
        {
            get { return __outsignals; }
        }

        ///<summary>
        ///Параметры
        ///</summary>
        public Dictionary<string, string> Parameters
        {
            get { return __parameters; }
        }
    }
}