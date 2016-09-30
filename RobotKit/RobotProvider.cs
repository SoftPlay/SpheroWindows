using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Proximity;

namespace RobotKit
{
	public class RobotProvider : IRobotProvider
	{
		private static RobotProvider _sharedProvider;

		private List<IRobot> pairedRobots = new List<IRobot>();

		private List<IRobot> connectedRobots = new List<IRobot>();

		static RobotProvider()
		{
			RobotProvider._sharedProvider = null;
		}

		private RobotProvider()
		{
			PeerFinder.AllowBluetooth = true;
			PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";
		}

		public async Task<bool> ConnectRobot(IRobot robot)
		{
			bool connected = await (robot as Robot).Connect();

			if (connected)
			{
				ConnectedRobotEvent?.Invoke(this, new RobotEventArgs(robot));
				connectedRobots.Add(robot);
			}

			return connected;
		}

		public void DisconnectAll()
		{
			foreach (Robot connectedRobot in connectedRobots)
			{
				connectedRobot.Disconnect();
			}
		}

		public async Task<IReadOnlyCollection<IRobot>> FindRobots()
		{
			var foundRobots = new List<IRobot>();
			try
			{
				var deviceInformationCollection = 
					await DeviceInformation.FindAllAsync(
						RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(new Guid("00001101-0000-1000-8000-00805F9B34FB"))));

				if (!deviceInformationCollection.Any())
				{
					NoRobotsEvent?.Invoke(this, EventArgs.Empty);
				}

				foreach(var item in deviceInformationCollection)
				{
					if (!item.Name.Contains("Sphero"))
					{
						Debug.WriteLine("There needs to be a permission in the app manifest.");
						Debug.WriteLine("Add UUID of Sphero to manifest: 00001101-0000-1000-8000-00805F9B34FB");
					}
					else
					{
						var spheroService = await RfcommDeviceService.FromIdAsync(item.Id);

						Sphero sphero = new Sphero(spheroService);

						foundRobots.Add(sphero);

						DiscoveredRobotEvent?.Invoke(this, new RobotEventArgs(sphero));
					}
				}
			}
			catch (Exception exception)
			{
				if ((uint)exception.HResult == 0x8007048F)
				{
					throw new InvalidOperationException("Bluetooth is currently switched off", exception);
				}
				else
				{
					throw;
				}
			}

			return foundRobots.AsReadOnly();
		}

		public IRobot GetConnectedRobot()
		{
			return connectedRobots.First();
		}

		public List<IRobot> GetConnectedRobots()
		{
			return connectedRobots.ToList();
		}

		public static RobotProvider GetSharedProvider()
		{
			if (RobotProvider._sharedProvider == null)
			{
				RobotProvider._sharedProvider = new RobotProvider();
			}
			return RobotProvider._sharedProvider;
		}

		public event EventHandler<RobotEventArgs> ConnectedRobotEvent;

		public event EventHandler<RobotEventArgs> DiscoveredRobotEvent;

		public event EventHandler NoRobotsEvent;
	}
}