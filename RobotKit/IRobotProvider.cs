using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RobotKit
{
	public interface IRobotProvider
	{
		event EventHandler<RobotEventArgs> ConnectedRobotEvent;
		event EventHandler<RobotEventArgs> DiscoveredRobotEvent;
		event EventHandler NoRobotsEvent;

		Task<bool> ConnectRobot(IRobot robot);
		void DisconnectAll();
		Task<IReadOnlyCollection<IRobot>> FindRobots();
		IRobot GetConnectedRobot();
		List<IRobot> GetConnectedRobots();
	}
}