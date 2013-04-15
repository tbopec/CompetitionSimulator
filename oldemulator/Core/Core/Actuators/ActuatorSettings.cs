using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;
using AIRLab.Mathematics;
using RoboCoP.Plus;
using RoboCoP.Plus.Common;

namespace Eurosim.Core
{
    [Serializable]
    public class ActuatorSettings : ConfirmingServiceSettings
    {
        [Thornado]
        public string Type { get; private set; }
        //здесь должны быть какие-то общие настройки актуатора, например, его положение относительно центра робота, хотя бы угловое.
        [Thornado] 
        public Frame3D Location;

        /// <summary>
        /// Угол зоны внутри которой действует актуатор
        /// </summary>
        [Thornado] 
        public Angle ActionAngle; 
        /// <summary>
        /// Максимальное расстояние до объекта
        /// </summary>
        [Thornado]
        public double ActionDistance;
        public virtual double GetTimeForAction(string actionName) { return 0.5; }
    }
}
