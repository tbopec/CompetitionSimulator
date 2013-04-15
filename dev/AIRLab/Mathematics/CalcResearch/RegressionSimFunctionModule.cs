using System;
using System.Linq;

namespace AIRLab.Mathematics.CalcResearch
{
	/// <summary>
	/// Класс регрессирующий, почти антисимметричные функции проходящие через точку (0,0)
	/// </summary>
	public class RegressionSimFunctionModule
	{
		private readonly RegressionModule _module;
		private readonly Func<double, RegressExperimentData, double> _regressFunctions;
		public RegressionSimFunctionModule(Func<double, RegressExperimentData, double> regressFunctions, Func<double, RegressExperimentData, double>[] regressionDerivatives)
		{
			_regressFunctions = regressFunctions;
			_module = new RegressionModule(regressFunctions, regressionDerivatives);
		}
		public SimRegressFunction GetRegressFunctionUse(Point2D[] points, double step= 0.01, double epsilonMethod=0.0000001)
		{
			var positiveFunc = points.Where(point => point.X > 0);
			var negativeFunc = points.Where(point=> !positiveFunc.Contains(point));
			var regressPositiveFunc = _module.GetOptimalApproximate(positiveFunc.ToArray());
			var regressNegativeFunc = _module.GetOptimalApproximate(negativeFunc.ToArray());
			return new SimRegressFunction(_regressFunctions, regressNegativeFunc, regressPositiveFunc);
		}
	}

	/// <summary>
	/// Функция регрессии почти симметричной функции проходящей через ноль
	/// </summary>
	public class SimRegressFunction :IRegressFunction
	{
		private readonly Func<double, RegressExperimentData, double> _regressFunctions;
		public RegressExperimentData RegressNegativeParam { get; private set; }
		public RegressExperimentData RegressPositiveParam { get; private set; }
		public double MaxX {get { return RegressPositiveParam.MaxDomain; }}
		public double MinX {get { return RegressNegativeParam.MinDomain; }}

		public SimRegressFunction(Func<double, RegressExperimentData, double> regressFunctions, RegressExperimentData regressNegativeParam, RegressExperimentData regressPositiveParam)
		{
			_regressFunctions = regressFunctions;
			RegressNegativeParam = regressNegativeParam;
			RegressPositiveParam = regressPositiveParam;
		}
		public double GetValue(double x)
		{
			double normX, normY;
			if (x > 0)
			{
				if (RegressPositiveParam.MaxDomain < x)
					return RegressPositiveParam.MaxValue;
				if (RegressPositiveParam.MinDomain > x)
					return 0;
				normX = x/RegressPositiveParam.MaxDomain;
				normY = _regressFunctions(normX, RegressPositiveParam) * RegressPositiveParam.MaxValue;
				if (normY > RegressPositiveParam.MaxValue)
					return RegressPositiveParam.MaxValue;
				if (normY < RegressPositiveParam.MinValue)
					return 0;
				return normY;
			}
			if (RegressNegativeParam.MinDomain > x)
				return RegressNegativeParam.MinValue;
			if (RegressNegativeParam.MaxDomain < x)
				return 0;
			normX = x/Math.Abs(RegressNegativeParam.MinDomain);
			normY = _regressFunctions(normX, RegressNegativeParam)*Math.Abs(RegressNegativeParam.MinValue);
			if (normY < RegressNegativeParam.MinValue)
				return RegressNegativeParam.MinValue;
			if (normY > RegressNegativeParam.MaxValue)
				return 0;
			return normY;
		}
	}
}
