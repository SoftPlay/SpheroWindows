﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;

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

		public async Task ConnectRobot(IRobot robot)
		{
			if (!await (robot as Robot).Connect())
			{
				await toastFailConnect(robot.BluetoothName);
			}
			else
			{
				ConnectedRobotEvent?.Invoke(this, new RobotEventArgs(robot));
				connectedRobots.Add(robot);
				await toastConnect(robot.BluetoothName);
			}
		}

		public void DisconnectAll()
		{
			foreach (Robot connectedRobot in connectedRobots)
			{
				connectedRobot.Disconnect();
			}
		}

		public async Task FindRobots()
		{
			try
			{
				var deviceInformationCollection = 
					await DeviceInformation.FindAllAsync(
						RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(new Guid("00001101-0000-1000-8000-00805F9B34FB"))));

				if (!deviceInformationCollection.Any())
				{
					EventHandler noRobotsEvent = NoRobotsEvent;
					if (noRobotsEvent != null)
					{
						noRobotsEvent.Invoke(this, null);
					}
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
						
						DiscoveredRobotEvent?.Invoke(this, new RobotEventArgs(sphero));
					}
				}
			}
			catch (Exception exception)
			{

				if ((uint)exception.HResult == 0x8007048F)
				{
					MessageDialog dialog = new MessageDialog("Bluetooth is currently switched off");
					dialog.DefaultCommandIndex = 0;
					dialog.CancelCommandIndex = 1;
					await dialog.ShowAsync();
				}

				Debug.WriteLine(String.Concat("Rfcomm Serial Service failed: ", exception));
			}
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
		

		private async Task toastConnect(String name)
		{
			MessageDialog dialog = new MessageDialog(String.Concat("Connected ", name), "Let's Go Ballin'!");
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;
			await dialog.ShowAsync();
		}

		private async Task toastFailConnect(String name)
		{
			MessageDialog dialog = new MessageDialog(String.Concat("Failed to Connect ", name), "Booooo!");
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;
			await dialog.ShowAsync();
		}

		public event EventHandler<RobotEventArgs> ConnectedRobotEvent;

		public event EventHandler<RobotEventArgs> DiscoveredRobotEvent;

		public event EventHandler NoRobotsEvent;
	}
}