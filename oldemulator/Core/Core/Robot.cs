using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using AIRLab.Mathematics;
using Eurosim.Core.Physics;
using System;


namespace Eurosim.Core
{
    public class Robot : PhysicalPrimitiveBody
    {
    	public IEnumerable<SensorModel> SensorModels { get { return Navigators.Cast<SensorModel>().Concat(MagicEyes).Concat(RobotCameras); } }

    	public BodyCollection<NavigatorModel> Navigators { get; private set; }
    	public BodyCollection<MagicEye> MagicEyes { get; private set; }
    	public BodyCollection<Actuator> Actuators { get; private set; }
    	public BodyCollection<RobotCamera> RobotCameras { get; private set; }

    	public Emulator Emulator { get; private set; }
    	public int RobotNumber { get; private set; }

    	public ActionQueue<IRobotAction> Movements { get; private set; }

    	public RobotSettings Settings { get; private set; }

    	private readonly RobotAI _bot;
		private double _currentAiRemainingTime;

    	public Robot(Emulator emulator, int robotNumber,
    	             RobotSettings settings, RobotAIFactory aiFactory)
    	{	//TODO. Пример того, почему физика в телах - это очень плохо. 
			//Тривиальная property с очень нетривиальными побочными эффектами
			//Хочется убрать бессмысленное присваивание, но страшно.
			Location = new Frame3D();
    		
			RobotNumber = robotNumber;
    		Emulator = emulator;
    		Settings = settings;
    		Name = "Robot" + robotNumber;
    		//определяем модель
    		PhysicalModel = MakeModel().PhysicalModel;
    		// Пусть у робота будет упрощённая симуляция. Без неё он может недокручиваться, если держит
    		// пешки, волокущиеся по полу.
//    		JoinWithFloorFriction = false;
    		Movements = new ActionQueue<IRobotAction>();
    		Add(Navigators = new BodyCollection<NavigatorModel>());
    		Add(MagicEyes = new BodyCollection<MagicEye>());
    		Add(Actuators = new BodyCollection<Actuator>());
    		Add(RobotCameras = new BodyCollection<RobotCamera>());
    		if(!string.IsNullOrEmpty(settings.AI))
    		{
    			_bot = aiFactory(settings.AI) ?? Emulator.Rules.CreateAI(settings.AI);
    			if(_bot != null)
    				_bot.DefineRobot(this);
    		}
    		else
    			InitializeRobotWithSettings();
    	}

    	public void InitializeRobotWithSettings()
    	{
    		foreach(EmulatedNavigatorSettings n in Settings.Navigators)
    			Navigators.Add(new NavigatorModel(this, n));
    		foreach(MagicEyeSettings n in Settings.MagicEyes)
    			MagicEyes.Add(new MagicEye(this, n));
    		foreach(ActuatorSettings a in Settings.Actuators)
    			Actuators.Add(Emulator.Rules.CreateActuator(this, a));
    		foreach(RobotCameraSettings n in Settings.RobotCameras)
    			RobotCameras.Add(new RobotCamera(this, n));
    	}

    	public void PerformAI(double dt)
    	{
    		if(_bot == null) return;
    		_currentAiRemainingTime -= dt;
    		if(_currentAiRemainingTime > 0) return;
    		ACMSensorInfo data = ACMSensorInfo.Create(this);
    		ACMCommand cmd = _bot.Perform(data);
    		cmd.Apply(this);
    		_currentAiRemainingTime = cmd.NextRequestInterval;
    	}

    	public void MoveRobot(double dt)
    	{
    		var offset = new Frame3D();
    		lock(Movements)
    		{
    			IEnumerable<ActionQueueSelection<IRobotAction>> movs = Movements.Dequeue(dt);
    			foreach(var m in movs)
    			{
    				if(m.Action is IPlaneMovement)
    				{
    					Frame2D dist = (m.Action as IPlaneMovement).GetOffset(m.StartTime, m.DTime);
    					offset = offset.Apply(new Frame3D(dist.X, dist.Y, 0, Angle.Zero, dist.Angle, Angle.Zero));
    				}
    				if(m.Action is ActuatorExternalAction)
    				{
    					var l = m.Action as ActuatorExternalAction;
    					Actuators.ElementAt(l.ActuatorNumber).DoActions(l, m.StartTime, m.EndTime);
    				}
    			}
    		}
    		foreach(Actuator act in Actuators)
    		{
    			lock(act.Actions)
    			{
    				foreach(var t in act.Actions.Dequeue(dt))
    					act.DoActions(t.Action, t.StartTime, t.EndTime);
    			}
    		}
    		//return;
    		if(double.IsNaN(offset.X) ||
    			double.IsNaN(offset.Y) ||
    			double.IsNaN(offset.Z) ||
    			double.IsNaN(offset.Pitch.Grad) ||
    			double.IsNaN(offset.Yaw.Grad) ||
    			double.IsNaN(offset.Roll.Grad)) 
				return;
    		if(Emulator.Settings.PhysicsMode == PhysicalEngines.No)
    		{
    			Location = Location.Apply(offset);
    			return;
    		}
    		//-------------------------------------------------------------
    		//if (offset.X == 0 && offset.Y == 0 && offset.Z == 0 && offset.Yaw.Radian == 0)
    		//    return;
    		if(dt < 10e-5) return; //TODO бывает проскакивает какое-то кривое dt
    		Frame3D frame = Location.Apply(offset);
    		var point = new Point2D(frame.X - Location.X, frame.Y - Location.Y);
    		double angle = offset.Yaw.Radian;
    		//Главное тело робота
    		IPhysical mainBody;
    		try
    		{
    			mainBody = PhysicalModel;
    		}
    		catch(Exception ex)
    		{
    			Debug.WriteLine("Can't get robot's main body:\n" + ex);
    			return;
    		}
    		//if (point.X == 0 && point.Y == 0) return;
    		mainBody.Velocity = new Frame3D(point.X / dt, point.Y / dt, 0,
    		                                Angle.Zero, Angle.FromRad(angle / dt), Angle.Zero); //TODO не сработает для 3д
    		//this.Location = this.Location.Apply(offset);
    		//Emulator.Rules.MoveRobot(this, offset, dt);
    	}

    	private IPhysicalBody MakeModel()
    	{
    		var bigBox = new PhysicalPrimitiveBody(new BoxShape(10, 20, 30), Color.LightSlateGray)
    		             {
    		             	Location = new Frame3D(0, 0, 0),
    		             	Mass = 30,
    		             	//FrictionCoefficient = 0.3
    		             };
    		bigBox.PhysicalModel.ActAs2d = true; //временно
    		Add(bigBox);
    		return bigBox;
    	}

    	public void Reset()
    	{
    		Movements = new ActionQueue<IRobotAction>();
    		foreach(SensorModel sensorModel in SensorModels)
    			sensorModel.Reset();
    		foreach(Actuator actuator in Actuators)
    			actuator.Reset();
    	}
    }
}
