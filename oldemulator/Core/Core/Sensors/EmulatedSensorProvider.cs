using RoboCoP.Common;
using RoboCoP.Plus;

namespace Eurosim.Core
{
	public class EmulatedSensorProvider<T> : SensorProvider<T>
		where T : SensorSettings, new()
	{
		public EmulatedSensorProvider(SensorModel model, ServiceApp<T> app)
			: base(app)
		{
			_sensorModel = model;
		}

		public override object GetSensorData()
		{
			return _sensorModel != null ? _sensorModel.RequestMeasure() : null;
		}

		private readonly SensorModel _sensorModel;
	}
}