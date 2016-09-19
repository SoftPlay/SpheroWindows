using System;
using System.Collections.Generic;
using System.Linq;
using RobotKit;

namespace SpheroController.Wpf.Robots
{
	public class RobotProviderWrapper : IRobotProvider
	{
		public event EventHandler<RobotEventArgs> ConnectedRobotEvent;
		public event EventHandler<RobotEventArgs> DiscoveredRobotEvent;
		public event EventHandler NoRobotsEvent;
		
		public RobotProviderWrapper()
		{
			RobotProvider.GetSharedProvider().ConnectedRobotEvent += (s, e) => this.ConnectedRobotEvent?.Invoke(this, new RobotEventArgs(new SpheroWrapper(e as Sphero)));
			RobotProvider.GetSharedProvider().DiscoveredRobotEvent += (s, e) => this.DiscoveredRobotEvent?.Invoke(this, new RobotEventArgs(new SpheroWrapper(e as Sphero)));
			RobotProvider.GetSharedProvider().NoRobotsEvent += (s, e) => this.NoRobotsEvent?.Invoke(this, EventArgs.Empty);
		}

		public void ConnectRobot(IRobot robot)
		{
			RobotProvider.GetSharedProvider().ConnectRobot(robot.RawObject);
		}

		public void DisconnectAll()
		{
			RobotProvider.GetSharedProvider().DisconnectAll();
		}

		public void FindRobots()
		{
			RobotProvider.GetSharedProvider().FindRobots();
		}

		public IRobot GetConnectedRobot()
		{
			var robot = RobotProvider.GetSharedProvider().GetConnectedRobot();

			if ((robot != null) && (robot is Sphero))
			{
				return new SpheroWrapper(robot as Sphero);
			}

			return null;
		}

		public List<IRobot> GetConnectedRobots()
		{
			var robots = RobotProvider.GetSharedProvider().GetConnectedRobots();

			var wrappedRobots = 
				from robot in robots
				where (robot != null) && (robot is Sphero)
				select new SpheroWrapper(robot as Sphero) as IRobot;

			return wrappedRobots.ToList();
		}
	}
}
