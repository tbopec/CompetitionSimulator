using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;

namespace Eurosim.Core
{
    [Serializable]
    public class ACMCommand
    {
        public ACMCommand()
        {
            TrivialPlaneMovement = new List<TrivialPlaneMovement>();
            ArcMovement = new List<ArcMovement>();
        }
        [Thornado]
        public double NextRequestInterval { get; set; }


        [Thornado]
        public List<TrivialPlaneMovement> TrivialPlaneMovement { get; set; }

        [Thornado]
        public List<ArcMovement> ArcMovement { get; set; }

        [Thornado]
        public string[] ActuatorCommands { get; set; }
               
        public void Apply(Robot robot)
        {
            if (TrivialPlaneMovement!=null)
                foreach(var m1 in TrivialPlaneMovement)
                    robot.Movements.Enqueue(m1);
            if (ArcMovement != null)
                foreach (var m1 in ArcMovement)
                    robot.Movements.Enqueue(m1);

            if (ActuatorCommands != null)
                for (int i = 0; i < Math.Min(robot.Actuators.Count(), ActuatorCommands.Length); i++)
                    if (!string.IsNullOrEmpty(ActuatorCommands[i]))
                        robot.Actuators.ElementAt(i).Actions.Enqueue(new ActuatorAction { ActuatorCommand = ActuatorCommands[i], TotalTime = 0.1 }); //TODO: на самом деле, тут еще должно быть какое-то время, не 0.1!
        }

        public void Autotime()
        {
            var max = 0.01;
            if (TrivialPlaneMovement != null && TrivialPlaneMovement.Count != 0) max = Math.Max(max, TrivialPlaneMovement.Select(z => z.TotalTime).Sum());
            NextRequestInterval=max;
        }


    }
}