using System;

namespace SpheroController.Wpf.Controls
{
    public class VirtualJoystickEventArgs:EventArgs
    {
        public double Angle { get; set; }
        public double Distance { get; set; }
    }
}
