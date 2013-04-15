using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace AIRLab.Mathematics
{
    public class Regression
    {
        public double[] X;
        public double[] Y;
        public double[] A;
        public Func<double, double[], double> RegressionFunction;
        public List<Func<double, double[], double>> RegressionDerivatives = new List<Func<double, double[], double>>();
        public double Step = 0.1;
        public double LastError = 0;

        public double XMin;
        public double XMax;
        public double YMin;
        public double YMax;

        public void NormalizeAffine()
        {
            XMin = X.Min();
            XMax = X.Max();
            YMin = Y.Min();
            YMax = Y.Max();
            Scale();
        }

        public void NormalizeLinear()
        {
            XMin = 0;
            XMax = X.Select(z => Math.Abs(z)).Max();
            YMin = 0;
            YMax = Y.Select(z => Math.Abs(z)).Max();
            Scale();
        }

        void Scale()
        {
            X = X.Select(x => (x - XMin) / (XMax - XMin)).ToArray();
            Y = Y.Select(y => (y - YMin) / (YMax - YMin)).ToArray();
        }

        double Percent(double a, double b)
        {
            if (a == b) return 0;
            return Math.Abs(a - b) / Math.Max(Math.Abs(a), Math.Abs(b));

        }


        public double RegressionIteration()
        {
            var delta = new double[A.Length];
            var funk = X.Select(z => RegressionFunction(z, A)).ToArray();

            for (int i = 0; i < X.Length; i++)
            {
                var c = funk[i] - Y[i];
                for (int j = 0; j < delta.Length; j++)
                {
                    var dfda = RegressionDerivatives[j](X[i], A);
                    delta[j] += c * dfda;
                }
            }

            for (int j = 0; j < delta.Length; j++)
                delta[j] /= X.Length;

            for (int j = 0; j < A.Length; j++)
                A[j] -= Step * delta[j];

            double error=0;
            for (int i = 0; i < X.Length; i++)
                error += Percent(RegressionFunction(X[i], A), Y[i]);
            error /= X.Length;
            LastError = error;
            return error;
        }

       

        public Chart CreateRegressionChart()
        {
            var chart = new Chart();
            var chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            var expSeries = new Series();
            for (int i = 0; i < X.Length; i++)
                expSeries.Points.AddXY(XMin + X[i] * (XMax - XMin), YMin + Y[i] * (YMax - YMin));
            expSeries.ChartType = SeriesChartType.FastPoint;

            chart.Series.Add(expSeries);

            var regSeries = new Series();
            for (double x=-1;x<1;x+=0.01)
                regSeries.Points.AddXY(XMin + x * (XMax - XMin), YMin + RegressionFunction(x,A) * (YMax - YMin));

            regSeries.ChartType = SeriesChartType.FastLine;

            chart.Series.Add(regSeries);

            return chart;
        }
        

        
    }
}
