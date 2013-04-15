



namespace RoboCoP.Plus.Common
{
using AIRLab.Thornado;
using AIRLab.Thornado.IOs;
    using RoboCoP.Plus;


///<summary>
/// Config of service, that produce video
///</summary>
public partial class VideoProducerSettings : ServiceSettings
{

///<summary>
/// Create VideoProducerSettings
///</summary>
public VideoProducerSettings(){
    EnableTimer=true;
    TimerInterval=1000;
    Compress=true;
    Angle=0;
    Margins= new int[]{0,0,0,0};
    ResizeQ=1;
}

///<summary>
///True, if camera must send images on timer. Default true
///</summary>
[ThornadoField("True, if camera must send images on timer", typeof(BoolIO))]
public bool EnableTimer { get; set; }
		
///<summary>
/// Time interval per that sending new image
///</summary>
[ThornadoField("Time interval per that sending new image", typeof(IntIO))]
public int TimerInterval { get; set; }
		
///<summary>
/// True, if you want to print Timestamp in each image
///</summary>
[ThornadoField("True, if you want to print Timestamp in each image", typeof(BoolIO))]
public bool EnableTimestamp { get; set; }
		
///<summary>
/// True, if you want to compress image by Jpeg filter (increase speed, but decrease quality)
///</summary>
[ThornadoField("True, if you want to compress image by Jpeg filter (increase speed, but decrease quality)", typeof(BoolIO))]
public bool Compress { get; set; }
		
///<summary>
/// Angle (in degrees) to rotate image clockwise
///</summary>
[ThornadoField("Angle (in degrees) to rotate image clockwise", typeof(DoubleIO))]
public double Angle { get; set; }
		
///<summary>
/// Margins, that will be cutted. You must type 4 margin: left|top|right|bottom
///</summary>
[ThornadoField("Margins, that will be cutted. You must type 4 margin: left|top|right|bottom", typeof(IntIO), TypeIOModifier.InArray)]
public int[] Margins { get; set; }
		
///<summary>
/// Coefficient of decreasing image size. It is after cut margins
///</summary>
[ThornadoField("Coefficient of decreasing image size. It is after cut margins", typeof(DoubleIO))]
public double ResizeQ { get; set; }
}

}

























