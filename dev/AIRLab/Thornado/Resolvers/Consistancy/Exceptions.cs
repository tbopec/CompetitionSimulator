using System;
namespace AIRLab.Thornado {

    /// <summary>
    /// Исключение, возникающее при ошибках в контроле целостности. Содержит в себе список ошибок.
    /// </summary>
    public class LogicException : Exception {
        /// <summary>
        /// Список ошибок при контроле целостности
        /// </summary>
        public readonly LogicErrorList ErrorList;
        /// <summary>
        /// Создает исключение с указанным списком ошибок
        /// </summary>
        public LogicException (LogicErrorList list)
            : base(list.CreateText()) {
            ErrorList = list;
        }
    }

    /// <summary>
    /// Исключение, содержащее уровень ошбки
    /// TODO: может стоит добавить контекст, тип и прочую информацию об ошибке?
    /// </summary>
    public class ThornadoException : Exception {
        /// <summary>
        /// Уровень ошибки
        /// </summary>
        public readonly LogicErrorLevel Level;

        /// <summary>
        /// Создает исключение, с указанным уровнем ошибки
        /// </summary>
        /// <param name="level">уровень ошибки</param>
        /// <param name="message">сообщение об ошибке</param>
        public ThornadoException (LogicErrorLevel level, string message) : base(message) {
            Level = level;
        }
    }
}