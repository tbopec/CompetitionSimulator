using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;
using AIRLab.Thornado.IOs;

namespace RoboCoP.Plus.Common {
    public class ServoData {
        
        [ThornadoField("", typeof(BoolIO))]
        public bool IsRoot { get; set; }

        [ThornadoField("", typeof(IntIO))]
        public int SeekTime { get; set; }

        [ThornadoField("", typeof(IntIO))]
        public int WaitTime { get; set; }

        List<ServoDOF> sdof = new List<ServoDOF>();
        [ThornadoList("")]
        public List<ServoDOF> DOF { get { return sdof; } set { sdof = value; } }

        /// <summary>
        /// Checking the NaN of DOF
        /// </summary>
        /// <returns>true - if exists invalid signal</returns>
        public bool IsInvalid() {
            foreach (var currentDOF in sdof) {
                if (double.IsNaN(currentDOF.Signal)) return true;
            }
            return false;
        }
    }
}
