using System;
namespace AIRLab.Thornado
{

    /// <summary>
    /// Типизированный класс форматов ввода-вывода. Переопределяет <see cref="ParseObject"/>, <see cref="WriteObject"/>, <see cref="Type"/> и <see cref="DefaultObject"/>, перенаправляя их в типизированные аналоги. С типизированными форматами удобнее работать, поэтому все форматы ввода-вывода реально наследуются от <see cref="TypeFormat<T>"/>, а не от <see cref="TypeFormat"/>
    /// Как правило, не следует создавать непосредственных наследников этого типа.
    /// </summary>
    public abstract class TypeFormat<T> : TypeFormat
    {
        #region Типизированные абстрактные методы
        /// <summary>
        /// Парсит объект заданного типа (типизированный аналог <see cref="TypeFormat.ParseObject"/>)
        /// </summary> 
        public abstract T Parse(string str);
        /// <summary>
        /// Записывает объект заданного типа в строку (типизированный аналог <see cref="TypeFormat.WriteObject"/>)
        /// </summary>
        public abstract string Write(T obj);
        #endregion
        #region Перегрузка нетипизированных методов
        /// <inheritdoc/>
        public override string WriteObject(object obj)
        {
            return Write((T)obj);
        }
        /// <inheritdoc/>
        public override object ParseObject(string str)
        {
            return (T)Parse(str);
        }
        /// <inheritdoc/>
        public override object DefaultObject
        {
            get { return default(T); }
        }
        /// <inheritdoc/>
        public override Type Type
        {
            get { return typeof(T); }
        }
        #endregion
     
        #region Общие модификации

        /// <summary>
        /// Модификация форматов ввода-вывода, которая транслирует значение по умолчанию в пустую строку, и обратно. Используется, например, в графических интерфейсах, чтобы не забивать поля нулями.
        /// </summary>
     //   public DefaultAsEmptyFormat<T> DefaultAsEmpty { get { return new DefaultAsEmptyFormat<T>(this); } }

     //   public PrecisionFormat<T> WithPrecision(int precision) { return new PrecisionFormat<T>(this, precision); } 

        #endregion

    }
}