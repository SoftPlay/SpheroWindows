using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using SpheroController.Wpf.Robots;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SpheroController.Wpf
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : PrismUnityApplication
	{
		protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
		{
			// Nagivate to MainPage.xaml
			NavigationService.Navigate("Main", null);

			Window.Current.Activate();

			return Task.FromResult<object>(null);
		}

		protected override Task OnInitializeAsync(IActivatedEventArgs args)
		{
			RegisterTypes();
			return base.OnInitializeAsync(args);
		}

		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

			this.Container.RegisterType<IRobotProvider, RobotProviderWrapper>(new ContainerControlledLifetimeManager());
		}

		private void RegisterTypes()
		{
			// Tell the ViewModelLocationProvider where to find the ViewModel for a view (using naming convention)
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "SpheroController.Wpf.ViewModels.{0}ViewModel, SpheroController.Wpf", viewType.Name);
				var viewModelType = Type.GetType(viewModelTypeName);
				return viewModelType;
			});
		}
	}
}
