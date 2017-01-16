using System;
using System.Globalization;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using RobotKit;
using SpheroController.Common.Controllers;
using SpheroController.Common.Interfaces;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SpheroController.Wpf
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	[Bindable]
	sealed partial class App : PrismUnityApplication
	{
		protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
		{
			// Nagivate to MainPage.xaml
			NavigationService.Navigate("Main", null);

			Window.Current.Activate();

			return Task.FromResult<object>(null);
		}

		protected override async Task OnInitializeAsync(IActivatedEventArgs args)
		{
			RegisterTypes();
			ResolveSingletons();
			await base.OnInitializeAsync(args);
		}

		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

			this.Container.RegisterType<IXboxController, XboxController>();

			this.Container.RegisterInstance<IAmazonSQS>(new AmazonSQSClient("AKIAJAPKYGXLJEIHI4CA", "-- REDACTED --", RegionEndpoint.EUWest1));
			this.Container.RegisterType<ISqsController, SqsController>();

			this.Container.RegisterInstance<IRobotProvider>(RobotProvider.GetSharedProvider());

			// A little unusual, but until I refactor things I'm going to inject the singleton ViewModel into the view and controllers
			this.Container.RegisterType<IMainPageViewModel, ViewModels.MainPageViewModel>(new ContainerControlledLifetimeManager());
		}

		private void RegisterTypes()
		{
			// Tell the ViewModelLocationProvider where to find the ViewModel for a view (using naming convention)
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "SpheroController.Common.Interfaces.I{0}ViewModel, SpheroController.Common", viewType.Name);
				var viewModelType = Type.GetType(viewModelTypeName);
				return viewModelType;
			});
		}

		private void ResolveSingletons()
		{
			var sqsController = this.Container.Resolve<ISqsController>();
			sqsController.StartAsync();
		}
	}
}
