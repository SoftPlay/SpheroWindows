using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;

namespace SpheroController.Wpf
{
	public class GamepadReadingEventArgs : EventArgs
	{
		public GamepadReadingEventArgs(GamepadReading reading)
		{
			this.Reading = reading;
		}

		public GamepadReading Reading { get; private set; }
	}
}
