using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AIRLab.Thornado;

namespace AIRLab.Thornado {
    /// <summary>
    /// Ошибка целостности данных
    /// </summary>
    public class LogicError {
        /// <summary>
        /// Источник ошибки, определяется контекстом возникновения
        /// </summary>
        public readonly LogicErrorType Type;

        /// <summary>
        /// Уровень ошибки
        /// </summary>
        public readonly LogicErrorLevel Level;

        /// <summary>
        /// Источник ошибки
        /// </summary>
        public readonly LogicErrorSource Source;

        /// <summary>
        /// Контекст возникновения ошибки 
        /// </summary>
        public readonly string Context;

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Адрес ошибки в дереве данных
        /// </summary>
        public readonly FieldAddress Address;

        /// <summary>
        /// Полное сообщение об ошибке
        /// </summary>
        public string ExtendedMessage {
            get {
                return "[" + Level + "]" + Context + ": " + Message;
            }
        }

        /// <summary>
        /// Создает ошибку
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="level">Уровень</param>
        /// <param name="source">Источник</param>
        /// <param name="context">Контекст</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="address">Адрес ошибки в дереве данных</param>
        public LogicError (LogicErrorType type, LogicErrorLevel level, LogicErrorSource source, string context, string message, FieldAddress address) {
            this.Type = type;
            this.Level = level;
            this.Source = source;
            this.Context = context;
            this.Message = message;
            this.Address = address;
        }
    }
}