namespace AIRLab.Thornado {

    /// <summary>
    /// Интерфейс класса, содержащего проверку ошибок
    /// </summary>
    public interface ICheckable {
        /// <summary>
        /// Проверяет совместность собственных примитивных полей, не вызывая рекурсивно проверку целостности нод
        /// </summary>
        /// <param name="list"></param>
        void CheckSelfConsistancy (LogicErrorList list);
    }

    public static class ICheckableExtensions
    {
        public static LogicErrorList CheckConsistancy(this ICheckable obj)
        {
            
            var list = new LogicErrorList(LogicErrorType.Internal);
            TypeResolver.CheckConsistancy(obj, list);
            list.ThrowException(LogicErrorLevel.Error);
            return list;
        }
    }
}