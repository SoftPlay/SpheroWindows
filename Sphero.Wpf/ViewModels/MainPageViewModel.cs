using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Prism.Mvvm;
using Prism.Windows.Navigation;
using RobotKit;
using Windows.UI.Popups;

namespace SpheroController.Wpf.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
		private readonly IRobotProvider robotProvider;
		private double rollAngle;
		private double rollDistance;
		

		public MainPageViewModel(IRobotProvider robotProvider)
		{
			this.robotProvider = robotProvider;
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
	}
}
