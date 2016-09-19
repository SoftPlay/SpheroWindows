using System;
using System.Collections.Generic;

namespace SpheroController.Wpf.Robots
{
	public interface IRobotProvider
	{
		event EventHandler<RobotEventArgs> ConnectedRobotEvent;
		event EventHandler<RobotEventArgs> DiscoveredRobotEvent;
		event EventHandler NoRobotsEvent;

		void ConnectRobot(IRobot robot);
		void DisconnectAll();
		void FindRobots();
		IRobot GetConnectedRobot();
		List<IRobot> GetConnectedRobots();
	}
}