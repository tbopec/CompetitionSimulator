using AIRLab.Mathematics;

namespace Eurosim.Core
{
	public interface IPlaneMovement : IRobotAction
	{
		//этот метод должен показывать координаты смещения, которое произойдет за время dtime, относительно координат смещения, достигнутых в stime
		Frame2D GetOffset(double startTime, double dtime); // startSpeeds- список начальных скоростей на двигателе
		// finishSpeeds- список конечных скоростей на двигателе
	}

	public static class IMovementExtension
	{
		public static Frame2D GetTotalOffset(this IPlaneMovement mov)
		{
			return mov.GetOffset(0, mov.TotalTime);
		}
	}
}