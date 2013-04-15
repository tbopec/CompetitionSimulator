using System;
using System.Collections.Generic;
using System.Net;
using Eurosim.ClientLib;

namespace Eurosim.NetworkClient
{
	internal class SampleProgram
	{
		private static void Main(string[] args)
		{
			string add = "127.0.0.1";
			if(args.Length > 0 && !string.IsNullOrEmpty(args[0]))
				add = args[0];
			using(var client = new EurosimClient(new DnsEndPoint(add, Port), "replay.file"))
			{
				string serializedSensorInfo;
				client.Hello(GetHello(), out serializedSensorInfo);
				foreach(MovementCommand command in SimpleCommandSequence)
					client.Move(command);
				client.End();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Finished!");
			}
		}

		private static readonly List<MovementCommand> SimpleCommandSequence =
			new List<MovementCommand>
				{
					MovementCommand.LineMovement(56),
					MovementCommand.AngleRotation(-90),
					MovementCommand.LineMovement(45),
					MovementCommand.ActuatorAction("Grip", 0),
					MovementCommand.LineMovement(10),
					MovementCommand.ActuatorAction("Grip", 0),
					MovementCommand.LineMovement(20),
					MovementCommand.AngleRotation(-56),
					MovementCommand.LineMovement(15),
					MovementCommand.ActuatorAction("Release", 0),
					MovementCommand.LineMovement(-20),
				};

		private static HelloRequest GetHello()
		{
			return new HelloRequest
			       	{
			       		Affiliation = "URFU",
			       		City = "Ekaterinburg",
			       		Email = "a@bc.com",
			       		Name = "Test team",
			       		Position = Position.Left,
						Opponent = Opponents.Nocturnal,
			       		Field = 817
			       	};
		}

		private const int Port = 9876;
	}
}