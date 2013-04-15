using System;
using System.Threading;
using AIRLab.Thornado;
using Eurosim.ClientLib;
using Eurosim.Core;

namespace EurosimNetworkServer
{
	internal class OnlineBot : RobotAI
	{
		public OnlineBot(NetworkInterface it, Logger logger)
		{
			networkInterface = it;
			this.logger = logger;
			new Thread(DeathClockThread) {IsBackground = true}.Start();
		}

		public void DefineRobot(Robot robot)
		{
			robot.InitializeRobotWithSettings();
		}

		public ACMCommand Perform(ACMSensorInfo info)
		{
			string data = IO.XML.WriteToString(info);
			networkInterface.Write(data);
			EnableTimer = true;	
			string taskStr = networkInterface.Read();
			EnableTimer = false;
			logger.LogCommandCycle(taskStr, info);
			var task = new ACMCommand();
			try
			{
				task = IO.XML.ParseString<ACMCommand>(taskStr);
			}
			catch(Exception exception)
			{
				networkInterface.SendError(exception.Message);
			}
			if(task.NextRequestInterval < CommonConsts.MinRequestInterval)
			{
				networkInterface.SendError(RequestIntervalErrorMessage);
			}
			else if(task.ArcMovement != null)
			{
				foreach(ArcMovement e in task.ArcMovement)
				{
					if(e.TotalTime == 0)
						networkInterface.SendError(ZeroMovementTimeErrorMessage);
					else if(Math.Abs(e.Distance / e.TotalTime) > CommonConsts.MaxLinearSpeed)
						networkInterface.SendError(MaxLinearSpeedErrorMessage);
					else if(Math.Abs(e.Rotation.Grad / e.TotalTime) > CommonConsts.MaxAngularSpeed)
						networkInterface.SendError(MaxAngularSpeedErrorMessage);
				}
			}
			return task;
		}

		#region Часы

		public void DeathClockThread()
		{
			while(!EnableTimer) Thread.Sleep(10);
			DateTime start = DateTime.Now;
			while(EnableTimer)
			{
				var span = DateTime.Now - start;
				if (span > Timeout && !timeoutSent)
				{
					timeoutSent = true;
					networkInterface.SendError("Connection timed out");
				}
				Thread.Sleep(10);
			}
		}

		private bool EnableTimer;

		#endregion


		public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);
		private readonly NetworkInterface networkInterface;
		private readonly Logger logger;
		private bool timeoutSent;
		
		public const string RequestIntervalErrorMessage = "Вы не можете запрашивать данные чаще, чем через 100 милисекунд";
		public const string ZeroMovementTimeErrorMessage = "Вы не можете установить время перемещения в 0";
		public const string MaxLinearSpeedErrorMessage = "Вы превысили максимально разрешенную линейную скорость";
		public const string MaxAngularSpeedErrorMessage = "Вы превысили максимально разрешенную угловую скорость";
	}
}