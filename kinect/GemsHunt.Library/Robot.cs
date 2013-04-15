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

		private void Grip()
		{
			Body found = _worldRoot.FirstOrDefault(CanBeAttached);
			if (found != null)
			{
				DetachAttachMaintaingLoction(found);
				_gripped.Add(found);
			}
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
			if (_gripped.Count == 0)
				return;
			Body latestGripped = _gripped.Last();
			_gripped.RemoveAt(_gripped.Count - 1);
			Frame3D absoluteLocation = latestGripped.GetAbsoluteLocation();
			Remove(latestGripped);
			latestGripped.Location = absoluteLocation;
			latestGripped.Velocity = Velocity;
			_worldRoot.Add(latestGripped);
		}

		private const double CloseDistance = 20;

		private bool IsCloseEnough(Body body)
		{
			return (Angem.Hypot(body.GetAbsoluteLocation() - GetAbsoluteLocation()) < CloseDistance);
		}
	}
}
