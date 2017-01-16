using Windows.UI;

namespace SpheroController.Common.Interfaces
{
	public interface ISpheroViewModel
	{
		Color Color { get; set; }
		string Name { get; }
		double RollAngle { get; set; }
		double RollDistance { get; set; }
		double RotateAngle { get; set; }
		bool TailLight { get; set; }
	}
}