using System.Linq;

namespace AIRLab.Mathematics.CalcResearch
{
	/// <summary>
	/// Параметры регрессионной функции
	/// </summary>
	public class RegressExperimentData
	{
		/// <summary>
		/// Параметры функции, подобранные приближенно. Как правило эти параметры НОРМАЛИЗОВАНЫ ЛИНЕЙНО!!!
		/// </summary>
		public double[] Args { get; private set; }
		/// <summary>
		/// Минимальная область определния
		/// </summary>
		public double MinDomain { get; private set; }
		/// <summary>
		/// Максимальная область определения
		/// </summary>
		public double MaxDomain { get; private set; }
		/// <summary>
		/// Минимальное значение
		/// </summary>
		public double MinValue { get; private set; }
		/// <summary>
		/// Максимальное значение
		/// </summary>
		public double MaxValue { get; private set; }
		public RegressExperimentData(double[] args)
		{
			Args = args;
		}

		public RegressExperimentData(double[] args, double minDomain, double maxDomain, double minValue, double maxValue)
		{
			Args = args;
			MinDomain = minDomain;
			MaxDomain = maxDomain;
			MinValue = minValue;
			MaxValue = maxValue;
		}
		public override string ToString()
		{
			var message = Args.Aggregate("Args: ", (current, arg) => string.Format("{0}, {1}", current, arg));
			return string.Format("{0}| X:[{1};{2}] Y:[{3};{4}]", message, MinDomain, MaxDomain, MinValue, MaxValue);
		}
	}
}