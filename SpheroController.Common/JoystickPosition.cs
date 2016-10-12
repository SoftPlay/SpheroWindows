using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpheroController.Common
{
	public class JoystickPosition
	{
		public JoystickPosition(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public double Angle
		{
			get
			{
				var angle = Math.Atan2(this.X, this.Y) * 180 / Math.PI;

				if (angle < 0)
				{
					angle = Math.Abs(angle) + 180;
				}

				return angle;
			}
		}

		public double Distance
		{
			get
			{
				return Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
			}
		}

		public double X { get; }

		public double Y { get; }
	}
}
