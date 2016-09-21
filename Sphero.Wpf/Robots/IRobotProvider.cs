using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpheroController.Wpf.Robots
{
	public interface IRobotProvider
	{
		event EventHandler<RobotEventArgs> ConnectedRobotEvent;
		event EventHandler<RobotEventArgs> DiscoveredRobotEvent;
		event EventHandler NoRobotsEvent;

		Task ConnectRobot(IRobot robot);
		void DisconnectAll();
		Task FindRobots();
		IRobot GetConnectedRobot();
		List<IRobot> GetConnectedRobots();
	}
}