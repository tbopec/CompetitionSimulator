using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core
{
    public enum RobotMovementQueueElementType
    {
        Movement,
        Actuator
    }

    public class RobotMovementQueueElement
    {
        public IMovement Movement { get; private set; }
        public Action CallBack { get; private set; }
        public int ActuatorNumber { get; private set; }
        public string ActuatorCommand { get; private set; }
        public double RemainingTime { get; private set; }
        public double ElapsedTime { get; private set; }
        public RobotMovementQueueElementType Type { get; private set; }
        public bool NotStarted { get { return ElapsedTime == 0; } }

        public double Tick(double dt)
        {
            if (RemainingTime > dt)
            {
                RemainingTime -= dt;
                ElapsedTime += dt;
                return 0;
            }
            else if (0 < RemainingTime && RemainingTime < dt)
            {
                var rem = RemainingTime;
                RemainingTime = 0;
                rem = dt - rem;
                ElapsedTime += rem;
                return rem;
            }
            else
            {
                return dt;
            }
        }

        public bool Elapsed { get { return RemainingTime<=0; } }

        public RobotMovementQueueElement(IMovement movement)
        {
            this.Movement = movement;
            RemainingTime=movement.TotalTime;
            ActuatorNumber = -1;
            Type = RobotMovementQueueElementType.Movement;
        }

        public RobotMovementQueueElement(IMovement movement, Action callBack)
            : this(movement)
        {
            this.CallBack = callBack;
        }


        public RobotMovementQueueElement(int ActuatorNumber, string ActuatorCommand, double Time)
        {
            RemainingTime = Time;
            this.ActuatorCommand = ActuatorCommand;
            this.ActuatorNumber = ActuatorNumber;
            Type = RobotMovementQueueElementType.Actuator;
        }
    }
}
