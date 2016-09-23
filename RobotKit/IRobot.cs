using RobotKit;
using RobotKit.Internal;

namespace RobotKit
{
	public interface IRobot
	{
		string BluetoothName { get; }
		CollisionControl CollisionControl { get; }
		ConnectionState ConnectionState { get; }
		string Name { get; }
		SensorControl SensorControl { get; }

		void Disconnect();
		void Sleep(int wakeup, int orbBasicLineNum = 0);
		void Sleep(int wakeup, byte macro = 0);
		void Sleep(int wakeup = 0, byte macro = 0, int orbBasic = 0);
		string ToString();
		void WriteName(string name);
		void WriteToRobot(DeviceMessage msg);
	}
}