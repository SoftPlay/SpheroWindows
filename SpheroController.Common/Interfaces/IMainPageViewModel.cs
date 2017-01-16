using System.Collections.ObjectModel;
using Windows.UI;

namespace SpheroController.Common.Interfaces
{
	public interface IMainPageViewModel
	{
		Color Colour { get; set; }
		ObservableCollection<string> DebugItemCollection { get; }
		double RollAngle { get; set; }
		double RollDistance { get; set; }
		int Count { get; }
		ObservableCollection<ISpheroViewModel> SpheroViewModelCollection { get; }
	}
}