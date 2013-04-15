using System;
using RoboCoP.Common;
using RoboCoP.Plus;
using RoboCoP.Plus.Common;

namespace Eurosim.Core
{
	public class MovementProvider<TSettings, TData> : ServiceProvider<TSettings>
		where TSettings : ConfirmingServiceSettings, new()
		where TData : IPlaneMovement
	{
		public MovementProvider(Robot robot, ServiceApp<TSettings> app, Func<TSettings, TData, TData> Initialization)
			: base(app)
		{
			Robot = robot;
			_initialization = Initialization;
			if(app.Service.In.Count != 0)
				app.Service.In[0].ReceiveObjectAll<TData>(ProcessPackage);
		}

		public Robot Robot { get; private set; }

		private void ProcessPackage(TData package)
		{
			package = _initialization(App.Settings, package);
			lock(Robot.Movements)
				Robot.Movements.Enqueue(package, CallBack);
		}

		private void CallBack()
		{
			if(!string.IsNullOrEmpty(App.Settings.Main))
				KnownSignal.MovementComplete.SendToMailslot(App, App.Settings.Main);
		}

		private readonly Func<TSettings, TData, TData> _initialization;
	}
}