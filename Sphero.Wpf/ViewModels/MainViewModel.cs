using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using RobotKit;
using SpheroController.Wpf.Robots;
using Windows.UI.Popups;

namespace SpheroController.Wpf.ViewModels
{
	public class MainViewModel : BindableBase
	{
		private readonly SynchronizationContext context;
		private double rollAngle;
		private double rollDistance;

		public MainViewModel()
		{
			this.context = SynchronizationContext.Current ?? new SynchronizationContext();

			RobotProvider provider = RobotProvider.GetSharedProvider();
			provider.DiscoveredRobotEvent += this.Provider_DiscoveredRobotEvent; ;
			provider.NoRobotsEvent += this.Provider_NoRobotsEvent; ;
			provider.ConnectedRobotEvent += Provider_ConnectedRobotEvent; ;
			provider.FindRobots();
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

		private void Provider_DiscoveredRobotEvent(object sender, Robot e)
		{
			if (e is Sphero)
			{
				// Discovered a sphero. Now connect to it.
				var provider = RobotProvider.GetSharedProvider();
				provider.ConnectRobot(e);
			}
			else
			{
				Debug.WriteLine(string.Format("Found some other kind of Robot: {0}", e.GetType()));
			}
		}

		private void Provider_ConnectedRobotEvent(object sender, Robot e)
		{
			if (e is Sphero)
			{
				var sphero = new SpheroWrapper(e as Sphero);
				var viewModel = new SpheroViewModel(sphero);

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
	}
}
