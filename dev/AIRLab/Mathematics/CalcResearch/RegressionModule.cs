using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRLab.Mathematics.CalcResearch
{
	/// <summary>
	/// Класс приближает мат. функцию по значения в точках + производным функции, приближаемая функция должна быть "непрерывна" в этих точках
	/// </summary>
	public class RegressionModule
    {
		/// <summary>
		/// Функция к которой будет производиться приближение
		/// </summary>
		private readonly Func<double, RegressExperimentData, double> _regressFunctions;
		/// <summary>
		/// Производные функции
		/// </summary>
		private readonly Func<double, RegressExperimentData, double>[] _regressionDerivativesFunctions;

		/// <summary>
		/// Погрешность сравнений
		/// </summary>
		private const double EpsilonCompare = 0.000000001;

		public RegressionModule(Func<double, RegressExperimentData, double> regressFunctions, Func<double, RegressExperimentData, double>[] regressionDerivatives)
		{
			_regressFunctions = regressFunctions;
			_regressionDerivativesFunctions = regressionDerivatives;
		}

		/// <summary>
		/// Вернуть оптимальные нормализованные параметры регрессии
		/// </summary>
		/// <param name="points">исходная функция -  заданная в конечном наборе точек </param>
		/// <param name="step">Шаг регрессии ?</param>
		/// <param name="epsilonMethod">Допустимая погрешность метода</param>
		/// <returns></returns>
		public RegressExperimentData GetOptimalApproximate(Point2D[] points, double step =0.1, double epsilonMethod = 0.0001)
		{
			var startApproximation = new RegressExperimentData(new []{0.5,0.5});
			var normalizePoints = NormalazeLinery(points).ToArray();
			var lastError = double.MaxValue -1;
			var currentError = 0.0;
			while(Math.Abs(lastError-currentError)>epsilonMethod)
			{
				startApproximation = GetBetterParam(normalizePoints, startApproximation, step);
				lastError = currentError;
				currentError = GetErrorRegress(normalizePoints, startApproximation);
			}
			var minY = points.Min(point => point.Y);
			var maxY = points.Max(point => point.Y);
			
			var minX = points.Min(point => point.X);
			var maxX = points.Max(point => point.X);

			//В функции параметры подобраны для нормализованных аргументов +>  перед вычислением аргумента следует проводить нормолизацию
			return new RegressExperimentData(startApproximation.Args, minX, maxX, minY, maxY);
		}

		/// <summary>
		/// Находит ошибку регрессии
		/// </summary>
		/// <param name="points">исходная функция -  заданная в конечном наборе точек и уже нормализованна</param>
		/// <param name="approximation">Приближенные параметры регрессирующей функции </param>
		/// <returns>Ошибка</returns>
		private double GetErrorRegress(Point2D[] points, RegressExperimentData approximation)
		{
			double error = points.Sum(point => Percent(_regressFunctions(point.X, approximation), point.Y));
			error /= points.Length;
			return error;
		}
		/// <summary>
		/// Функция переводящая другой набор точек в квадрат [0,1]X[0,1]
		/// </summary>
		/// <param name="points">Исходный набор точек</param>
		/// <returns>Нормализованный набор точек</returns>
		private IEnumerable<Point2D> NormalazeLinery(Point2D[] points)
		{
			var maxX = points.Select(point => Math.Abs(point.X)).Max();
			var maxY = points.Select(point => Math.Abs(point.Y)).Max();
			return points.Select(point => new Point2D(point.X / maxX, point.Y / maxY));
		}

		/// <summary>
		/// Дать относительную ошибку?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private double Percent(double a, double b)
		{
			if (Math.Abs(a - b) < EpsilonCompare) return 0;
			return Math.Abs(a - b) / Math.Max(Math.Abs(a), Math.Abs(b));
		}
		/// <summary>
		/// Улучшить параметр регрессии
		/// </summary>
		/// <param name="points">Исходная функция</param>
		/// <param name="startApproximate">Начальные параметры</param>
		/// <param name="step">Шаг регрессии</param>
		/// <returns>Улучшеный параметры регрессируемой функции</returns>
		private RegressExperimentData GetBetterParam(Point2D[] points, RegressExperimentData startApproximate, double step)
        {
			var delta = new double[startApproximate.Args.Length];

			foreach (var point in points)
			{
				var regressionPoint = _regressFunctions(point.X, startApproximate);
				var errorRegress = regressionPoint - point.Y;
                for (int j = 0; j < delta.Length; j++)
                {
					var dfda = _regressionDerivativesFunctions[j](point.X, startApproximate);
					delta[j] += (errorRegress * dfda) / points.Length;
                }
            }
			return new RegressExperimentData(startApproximate.Args.Select((param, i) => param - step * delta[i]).ToArray());
        }
    }
}
