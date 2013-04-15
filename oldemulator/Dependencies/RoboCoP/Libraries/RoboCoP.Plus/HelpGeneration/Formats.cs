using System;
namespace RoboCoP
{
    /// <summary>
    /// Атрибут для описания элементов перечисления <see cref="DataChannelFormat"/>. Содержит правило, по которому нужно составлять описания формата
    /// </summary>
    public class FormatEnumAttribute : Attribute
    {
        /// <summary>
        /// Строка, которая описывает формат.
        /// </summary>
        public readonly string SelfDescription;
        /// <summary>
        /// True, если формат требует указания какого-либо типа C# (например, INI-файлы требуют указания типа, который в них содержится)
        /// </summary>
        public readonly bool AddFormatType;
        /// <summary>
        /// True, если нужно добавить текст, который пользователь добавил в описание формата от себя.
        /// </summary>
        public readonly bool AddUserDescription;
        /// <summary>
        /// Создает атрибут с указанными свойствами
        /// </summary>
        public FormatEnumAttribute(string SelfDescription, bool AddFormatType, bool AddUserDescription)
        {
            this.SelfDescription = SelfDescription;
            this.AddFormatType = AddFormatType;
            this.AddUserDescription = AddUserDescription;
        }
    }

    /// <summary>
    /// Форматы данных, поддерживаемые робокопом
    /// </summary>
    public enum DataChannelFormat
    {
        /// <summary>
        /// Изображение
        /// </summary>
        [FormatEnum("Растровое изображение в формате, поддерживаемом операционной системой (BMP,JPG,GIF,PNG)", false, false)]
        Image,
        /// <summary>
        /// Распознанное изображение
        /// </summary>
        [FormatEnum("Распознанное изображение в формате, поддерживаемом классом RoboCoP.Common.ClassifiedBitmap", false, false)]
        ClassifiedImage,
        [FormatEnum("Векторизованное изображение в формате VectorVideoDataIO", false, false)]
        VVD,
        /// <summary>
        /// Ini-файл с одним объектом
        /// </summary>
        [FormatEnum("INI-файл с описанием объекта типа ", true, false)]
        Ini,
        /// <summary>
        /// Ini-файл с массивом объектов
        /// </summary>
        [FormatEnum("INI-файл с описанием массива объектов типа ", true, false)]
        IniList,
        /// <summary>
        /// Пользовательский (произвольный) формат
        /// </summary>
        [FormatEnum("", false, true)]
        Custom
    }
}