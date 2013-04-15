using RoboCoP.Common;
using RoboCoP.Plus;

namespace Eurosim.Core
{
	public class ActuatorProvider : ServiceProvider<ActuatorSettings>
	{
		public ActuatorProvider(Actuator actuator, ServiceApp<ActuatorSettings> app)
			: base(app)
		{
			Actuator = actuator;
			if(app.Service.In.Count > 0)
				app.Service.In[0].ReceiveTextAll(ProcessPackage);
		}

		public Actuator Actuator { get; private set; }

		private void ProcessPackage(string packageText)
		{
			lock(Actuator.Actions)
				Actuator.Actions.Enqueue(
					new ActuatorAction
					{
						ActuatorCommand = packageText,
						TotalTime = App.Settings.GetTimeForAction(packageText)
					}, SendConfirmation);
		}

		private void SendConfirmation()
		{
			KnownSignal.MovementComplete.SendToMailslot(App, App.Settings.Main);
		}
	}
}