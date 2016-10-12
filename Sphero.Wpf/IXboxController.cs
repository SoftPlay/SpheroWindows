using System;

namespace SpheroController.Wpf
{
	public interface IXboxController
	{
		event EventHandler<GamepadReadingEventArgs> ReadingChanged;
	}
}