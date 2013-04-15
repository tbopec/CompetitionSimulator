using System.Linq;

namespace AIRLab.Mathematics.CalcResearch
{
	/// <summary>
	/// ��������� ������������� �������
	/// </summary>
	public class RegressExperimentData
	{
		/// <summary>
		/// ��������� �������, ����������� �����������. ��� ������� ��� ��������� ������������� �������!!!
		/// </summary>
		public double[] Args { get; private set; }
		/// <summary>
		/// ����������� ������� ����������
		/// </summary>
		public double MinDomain { get; private set; }
		/// <summary>
		/// ������������ ������� �����������
		/// </summary>
		public double MaxDomain { get; private set; }
		/// <summary>
		/// ����������� ��������
		/// </summary>
		public double MinValue { get; private set; }
		/// <summary>
		/// ������������ ��������
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