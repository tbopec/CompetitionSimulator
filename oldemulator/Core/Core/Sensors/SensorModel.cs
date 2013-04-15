using System.Threading;
using RoboCoP.Common;

namespace Eurosim.Core
{
	//Эта штука представляет собой сенсор, вмонтированный в эмуль, и служит бриджем между EmulatedSensor и Emulator (иначе будут проблемы н трейдинге)
	public abstract class SensorModel : BodyCollection<Body>
	{
		protected SensorModel(Robot robot)
		{
			Robot = robot;
		}

		public object RequestMeasure()
		{
			_resetEvent.Reset();
			_resetEvent.Wait();
			return _measurementResult;
		}

		public void ProvideMeasure()
		{
			if(_resetEvent.IsSet) return;
			_measurementResult = InternalMeasure();
			_resetEvent.Set();
		}

		public virtual void Reset()
		{
		}

		public readonly Robot Robot;

		public abstract object InternalMeasure();
		private object _measurementResult;
		private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim();
	}

	public abstract class SensorModel<T> : SensorModel
		where T : SensorSettings
	{
		protected SensorModel(Robot robot, T settings)
			: base(robot)
		{
			Settings = settings;
		}

		public T Settings { get; private set; }
	}
}