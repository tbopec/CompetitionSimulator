using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Drawing;
using System.Drawing;

namespace RoboCoP.Plus.Common {

    [Serializable]
    public class RecognitionSample {
        FastBitmap fastBitmap;
        public FastBitmap FastBitmap { get { return fastBitmap; } }
        Bitmap bitmap;
        public Bitmap Bitmap { get { return bitmap; } }
        public RecognitionSample(FastBitmap fast) {
            this.bitmap = fast.ToBitmap();
            this.fastBitmap = fast;
        }
        [NonSerialized]
        internal Rectangle Layout;
    }

    /// <summary>
    /// Contains several samples of one class
    /// </summary>
    [Serializable]
    public class RecognitionClassSamples {
        /// <summary>
        /// Class of samples
        /// </summary>
        public int Class { get; set; }

        List<RecognitionSample> samples = new List<RecognitionSample>();

        /// <summary>
        /// Samples
        /// </summary>
        public List<RecognitionSample> Samples { get { return samples; } }
    }

    [Serializable]
    public class RecognitionBase : List<RecognitionClassSamples> { }

}
