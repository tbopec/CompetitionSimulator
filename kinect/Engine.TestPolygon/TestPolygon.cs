using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Core.Physics;
using Eurosim.Core.Physics.BepuWrap;
using Eurosim.Core.Physics.FarseerWrap;
using Eurosim.Graphics;
using Eurosim.Physics;

namespace Eurosim.Engine.TestPolygon
{
	public class TestPolygon : Body
	{
		public Body MainBody;

		private Body _root=new Body();
		public readonly double DT = 1.0 / 60;
		public readonly int PhysicalPrecision = 10;		

		public TestPolygonSettings Settings;
		

		public void MakeCycle(bool realtime)
		{

			var begin = DateTime.Now;
			if (Settings.PhysicsMode != PhysicalEngines.No)
			{
				// Посчитаем подходящий для физического мира dt
				var physicalDT = DT / PhysicalPrecision;
				for (int i = 0; i < PhysicalPrecision; i++)
				{
					PhysicalManager.MakeIteration(physicalDT, _root);
				}
			}

			foreach (var body in Nested)
			{
				body.Update(DT);
			}
			
			var elapsed = 1000 * DT - (DateTime.Now - begin).TotalMilliseconds;
			if (realtime && elapsed > 0)
				Thread.Sleep((int)elapsed);
		}

		public TestPolygon(TestPolygonSettings settings)
		{
			Settings = settings;
			switch(Settings.PhysicsMode)
			{
				case PhysicalEngines.Bepu:
					PhysicalManager.InitializeEngine(PhysicalEngines.Bepu, new BepuWorld(), _root);
					break;
				case PhysicalEngines.Farseer:
				case PhysicalEngines.No:
					PhysicalManager.InitializeEngine(PhysicalEngines.Farseer, new FarseerWorld(), _root);
					break;
			}

			Body floor = new Box(10, 20, 20, Color.Aquamarine)
			{
				Location = new Frame3D(0, 0, -50)
			};	

			floor.Velocity = new Frame3D(0,0,100);
//
//			Add(floor);
//			PhysicalPrimitiveBody firstPrim = new PhysicalPrimitiveBody(new BoxShape(20, 20, 50), Color.Cyan);
//			firstPrim.Location = new Frame3D(20, 0, 0, Angle.FromGrad(0), Angle.FromGrad(70), Angle.Zero);
//			firstPrim.PhysicalModel.FrictionCoefficient = 1;
//			firstPrim.Name = "firstPrim";
//			PhysicalPrimitiveBody hand = new PhysicalPrimitiveBody(new BoxShape(50, 10, 10), Color.Crimson);
//			hand.Location = new Frame3D(25, 0, 30);
//			hand.Name = "hand";
//
//			firstPrim.Add(hand);
//			firstPrim.Mass = 50;
//
//			PrimitiveBody dummy = new PrimitiveBody(new BallShape(5), Color.Bisque);
//			dummy.Location = new Frame3D(0, 0, 20);
//			floor.Velocity = new Frame3D(1, 0, 0);
//			hand.Add(dummy);
//
//			PhysicalPrimitiveBody finger = new PhysicalPrimitiveBody(new BoxShape(50, 10, 10), Color.BurlyWood);
//			finger.Location = new Frame3D(60, 0, 0);
//			dummy.Add(finger);
//
//			Add(firstPrim);
//
//			firstPrim.Remove(hand);
//			Add(hand);
//			hand.Location -= new Frame3D(20, 0, 0);
//
//			MainBody = new PhysicalPrimitiveBody(new BoxShape(20, 20, 60), Color.Blue) {Location = new Frame3D(-30, 65, 0)};
//			MainBody.FrictionCoefficient = 0.5;
//			MainBody.Mass = 50;
//			Add(MainBody);

//			firstPrim.Collision += (body, primitiveBody) =>
//				{
//					firstPrim.Remove(hand); 
//					Add(hand);
//				};
		}

		public void CreateDrawer()
		{
			if (Settings.Drawers.Count == 0)
				Settings.Drawers.Add(new DrawerSettings());
			Func<DrawerSettings, Form> formFactory = 
				FormDrawer.CreateDefaultEmptyForm;
			var drawerFactory = new DrawerFactory(_root);
			Drawers.AddRange(drawerFactory.CreateForSettingsList(Settings.VideoMode,
				Settings.Drawers, this, formFactory));
			Drawers.ForEach(x => x.Run());
		}
		public List<FormDrawer> Drawers=new List<FormDrawer>();
	}
}
