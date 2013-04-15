using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace GemsHunt.Library
{
	//TODO(для Serj) Robot не должен быть наследником от Body!
	[Serializable]
	public class Robot2013 : Box
	{
		private readonly List<String> _commands = new List<string>();
		private readonly List<Body> _gripped = new List<Body>();
        private readonly List<Body> _lifted = new List<Body>();
		private readonly Body _worldRoot;

		public Robot2013(Body worldRoot)
        {
	        _worldRoot = worldRoot;
	        Name = "Robot";
        }

		public void AddCommand(string command)
		{
			_commands.Add(command);
		}

		public void WorkAi()
		{
			ApplyCommands();
			//measure sensors?
		}

		private void ApplyCommands()
		{
			foreach(string command in _commands)
			{
				switch(command)
				{
					case "grip":
						Grip();
						break;
					case "release":
						Release();
						break;
				}
			}
			_commands.Clear();
		}


        public void Move(double distance, double angle)
        {
            this.Velocity = new Frame3D(distance * Math.Cos(this.Location.Yaw.Radian), distance * Math.Sin(this.Location.Yaw.Radian), 0, Angle.Zero, Angle.FromGrad(angle), Angle.Zero);          
        }

		private void Grip()
		{

            Body found = _worldRoot.FirstOrDefault(CanBeAttached);
            if (found != null)
            {
                Body latestGripped = null;
                int oldGrippedCount = _gripped.Count;

                if (oldGrippedCount > 0)
                {
                    latestGripped = _gripped.Last();
                    if (found.DefaultColor != latestGripped.DefaultColor)
                    {
                        return;
                    }
                }
                
                Body tempfound = found;

                while (tempfound.Count() > 0)
                {
                    tempfound = tempfound.FirstOrDefault();
                    _gripped.Add(tempfound);
                }

                if (oldGrippedCount > 0)
                {
                    Remove(latestGripped);
                    latestGripped.Location = new Frame3D(0, 0, 8, Angle.Zero, Angle.Zero, Angle.Zero);
                    tempfound.Add(latestGripped);
                }

                CloseDistance = 30;

                CaptureDevicet(this, found);
                _gripped.Add(found);
            }


			/*Body found = _worldRoot.FirstOrDefault(CanBeAttached);
		    if(found == null) return;
            if (_gripped.Count > 0)
                if(_gripped.First().Name != found.Name)
                    return;
		    if(_gripped.Count == 0)
		    {
		        CaptureDevicet(this, found);
		        _gripped.Add(found);
		        if(found.Any())
		            _gripped.Add(found.FirstOrDefault());
		        CloseDistance = 30;
		    } else {
                Body latestGripped = _gripped.First();
		        found.Density = Density.None;
                if (found.Parent != null)
                    found.Parent.Remove(found);
                found.Location = new Frame3D(0,0, _gripped.OfType<Box>().Sum(a => a.ZSize));
                latestGripped.Add(found);
                _gripped.Add(found);
			}
            */
            

		}

		private bool CanBeAttached(Body body)
		{
			return body != this &&
				!body.IsStatic &&
				!SubtreeContainsChild(body) &&
				!ParentsContain(body) &&
				IsCloseEnough(body);
		}

		private void Release()
		{
            if (_gripped.Count > 0)
            {
                Body latestGripped = _gripped.Last();

                Frame3D absoluteLocation = latestGripped.GetAbsoluteLocation();
                Remove(latestGripped);

                latestGripped.Location = absoluteLocation;
                latestGripped.Velocity = Velocity;

                _worldRoot.Add(latestGripped);
                _gripped.RemoveRange(0, _gripped.Count);

            }

            CloseDistance = 20;

           /* Console.WriteLine("Сколько было" + _gripped.Count.ToString());
		    if(_gripped.Count == 0) return;
		    var root = _gripped.First();
		    var l = root.GetAbsoluteLocation();
            Remove(root);
		    root.Location = l;
            _worldRoot.Add(root);
            _gripped.Clear();
            CloseDistance = 20;
            Console.WriteLine("Сколько стало" + _gripped.Count.ToString());
			*/
		}
		/// <summary>
		/// Подсоединяет объект к Box спереди
		/// </summary>
		public static void CaptureDevicet(Box box, Body newChild)
		{
			Frame3D childAbsolute = newChild.GetAbsoluteLocation();
			if (newChild.Parent != null)
			{
				newChild.Parent.Remove(newChild);
			}
			newChild.Location = box.GetAbsoluteLocation().Invert().Apply(childAbsolute);
			newChild.Location = newChild.Location.NewYaw(Angle.Zero);
			newChild.Location = newChild.Location.NewX(14);
			newChild.Location = newChild.Location.NewY(0);

			box.Add(newChild);
		}
        
        private double CloseDistance = 20;

        
		private bool IsCloseEnough(Body body)
		{
            //Console.WriteLine((Angem.Hypot(body.GetAbsoluteLocation() - GetAbsoluteLocation())));
			return (Angem.Hypot(body.GetAbsoluteLocation() - GetAbsoluteLocation()) < CloseDistance);
		}
	}
}
