namespace Eurosim.Graphics
{
	/// <summary>
	/// ������ - �����, ������� ���������� ��� Transform,
	/// �����������, ����� ���������� ����� ������
	/// </summary>
	/// <typeparam name="T">��� Transform</typeparam>
	public interface ICamera<out T>
	{
		T ViewTransform { get; }
		T WorldTransform { get; }
		T ProjectionTransform { get; }

		/// <summary>
		/// ����������� ������ ������
		/// </summary>
		double AspectRatio { get; set; }
	}
}