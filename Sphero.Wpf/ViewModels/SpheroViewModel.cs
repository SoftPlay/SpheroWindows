using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using RobotKit;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SpheroController.Wpf.ViewModels
{
	public class SpheroViewModel : BindableBase
	{
		private readonly Sphero sphero;

		private Color color = Colors.Black;

		private bool tailLight = false;
		private double rollAngle;
		private double rollDistance;

		public SpheroViewModel(Sphero sphero)
		{
			this.sphero = sphero;
		}

		public string Name
		{
			get
			{
				return string.Format($"{this.sphero.Name} ({this.sphero.BluetoothName})");
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}

			set
			{
				if (this.SetProperty(ref color, value))
				{
					this.sphero.SetRGBLED(value.R, value.G, value.B);
				}
			}
		}

		public bool TailLight
		{
			get
			{
				return this.tailLight;
			}

			set
			{
				if (this.SetProperty(ref this.tailLight, value))
				{
					this.sphero.SetBackLED(value ? 1.0f : 0.0f);
				}
			}
		}

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
					this.Roll();
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
					this.Roll();
				}
			}
		}

		private void Roll()
		{
			if (this.rollDistance > 0)
			{
				this.sphero.Roll((int)this.RollAngle, (float)this.RollDistance / 100);
			}
		}
	}
}
