using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using RobotKit;
using SpheroController.Common.Interfaces;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SpheroController.Wpf.ViewModels
{
    public class SpheroViewModel : BindableBase, ISpheroViewModel
	{
        private readonly ISphero sphero;

        private Color color = Colors.Black;

        private bool tailLight = false;
        private double rollAngle;
        private double rollDistance;
        private double rotateAngle;

        public SpheroViewModel(ISphero sphero)
        {
            this.sphero = sphero;
        }

        public string Name
        {
            get
            {
                return $"{this.sphero.Name} ({this.sphero.BluetoothName})";
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

        public double RotateAngle
        {
            get
            {
                return this.rotateAngle;
            }

            set
            {
                if (this.SetProperty(ref this.rotateAngle, value))
                {
                    
                    this.sphero.Roll((int)this.rotateAngle, 0);

                    // For Sphero to have a new heading we need to rotate and then call SetHeading
                    // apparently with a delay in the middle
                    Task.Delay(1000).Wait(2000);

                    this.sphero.SetHeading(0);
                }
            }
        }

        private void Roll()
        {
            if (this.RollDistance > 0)
            {
                this.sphero.Roll((int)this.RollAngle, (float)this.RollDistance / 100);
            } else
            {
                this.sphero.Roll(0, 0f);
            }
        }
    }
}
