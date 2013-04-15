using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    /// <summary>
    /// Класс для Tensor/List, для *
    /// </summary>
    public class CollectionCurrentIndex 
    {
        [Thornado]
        public int i { get; set; }
        [Thornado]
        public string LastStar { get; set; }
        public CollectionCurrentIndex() { i = 0; }
        public int Increment()
        {
            i++;
            return i;
        }
    }
}
