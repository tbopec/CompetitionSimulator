using System;

namespace RoboCoP
{

    /// <summary>
    /// Аттрибут, содержащий общее описание сервиса. Отмечает сущность - сборку или класс, 
    /// содержащию в аттрибуах документацию к сервису.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false)]
    public class RoboCoPService : Attribute
    {

        public string Description { get; private set; }

        public string Service { get; private set; }

        public string Author { get; private set; }

        /// <summary>
        /// Общее описание сервиса
        /// </summary>
        /// <param name="description">описание</param>
        /// <param name="service">имя сервиса</param>
        public RoboCoPService(string service, string description, string author)
        {
            Description = description;
            Service = service;
            Author = author;
        }

    }
        
    /// <summary>
    /// Атрибут, применяемый для документирования сервиса Robocop. Содержит в себе описание одного входящего сигнала
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
    public class InSignalAttribute: Attribute
    {
        /// <summary>
        /// Информация о входящем сигнале
        /// </summary>
        public readonly InSignalInfo Info;

        /// <summary>
        /// Описание принимаемого сервисом сигнала
        /// </summary>
        /// <param name="mail">имя mailbox, c которого принимается сигнал</param>
        /// <param name="signalName">название сигнала</param>
        /// <param name="description">описание сигнала</param>
        public InSignalAttribute(string mail, string signalName, string description)
        {
            Info = new InSignalInfo { Mail = mail, Name = signalName, Description = description };
        }

        /// <summary>
        /// Описание принимаемого сервисом сигнала
        /// </summary>
        /// <param name="mail">имя mailbox, c которого принимается сигнал</param>
        /// <param name="description">описание сигнала</param>
        public InSignalAttribute(string mail, string description)
        {
            Info = new InSignalInfo { Mail = mail, Name = " ", Description = description };
        }

    }

    /// <summary>
    /// Атрибут, применяемый для документирования сервиса Robocop. Содержит в себе описание одного исходящего сигнала
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
    public class OutSignalAttribute: Attribute
    {
        public readonly OutSignalInfo Info;

        /// <summary>
        /// Описание отправляемого сервисом сигнала
        /// </summary>
        /// <param name="mail">имя mailbox, на который отправляется сигнал</param>
        /// <param name="signalName">название сигнала</param>
        /// <param name="description">описание сигнала</param>
        public OutSignalAttribute(string mail, string signalName, string description)
        {
            Info = new OutSignalInfo { Mail = mail, Name = signalName, Description = description };
        }

        /// <summary>
        /// Описание отправляемого сервисом сигнала
        /// </summary>
        /// <param name="mail">имя mailbox, на который отправляется сигнал</param>
        /// <param name="description">описание сигнала</param>
        public OutSignalAttribute(string mail, string description)
        {
            Info = new OutSignalInfo { Mail = mail, Name = " ", Description = description };
        }

    }

    /// <summary>
    /// Атрибут, описывающий канал данных. Сам по себе не применяется, см. <see cref="InDataAttribute"/> и <see cref="OutDataAttribute"/>
    /// </summary>
    public class DataChannelAttribute: Attribute
    {
        /// <summary>
        /// Информация о канале данных
        /// </summary>
        public readonly DataChannelInfo Info;

        /// <summary>
        /// Создает атрибут с соответствующими значениями полей <see cref="DataChannelInfo"/>
        /// </summary>
        protected DataChannelAttribute(int index, string description, Type formatType, string formatUserDescription)
        {
            Info = new DataChannelInfo
                   {
                       Number = index,
                       Description = description,
                       FormatType = formatType,
                       FormatUserDescription = formatUserDescription
                   };
        }

        /// <summary>
        /// Создает атрибут с соответствующими значениями полей <see cref="DataChannelInfo"/>
        /// </summary>
        protected DataChannelAttribute(int index, string description, DataChannelFormat format, string formatUserDescription)
        {
            Info = new DataChannelInfo
            {
                Number = index,
                Description = description,
                Format = format,
                FormatUserDescription = formatUserDescription
            };
        }

    }

    /// <summary>
    /// Атрибут, описывающий канал входных данных
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
    public class InDataAttribute: DataChannelAttribute
    {

        /// <summary>
        /// Описание одного из каналов принимаемых данных и их формат
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        /// <param name="formatDescription">описание формата принимаемых данных</param>
        public InDataAttribute(int index, string description, Type format, string formatDescription = "")
            : base(index, description, format, formatDescription) {}


        /// <summary>
        /// Описание одного из каналов принимаемых данных и их формат
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        public InDataAttribute(int index, string description, Type format)
            : base(index, description, format, "") { }


        /// <summary>
        /// Описание одного из каналов принимаемых данных и их формат
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        /// <param name="formatDescription">описание формата принимаемых данных</param>
        public InDataAttribute(int index, string description,
            DataChannelFormat format, string formatDescription = "")
            : base(index, description, format, formatDescription) { }


        /// <summary>
        /// Описание одного из каналов принимаемых данных и их формат
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        public InDataAttribute(int index, string description,
            DataChannelFormat format)
            : base(index, description, format, "") { }

    }

    /// <summary>
    /// Атрибут, описывающий канал выходных данных
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
    public class OutDataAttribute: DataChannelAttribute
    {
        /// <summary>
        /// Описание одного из каналов отправляемых данных
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        /// <param name="formatDescription">описание формата принимаемых данных</param>
        public OutDataAttribute(int index, string description, Type format, string formatDescription = "")
            : base(index, description, format, formatDescription) {}

        /// <summary>
        /// Описание одного из каналов отправляемых данных
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        public OutDataAttribute(int index, string description, Type format)
            : base(index, description, format, "") { }

        /// <summary>
        /// Описание одного из каналов отправляемых данных
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        /// <param name="formatDescription">описание формата принимаемых данных</param>
        public OutDataAttribute(int index, string description,
            DataChannelFormat format, string formatDescription = "")
            : base(index, description, format, formatDescription) { }

        /// <summary>
        /// Описание одного из каналов отправляемых данных
        /// </summary>
        /// <param name="index">номер канала</param>
        /// <param name="description">описание</param>
        /// <param name="format">тип принимаемых данных</param>
        public OutDataAttribute(int index, string description,
            DataChannelFormat format)
            : base(index, description, format, "") { }

    }

}