using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AIRLab.Mathematics;
using Eurosim.Core;
using GemsHunt.Library.Sensors;
using NUnit.Framework;

namespace kinect.Integration
{
    public class Kinect:ISensor<KinectData>
	{
        private readonly Body _worldRoot;
        private readonly KinectSettings _settings;
        public Frame3D Location { get; set; }
        public Kinect(Body worldRoot, KinectSettings settings)
		{
			_worldRoot = worldRoot;
			_settings = settings;
            Location = settings.Location;
		}

	    public KinectData Measure()
	    {
	        var tmpLocation = Location;
	        var result = new KinectData(_settings.VerticalResolution, _settings.HorisontalResolution);
	        var horisontalAngle = -_settings.HorisontalViewAngle/2.0;
	        var verticalAngle = -_settings.VerticalViewAngle/2.0;
            for (int i = 0; i < _settings.VerticalResolution; i++)
            {
                Frame3D mediateDirection = SensorRotation.VerticalFrameRotation(tmpLocation, verticalAngle);
                horisontalAngle = -_settings.HorisontalViewAngle / 2.0;
                for (int j = 0; j < _settings.HorisontalResolution; j++)
                {
                    Frame3D direction = SensorRotation.HorisontalFrameRotation(mediateDirection, horisontalAngle);
	                Ray ray = new Ray(tmpLocation.ToPoint3D(), SensorRotation.GetFrontDirection(direction));
	                //Console.WriteLine("Ray: " + ray);
	                var dist = double.PositiveInfinity;
	                foreach (var body in _worldRoot.GetSubtreeChildrenFirst())
	                    if(_settings.Exclude.All(a => a != body))
	                    {
	                        var inter = Intersector.Intersect(body, ray);
	                        dist = Math.Min(dist, inter);
	                    }
                    result.Depth[i, j] = dist;
	                
                    //verticalAngle += _settings.VStep;
                    horisontalAngle += _settings.HStep;
	            }
                verticalAngle += _settings.VStep;
                //horisontalAngle += _settings.HStep;
	    }
	return result;
		}
	}

    public class KinectData
	{
	    public double[,] Depth;
        public KinectData(int i, int j)
        {
            Depth = new double[i,j];
        }
	}

    public class KinectSettings
	{
        public Frame3D Location { get; private set; }
        public Angle HorisontalViewAngle { get; private set; }
        public Angle VerticalViewAngle { get; private set; }
        public int HorisontalResolution { get; private set; }
        public int VerticalResolution { get; private set; }
        public Angle HStep { get; private set; }
        public Angle VStep { get; private set; }
        public List<Body> Exclude { get; set; }
        public KinectSettings(Frame3D location, Angle horisontalViewAngle, Angle verticalViewAngle,  int horisontalResolution, int verticalResolution)
        {
            Location = location; 
            HorisontalViewAngle = horisontalViewAngle;
            VerticalViewAngle = verticalViewAngle;
            HorisontalResolution = horisontalResolution;
            VerticalResolution = verticalResolution;
            HStep = HorisontalViewAngle/HorisontalResolution;
            VStep = VerticalViewAngle/VerticalResolution;
            Exclude = new List<Body>();
        }
	}
    [TestFixture]
    internal class KinectTest
    {
        //Кинект
        private static readonly Frame3D Location = new Frame3D(-10, 0, 3);
        private static readonly Angle HorisontalViewAngle = Angle.FromRad(Math.PI);
        private static readonly Angle VerticalViewAngle = Angle.FromRad(Math.PI);
        private const int HorisontalResolution = 180;
        private const int VerticalResolution = 180;
        //Шар
        private static readonly Frame3D BallPosition = new Frame3D(0, 0, 0);
        private static readonly Ball Ball = new Ball { Location = BallPosition, Radius = 3 };
        //Цилиндр
        private static readonly Frame3D CylinderPosition = new Frame3D(0, 0, 0);
        private static readonly Cylinder Cylinder = new Cylinder
        {
            Location = CylinderPosition,
            RBottom = 3,
            RTop = 3,
            Height = 6
        };
        //Бокс
        private static readonly Frame3D BoxPosition = new Frame3D(0, 0, 0, Angle.FromRad(0), Angle.FromRad(Math.PI / 4.0), Angle.FromRad(0));
        private static readonly Box Box = new Box { Location = BoxPosition, XSize = 6.0, YSize = 6.0, ZSize = 6.0 };

        private readonly object[][] _data = new[]
            {
                new object[]
                    {
                        new KinectSettings(Location, HorisontalViewAngle, VerticalViewAngle, HorisontalResolution, VerticalResolution),
                        Ball
                    },
                    new object[]
                    {
                        new KinectSettings(Location, HorisontalViewAngle, VerticalViewAngle, HorisontalResolution, VerticalResolution),
                        Cylinder
                    },
                    new object[]
                    {
                        new KinectSettings(Location, HorisontalViewAngle, VerticalViewAngle, HorisontalResolution, VerticalResolution),
                        Box
                    }
            };


        [Test]
        [TestCaseSource("_data")]
        public void RayTest(KinectSettings settings, Body body)
        {
            Kinect kinect = new Kinect(body, settings);
            double[,] measure = kinect.Measure().Depth;
//            for (int i = 0; i < settings.VerticalResolution; i++)
//                for (int j = 0; j < settings.HorisontalResolution; j++)
//                {
//                    Console.WriteLine(measure[i,j]);
//                }

            Bitmap depthMap = new Bitmap(settings.HorisontalResolution, settings.VerticalResolution);
            Color color = Color.Black;
            for (int i = 0; i < settings.VerticalResolution; i++)
                for (int j = 0; j < settings.HorisontalResolution; j++)
            {
                if (!double.IsPositiveInfinity(measure[i,j]))
                    depthMap.SetPixel(j, i, color);
            }
            if (body is Ball)
                depthMap.Save(@"C:\Users\Anton\ballkinect.bmp");
            if (body is Cylinder)
                depthMap.Save(@"C:\Users\Anton\cylinderkinect.bmp");
            if (body is Box)
                depthMap.Save(@"C:\Users\Anton\boxkinect.bmp");
            else depthMap.Save(@"C:\Users\Anton\pickinect.bmp");
        }
    }
}
