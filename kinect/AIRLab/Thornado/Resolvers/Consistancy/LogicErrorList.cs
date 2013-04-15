using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using AIRLab.Thornado;
namespace AIRLab.Thornado {

    /// <summary>
    /// Список ошибок, обнаруженых при проверке целостности
    /// </summary>
    public class LogicErrorList : IEnumerable<LogicError> {
        /// <summary>
        /// Тип ошибки (определяется контекстом возникновения)
        /// </summary>
        public readonly LogicErrorType Type;

        /// <summary>
        /// Контекст возникновения ошибки 
        /// </summary>
        public string Context;

        #region Сессия проверки
        // ?? почему эти поля, свойства и методы объеденены
        // в секцию с таким названием? Что такое сессия и кто
        // этим пользуется?
        /// <summary>
        /// Источник ошибки
        /// </summary>
        LogicErrorSource source;

        /// <summary>
        /// Источник ошибки
        /// </summary>
        public LogicErrorSource Source { get { return source; } }

        /// <summary>
        /// Указывает, что создаваемую ошибку нужно считать коррекцией
        /// </summary>
        bool treatErrorAsCorrection;

        /// <summary>
        /// Указывает, что создаваемую ошибку нужно считать коррекцией
        /// </summary>
        public bool TreatErrorAsCorrection { get { return treatErrorAsCorrection; } }

        /// <summary>
        /// Номер ошибки, с которой начинается текущая сессия
        /// </summary>
        int sessionStartIndex;

        /* Поля и методы связанные с локализацией ошибки
         * 
         * /// <summary>
         * /// Локализации ошибки
         * /// </summary>
         * List<LogicErrorLocation> locations=new List<LogicErrorLocation>();
         * 
         * /// <summary>
         * /// Локализации ошибки
         * /// </summary>
         * public List<LogicErrorLocation> Locations { get { return locations; } }
         * 
         * /// <summary>
         * /// Устанавливает локацию ошибок внутри одного объекта (наиболее типичная ситуация)
         * /// </summary>
         * /// <param name="obj"></param>
         * /// <param name="fields"></param>
         * public void SetLocation(object obj, params string[] fields)
         * {
         *     Locations.Clear();
         *     Locations.Add(new LogicErrorLocation(obj, fields));
         * }
         */

        /// <summary>
        /// Адрес ошибки в дереве данных
        /// </summary>
        FieldAddress address;

        /// <summary>
        /// Адрес ошибки в дереве данных
        /// </summary>
        public FieldAddress Address { get { return address; } }

        /// <summary>
        /// Устанавливает адрес ошибки в дереве данных
        /// </summary>
        /// <param name="address">Адрес ошибки в дереве данных</param>
        public void SetAddress (FieldAddress address) {
            this.address = address;
        }

        /// <summary>
        /// Открывает сессию проверки, т.е. серию проверок со схожими условиями
        /// </summary>
        /// <param name="source">Источник ошибки</param>
        public void StartCheckSession (LogicErrorSource source) {
            sessionStartIndex = errors.Count;
            //    Locations.Clear();
            this.source = source;
        }

        /// <summary>
        /// Открывает сессию проверки, т.е. серию проверок со схожими условиями
        /// </summary>
        /// <param name="source">Источник ошибки</param>
        /// <param name="treatErrorAsCorrection">Считать ошибку скорректированной</param>
        public void StartCheckSession (LogicErrorSource source, bool treatErrorAsCorrection) {
            StartCheckSession(source);
            this.treatErrorAsCorrection = treatErrorAsCorrection;
        }

        /// <summary>
        /// Открывает сессию проверки, т.е. серию проверок со схожими условиями
        /// </summary>
        public void StartCheckSession (LogicErrorSource source, bool threatErrorAsCorrection, FieldAddress address) {
            StartCheckSession(source, threatErrorAsCorrection);
            this.address = address;
        }
        #endregion

        /// <summary>
        /// Список ошибок
        /// </summary>
        readonly List<LogicError> errors=new List<LogicError>();

        /// <summary>
        /// Создает список ошибок с указанным источником
        /// </summary>
        public LogicErrorList (LogicErrorType type) { Type = type; }

        /// <summary>
        /// Добавляет сообщение об ошибке, с заранее указаным адресом
        /// </summary>
        /// <param name="level">уровень</param>
        /// <param name="message">Сообщение об ошибке</param>
        public void Add (LogicErrorLevel level, string message) {
            Add(level, message, this.address);
        }

        /// <summary>
        /// Добавляет сообщение об ошибке
        /// </summary>
        /// <param name="level">Уровень</param>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="address">Адрес ошибки в дереве данных</param>
        public void Add (LogicErrorLevel level, string message, FieldAddress address) {
            // Меняем уровень ошибки, если смогли ее скорректировать
            errors.Add(new LogicError(Type, level, Source, Context, message, address));
        }

        /* Больше не нужный сахар
         * public void AddWithLocationList(LogicErrorLevel level, string message, params LogicErrorLocation[] locs)
         * {
         *     List<LogicErrorLocation> l = new List<LogicErrorLocation>();
         *     foreach (var n in locs)
         *         l.Add(n);
         *     Add(level, message, l);
         * }
         */

        ///<inheritdoc/>
        public IEnumerator<LogicError> GetEnumerator () {
            return errors.GetEnumerator();
        }

        ///<inheritdoc/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
            return errors.GetEnumerator();
        }

        public bool HaveErrors (LogicErrorLevel level, bool inSession) {
            int begin = inSession ? sessionStartIndex : 0;
            for (int i = begin; i < errors.Count; i++)
                if (errors[i].Level >= level) return true;
            return false;
        }

        public bool HaveErrors (LogicErrorLevel level) {
            return HaveErrors(level, false);
        }

        public void ThrowException (LogicErrorLevel level) {
            if (!HaveErrors(level, false)) return;
            throw new LogicException(this);
        }

        /// <summary>
        /// Создает текстовое описание всех ошибок в списке
        /// </summary>
        public string CreateText () {
            string Message = "";
            foreach (LogicError e in this)
                Message += e.ExtendedMessage + "\n";
            return Message;
        }
    }
}