using System;

namespace Eurosim.ClientLib
{
	public static class RequestFactory
	{
		public static string HelloPacket(HelloRequest request)
		{
			return String.Format(
				@"<HelloEurosim>
					<Name>{0}</Name>
					<Affiliation>{1}</Affiliation>
					<Email>{2}</Email>
					<City>{3}</City>
					<Position>{4}</Position>
					<Opponent>{5}</Opponent>
					<Field>{6}</Field>
				</HelloEurosim>",
				request.Name, request.Affiliation, request.Email,
				request.City, request.Position, request.Opponent, request.Field);
		}

		public static string CommandPacket(MovementCommand movementCommand)
		{
			if(string.IsNullOrEmpty(movementCommand.ActuatorCommand))
			{
				return
					CommandPacket(movementCommand.Time, movementCommand.Distance, movementCommand.Angle);
			}
			return string.Format(
				@"<Action>
					<NextRequestInterval>{0}</NextRequestInterval>
					<ActuatorCommands><item0>{1}</item0></ActuatorCommands>
				</Action>", 
				movementCommand.Time, movementCommand.ActuatorCommand);
		}

		public static string EndPacket()
		{
			return CommandPacket(int.MaxValue, 0, 0);
		}

		private static string CommandPacket(double time, double distance, double angle)
		{
			return string.Format(
				@"<Action>
					<NextRequestInterval>{2}</NextRequestInterval>
					<ArcMovement>
						<item0>
							<Distance>{0}</Distance>
							<Rotation>{1}</Rotation>
							<TotalTime>{2}</TotalTime>
						</item0>
					</ArcMovement>
				</Action>", distance,angle, time);
		}
	}
}