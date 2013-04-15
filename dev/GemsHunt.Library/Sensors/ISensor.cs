namespace GemsHunt.Library.Sensors
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TData">Тип данных сенсора</typeparam>
	public interface ISensor<out TData>
	{
		/// <summary>
		/// Метод производит измерение и возвращает данные, снятые с сенсора.
		/// </summary>
		/// <returns></returns>
		TData Measure();
	}
}
