using System;
using System.Windows.Forms;
using Eurosim.Graphics;
using RoboCoP.Plus;
using Eurosim.Core;
using AIRLab.Thornado;
using System.Threading;
using RoboCoP.Plus.Common;


namespace Eurosim
{
    class Program
    {
        static ServiceAppEnvironment commonEnv;

        static ServiceApp<T> CreateApp<T>(string newName, int robotNumber, int boardNumber, T settings)
            where T: ServiceSettings, new()
        {
            var env = new ServiceAppEnvironment(newName, robotNumber+"."+boardNumber, commonEnv);
            return new ServiceApp<T>(env, settings);
        }

        static void CreateServicesForRobot(Robot robot)
        {
            int i = 0;
            
            foreach (var nav in robot.Navigators)
                new EmulatedSensorProvider<EmulatedNavigatorSettings>(
                    nav, CreateApp("Navigator", robot.RobotNumber, i++, nav.Settings));
            i = 0;
            foreach (MagicEye eye in robot.MagicEyes)
                new EmulatedSensorProvider<MagicEyeSettings>(
                    eye,
                    CreateApp("MagicEye", robot.RobotNumber, i++, eye.Settings));
            i = 0;
            foreach (var cam in robot.RobotCameras)
            {
                var prov = new EmulatedSensorProvider<RobotCameraSettings>(
                    cam,
                    CreateApp("Camera", robot.RobotNumber, i++, cam.Settings));
                prov.InitCustomSerializer(z => ((RobotCameraData)z).Bitmap);
            }
            i = 0;
            foreach (var act in robot.Actuators)
            {
                new ActuatorProvider(act,
                    CreateApp("Actuator", robot.RobotNumber, i++, act.Settings));
            }

            i = 0;
            new MovementProvider<ConfirmingServiceSettings, TrivialPlaneMovement>(
                robot,
                CreateApp("TrivialInput", robot.RobotNumber, i++, robot.Settings.TrivialInput),
                (a, b) => b);
            i = 0;
            new MovementProvider<DoubleWheelMovementSettings, DoubleWheelMovement>(
                robot,
                CreateApp("DoubleWheelInput", robot.RobotNumber, i++, robot.Settings.DoubleWheelInput),
                (settings, data) => new DoubleWheelMovement
                {
                    DistanceWheels = settings.DistanceWheels,
                    TotalTime = data.TotalTime,
                    VLeft0 = settings.TransformVelocity(data.VLeft0, true),
                    VLeft1 = settings.TransformVelocity(data.VLeft1, true),
                    VRight0 = settings.TransformVelocity(data.VRight0, false),
                    VRight1 = settings.TransformVelocity(data.VRight1, false),
                });
            i = 0;
            if (robot.Settings.ArcInput != null)
                new MovementProvider<ConfirmingServiceSettings, ArcMovement>(
                    robot,
                    CreateApp("ArcInput", robot.RobotNumber, i++, robot.Settings.ArcInput),
                    (settings, data) => data);

        }


        [STAThread]
        public static void Main(string[] args)
        {


            //var mov = new DoubleWheelMovement { VLeft0 = 0, VLeft1 = 1, VRight0 = 0, VRight1 = -1, TotalTime = 1 };
            //var off = mov.GetOffset(0, 0.1);

            commonEnv = new ServiceAppEnvironment("Eurosim", args);
            var  sets = IO.INI.ParseString<EmulatorSettings>(commonEnv.CfgFileEntry, commonEnv.ServiceName);





            var emulator = new Emulator(sets);
            new EmulatorService(emulator, new ServiceApp<EmulatorSettings>(commonEnv, sets));

            emulator.App = commonEnv;
            foreach (var r in emulator.Robots)
                CreateServicesForRobot(r);

            Thread.Sleep(3000);

            var emulatorThread = new Thread(() => { while (true) emulator.MakeCycle(true); });
            emulatorThread.IsBackground = true;
            emulatorThread.Start();
			if (sets.VideoMode==VideoModes.No)
			{
				var thread = new Thread(() =>
				{
					var f = new Form { Text = "Don't worry. Eurosim is working" };
					Application.Run(f);
				});
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
            emulator.CreateGraphics();
        }
    }
}
