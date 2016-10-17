using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using Prism.Mvvm;
using Prism.Windows.Navigation;
using RobotKit;
using SpheroController.Common;
using Windows.Gaming.Input;
using Windows.UI;
using Windows.UI.Popups;

namespace SpheroController.Wpf.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
		private readonly IRobotProvider robotProvider;
		private double rollAngle;
		private double rollDistance;
		private readonly IXboxController xboxController;
		private Color colour;

		public MainPageViewModel(IRobotProvider robotProvider, IXboxController xboxController)
		{
			this.robotProvider = robotProvider;
			this.xboxController = xboxController;
			this.xboxController.ReadingChanged += XboxController_ReadingChanged;
		}

		public ObservableCollection<SpheroViewModel> SpheroViewModelCollection { get; } = new ObservableCollection<SpheroViewModel>();

		public ObservableCollection<string> DebugItemCollection { get; } = new ObservableCollection<string>();

		public double RollAngle
		{
			get
			{
				return this.rollAngle;
			}

			set
			{
				if (this.SetProperty(ref this.rollAngle, value))
				{
					foreach (var sphero in this.SpheroViewModelCollection)
					{
						sphero.RollAngle = value;
					}
				}
			}
		}

		public double RollDistance
		{
			get
			{
				return this.rollDistance;
			}

			set
			{
				if (this.SetProperty(ref this.rollDistance, value))
				{
					foreach (var sphero in this.SpheroViewModelCollection)
					{
						sphero.RollDistance = value;
					}
				}
			}
		}

		public async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			var robots = await this.robotProvider.FindRobots();

			if (robots.Count == 0)
			{
				this.DebugItemCollection.Add("Didn't find any Spheros :-(");
			}
			else
			{
				foreach (var robot in robots)
				{
					if (robot is ISphero)
					{
						// Discovered a sphero. Now connect to it.
						this.DebugItemCollection.Add($"Connecting to Sphero {robot.BluetoothName}");

						var connected = await this.robotProvider.ConnectRobot(robot);

						if (connected)
						{
							this.DebugItemCollection.Add($"Connected to Sphero {robot.BluetoothName}!");

							var viewModel = new SpheroViewModel(robot as ISphero);

							this.SpheroViewModelCollection.Add(viewModel);
						}
						else
						{
							this.DebugItemCollection.Add($"Failed to connect to Sphero {robot.BluetoothName}");
						}
					}
					else
					{
						Debug.WriteLine($"Found some other kind of Robot: {robot.GetType()}");
					}
				}
			}
		}

		public void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
		{
		}

		private void XboxController_ReadingChanged(object sender, GamepadReadingEventArgs e)
		{
			if (e.Reading.Buttons.HasFlag(GamepadButtons.Y))
			{
				this.Colour = Colors.Yellow;
			}
			else if (e.Reading.Buttons.HasFlag(GamepadButtons.B))
			{
				this.Colour = Colors.Red;
			}
			else if (e.Reading.Buttons.HasFlag(GamepadButtons.A))
			{
				this.Colour = Colors.Green;
			}
			else if (e.Reading.Buttons.HasFlag(GamepadButtons.X))
			{
				this.Colour = Colors.Blue;
			}
			
			var joystickPosition = new JoystickPosition(e.Reading.LeftThumbstickX, e.Reading.LeftThumbstickY);

			var distance = joystickPosition.Distance * 100;
			if (distance < 10)
			{
				// Ignore small movements of the joystick
				distance = 0;
			}
			else if (distance > 100)
			{
				// 100 is the maximum speed
				distance = 100;
			}

			this.RollDistance = distance;
			this.RollAngle = joystickPosition.Angle;

			this.DebugItemCollection.Add($"LeftX {joystickPosition.X}, LeftY {joystickPosition.Y}. Roll {distance}, Angle {joystickPosition.Angle}");
		}

		public Color Colour
		{
			get
			{
				return this.colour;
			}

			set
			{
				if (this.SetProperty(ref this.colour, value))
				{
					foreach(var sphero in this.SpheroViewModelCollection)
					{
						sphero.Color = value;
					}
				}
			}
		}
	}
}
