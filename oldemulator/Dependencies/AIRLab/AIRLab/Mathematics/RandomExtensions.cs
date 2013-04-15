﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Mathematics
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random rnd, double min, double max)
        {
            return rnd.NextDouble() * (max - min) + min;
        }
    }
}
