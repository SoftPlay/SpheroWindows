using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotKit;

namespace SpheroController.Wpf.Robots
{
	public class SpheroWrapper : RobotWrapper, ISphero
	{
		private readonly RobotKit.Sphero sphero;

		public SpheroWrapper(RobotKit.Sphero sphero)
			: base(sphero)
		{
			this.sphero = sphero;
		}


		public void Roll(int heading, float speed)
		{
			this.sphero.Roll(heading, speed);
		}

		public void SetBackLED(float intensity)
		{
			this.sphero.SetBackLED(intensity);
		}

		public void SetHeading(int heading)
		{
			this.sphero.SetHeading(heading);
		}

		public void SetRGBLED(int red, int green, int blue)
		{
			this.sphero.SetRGBLED(red, green, blue);
		}
	}
}
