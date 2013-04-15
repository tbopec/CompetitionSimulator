using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;

namespace Eurosim.Core
{
    public abstract class Actuator : BodyCollection<Body>
    {
        protected Actuator(Robot robot, ActuatorSettings settings)
        {
            Settings = settings;
            Location = Settings.Location;
            Robot = robot; 
            Actions = new ActionQueue<ActuatorAction>();
        }
        public Robot Robot { get; private set; }
        public abstract void DoActions(ActuatorAction action, double startTime, double endTime);
        public ActionQueue<ActuatorAction> Actions { get; private set; }
        public string State { get; protected set; }
        public ActuatorSettings Settings { get; protected set; }
        public virtual void Reset()
        {
            Actions=new ActionQueue<ActuatorAction>();
            State = String.Empty;
        }
    }
}
