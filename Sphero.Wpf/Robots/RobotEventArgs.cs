using System;

namespace SpheroController.Wpf.Robots
{
	public class RobotEventArgs : EventArgs
	{
		public RobotEventArgs(IRobot robot)
		{
			this.Robot = robot;
		}

		public IRobot Robot { get; private set; }
	}
}
