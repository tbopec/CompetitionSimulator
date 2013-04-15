namespace GemHunt.Server.Sensors
{
	public interface ISensor<out TData>
	{
		TData Measure();
	}
}
