using System;
using System.Collections.Generic;
using System.Linq;
using RobotKit;
using RobotKit.Internal;

namespace SpheroController.Wpf.Robots
{
	public class RobotWrapper : IRobot
	{
		private readonly RobotKit.Robot robot;

		public RobotWrapper(RobotKit.Robot robot)
		{
			this.robot = robot;
		}

		public Robot RawObject
		{
			get
			{
				return this.robot;
			}
		}

		public string BluetoothName
		{
			get
			{
				return this.robot.BluetoothName;
			}
		}

		public CollisionControl CollisionControl
		{
			get
			{
				return this.robot.CollisionControl;
			}
		}

		public ConnectionState ConnectionState
		{
			get
			{
				return this.robot.ConnectionState;
			}
		}

		public string Name
		{
			get
			{
				return this.robot.Name;
			}
		}

		public SensorControl SensorControl
		{
			get
			{
				return this.SensorControl;
			}
		}

		public void Disconnect()
		{
			this.robot.Disconnect();
		}

		public void Sleep(int wakeup, byte macro = 0)
		{
			this.robot.Sleep(wakeup, macro);
		}

		public void Sleep(int wakeup, int orbBasicLineNum = 0)
		{
			this.robot.Sleep(wakeup, orbBasicLineNum);
		}

		public void Sleep(int wakeup = 0, byte macro = 0, int orbBasic = 0)
		{
			this.robot.Sleep(wakeup, macro, orbBasic);
		}

		public override string ToString()
		{
			return this.robot.ToString();
		}

		public void WriteName(string name)
		{
			this.robot.WriteName(name);
		}

		public void WriteToRobot(DeviceMessage msg)
		{
			this.robot.WriteToRobot(msg);
		}
	}
}
