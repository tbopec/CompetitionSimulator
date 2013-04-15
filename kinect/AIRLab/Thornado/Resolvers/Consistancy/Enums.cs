using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado {
    /// <summary>
    /// Уровень ошибки
    /// </summary>
    public enum LogicErrorLevel {
        /// <summary>
        /// Отсутствие всякой ошибки
        /// </summary>
        No = 0,
        /// <summary>
        /// Данные полностью корректны, но хочется привлечь к ним внимание пользователя
        /// </summary>
        Information = 1,
        /// <summary>
        /// Предупреждение (приложение работоспособно, но данные некоррентны)
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Ошибка (приложение со введенными данными неработоспособно)
        /// </summary>
        Error = 3,
    }

    /// <summary>
    /// Тип ошибки
    /// </summary>
    public enum LogicErrorType {
        /// <summary>
        /// Внутренние действия программы
        /// </summary>
        Internal,
        /// <summary>
        /// Проверка целостности данных перед сохранением в источник данных
        /// </summary>
        Reading,
        /// <summary>
        /// Проверка целостности данных после чтения из источника данных
        /// </summary>
        Writing,
    }

    public enum LogicErrorSource {
        /// <summary>
        /// Ошибка, выброшенная сеттером поля
        /// </summary>
        Field,
        /// <summary>
        /// Ошибка, выброшенная проверкой целостности класса
        /// </summary>
        Class,
        /// <summary>
        /// Ошибка, выброшенная кем-то еще
        /// </summary>
        External
    }

    /// <summary>
    /// Тип операции над данными
    /// </summary>
    public enum LogDataType {
        /// <summary>
        /// Добавление
        /// </summary>
        Add,
        /// <summary>
        /// Удаление
        /// </summary>
        Remove,
        /// <summary>
        /// Изменение
        /// </summary>
        Change
    }
}
