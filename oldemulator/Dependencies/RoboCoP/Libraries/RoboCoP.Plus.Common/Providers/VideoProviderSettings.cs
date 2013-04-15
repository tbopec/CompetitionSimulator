



namespace RoboCoP.Common
{
using AIRLab.Thornado;

using RoboCoP.Plus;


///<summary>
/// Config of service, that produce video
///</summary>
public partial class VideoProviderSettings : SensorSettings
{

///<summary>
/// Create VideoProducerSettings
///</summary>
public VideoProviderSettings(){
  
    Compress=true;
    Angle=0;
    Margins= new int[]{0,0,0,0};
    ResizeQ=1;
}
		
///<summary>
/// True, if you want to print Timestamp in each image
///</summary>
[Thornado("True, if you want to print Timestamp in each image")]
public bool EnableTimestamp { get; set; }
		
///<summary>
/// True, if you want to compress image by Jpeg filter (increase speed, but decrease quality)
///</summary>
[Thornado("True, if you want to compress image by Jpeg filter (increase speed, but decrease quality)")]
public bool Compress { get; set; }
		
///<summary>
/// Angle (in degrees) to rotate image clockwise
///</summary>
[Thornado("Angle (in degrees) to rotate image clockwise")]
public double Angle { get; set; }
		
///<summary>
/// Margins, that will be cutted. You must type 4 margin: left|top|right|bottom
///</summary>
[Thornado("Margins, that will be cutted. You must type 4 margin: left|top|right|bottom")]
public int[] Margins { get; set; }
		
///<summary>
/// Coefficient of decreasing image size. It is after cut margins
///</summary>
[Thornado("Coefficient of decreasing image size. It is after cut margins")]
public double ResizeQ { get; set; }
}
}

























