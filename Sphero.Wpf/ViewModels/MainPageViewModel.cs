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
		private readonly SynchronizationContext context;
		private readonly IRobotProvider robotProvider;
		private double rollAngle;
		private double rollDistance;
		

		public MainPageViewModel(IRobotProvider robotProvider)
		{
			this.context = SynchronizationContext.Current ?? new SynchronizationContext();

			this.robotProvider = robotProvider;

			this.robotProvider.DiscoveredRobotEvent += this.Provider_DiscoveredRobotEvent; ;
			this.robotProvider.NoRobotsEvent += this.Provider_NoRobotsEvent; ;
			this.robotProvider.ConnectedRobotEvent += Provider_ConnectedRobotEvent; ;
		}

		public ObservableCollection<SpheroViewModel> SpheroViewModelCollection { get; private set; } = new ObservableCollection<SpheroViewModel>();

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

		private void Provider_DiscoveredRobotEvent(object sender, RobotEventArgs e)
		{
			if (e.Robot is ISphero)
			{
				// Discovered a sphero. Now connect to it.
				this.robotProvider.ConnectRobot(e.Robot);
			}
			else
			{
				Debug.WriteLine(string.Format("Found some other kind of Robot: {0}", e.Robot.GetType()));
			}
		}

		private void Provider_ConnectedRobotEvent(object sender, RobotEventArgs e)
		{
			if (e.Robot is ISphero)
			{
				var viewModel = new SpheroViewModel(e.Robot as ISphero);

				this.context.Post((obj) => this.SpheroViewModelCollection.Add(viewModel), null);
			}
		}

		private async void Provider_NoRobotsEvent(object sender, EventArgs e)
		{
			MessageDialog dialog = new MessageDialog("Didn't find Sphero :-(");
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;
			await dialog.ShowAsync();
		}

		public async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			await this.robotProvider.FindRobots();
		}

		public void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
		{
		}
	}
}
