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
	class SimpleSonar:ISensor<List<double>>
	{
		public SimpleSonar(Body worldRoot, SimpleSonarSettings settings)
		{
			_worldRoot = worldRoot;
			_settings = settings;
		}
        
		public List<double> Measure()
		{
		    var result = new List<double>();
		    var angle = -_settings.ViewAngle/2.0;
            for (int i = 0; i < _settings.Resolution; i++)
            {
                Ray ray = new Ray(_settings.Location.ToPoint3D(), SensorRotation.HorisontalRotation(_settings.Location, angle));
                Console.WriteLine("Ray: " + ray);
                //здесь мы перебираем все объекты, которые есть в мире
                var dist = double.PositiveInfinity;
                foreach (var body in _worldRoot.GetSubtreeChildrenFirst())
                    dist = Math.Min(dist, Intersector.Intersect(body, ray));
                result.Add(dist);
                angle += _settings.Step;
            }
                
			return result;
		}
        
		private readonly Body _worldRoot;
		private readonly SimpleSonarSettings _settings;
	}


    internal class SimpleSonarSettings
	{
        public Frame3D Location { get; private set; }
        public Angle ViewAngle { get; private set; }
        public int Resolution { get; private set; }
        public Angle Step { get; private set; }

        public SimpleSonarSettings(Frame3D location, Angle viewAngle, int resolution)
        {
            Location = location;
            ViewAngle = viewAngle;
            Resolution = resolution;
            Step = ViewAngle/Resolution;
        }
	}
    
    [TestFixture]
    internal class SimpleSonarTest
    {
        //Сонар
        private static readonly Frame3D Location = new Frame3D(-2, 0, 0.5);
        private static readonly Angle ViewAngle = Angle.FromRad(Math.PI);
        private const int Resolution = 180;
        //Шар
        private static readonly Frame3D BallPosition = new Frame3D(0, 0, 0);
        private static readonly Ball Ball = new Ball { Location = BallPosition, Radius = 1};
        //Цилиндр
        private static readonly Frame3D CylinderPosition = new Frame3D(0, 0, 0);
        private static readonly Cylinder Cylinder = new Cylinder
            {
                Location = CylinderPosition,
                RBottom = 1.5,
                RTop = 1.5,
                Height = 3
            };
        //Бокс
        private static readonly Frame3D BoxPosition = new Frame3D(0, 0, 0, Angle.FromRad(0), Angle.FromRad(Math.PI/4.0), Angle.FromRad(0));
        private static readonly Box Box = new Box { Location = BoxPosition, XSize = 1, YSize = 1, ZSize = 1 };

        private readonly object[][] _data = new []
            {
                new object[]
                    {
                        new SimpleSonarSettings(Location, ViewAngle, Resolution),
                        Ball
                    },
                    new object[]
                    {
                        new SimpleSonarSettings(Location, ViewAngle, Resolution),
                        Cylinder
                    },
                    new object[]
                    {
                        new SimpleSonarSettings(Location, ViewAngle, Resolution),
                        Box
                    }
            };


        [Test]
        [TestCaseSource("_data")]
        public void RayTest(SimpleSonarSettings settings, Body body)
        {
            SimpleSonar sonar = new SimpleSonar(body, settings);
            List<double> measure = sonar.Measure();
            measure/*.Where(a => a < double.PositiveInfinity)*/.ToList().ForEach(Console.WriteLine);

            int height = (int)(100 * measure.Where(a => a < double.PositiveInfinity).Max());
            
            Bitmap depthMap = new Bitmap(Resolution, 2 * height);
            Color color = Color.Black;
            for (int i = 0; i < depthMap.Width; i++)
            {
                if (!double.IsPositiveInfinity(measure[i]))
                    
                    depthMap.SetPixel(i, (int)(100* measure[i]), color);
                
            }
            if (body is Ball)
                depthMap.Save(@"C:\Users\Anton\ball.bmp");
            if (body is Cylinder)
                depthMap.Save(@"C:\Users\Anton\cylinder.bmp");
            if (body is Box)
                depthMap.Save(@"C:\Users\Anton\box.bmp");
            else depthMap.Save(@"C:\Users\Anton\pic.bmp");

        }
    }
}
