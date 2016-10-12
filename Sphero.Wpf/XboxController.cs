using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI.Xaml;

namespace SpheroController.Wpf
{
	public class XboxController : IXboxController
	{
		private readonly DispatcherTimer dispatcherTimer;
		private GamepadReading previousReading;

		public XboxController()
		{
			dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Start();
		}

		public event EventHandler<GamepadReadingEventArgs> ReadingChanged;

		private void dispatcherTimer_Tick(object sender, object e)
		{
			var controller = Gamepad.Gamepads.FirstOrDefault();

			if (controller != null)
			{	
				var reading = controller.GetCurrentReading();

				if (!GamepadReading.Equals(reading, this.previousReading))
				{
					this.previousReading = reading;

					this.OnReadingChanged(reading);
					/*
					pbLeftThumbstickX.Value = reading.LeftThumbstickX;
					pbLeftThumbstickY.Value = reading.LeftThumbstickY;

					pbRightThumbstickX.Value = reading.RightThumbstickX;
					pbRightThumbstickY.Value = reading.RightThumbstickY;

					pbRightThumbstickY.Value = reading.RightThumbstickY;

					pbLeftTrigger.Value = reading.LeftTrigger;
					pbRightTrigger.Value = reading.RightTrigger;

					//https://msdn.microsoft.com/en-us/library/windows/apps/windows.gaming.input.gamepadbuttons.aspx
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.A), lblA);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.B), lblB);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.X), lblX);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.Y), lblY);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.Menu), lblMenu);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadLeft), lblDPadLeft);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadRight), lblDPadRight);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadUp), lblDPadUp);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.DPadDown), lblDPadDown);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.View), lblView);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.RightThumbstick), ellRightThumbstick);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick), ellLeftThumbstick);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.LeftShoulder), rectLeftShoulder);
					ChangeVisibility(reading.Buttons.HasFlag(GamepadButtons.RightShoulder), recRightShoulder);
					*/
				}

			}


		}

		private void OnReadingChanged(GamepadReading reading)
		{
			this.ReadingChanged?.Invoke(this, new GamepadReadingEventArgs(reading));
		}
	}
}
