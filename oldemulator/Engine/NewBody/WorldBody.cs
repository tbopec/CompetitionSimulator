using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core
{
    public class WorldBody : BodyCollection<Body>
    {
        public WorldBody()
        {
            World = this;
        }
    }
}
