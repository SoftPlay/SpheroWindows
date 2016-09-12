using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace SpheroController.Wpf.Controls
{
    public class ColorWheel : UserControl
    {
        //! @brief	translate transform for the puck
        private TranslateTransform m_translateTransform;
		
        //! @brief  the initial point that we are referencing
        private Point m_initialPoint;

        //! @brief  the last time a command was sent in milliseconds
        private long m_lastCommandSentTimeMs;

        /*!
         * @brief	creates a joystick with the given @a puck element for a @a sphero
         * @param	puck the puck to control with the joystick
         * @param	sphero the sphero to control
         */
        public ColorWheel() {			
            this.PointerPressed += ColorWheel_PointerPressed;
            this.PointerMoved += ColorWheel_PointerMoved;
			this.PointerReleased += ColorWheel_PointerReleased;
			this.PointerCaptureLost += ColorWheel_PointerReleased;
			this.PointerCanceled += ColorWheel_PointerReleased;

            m_translateTransform = new TranslateTransform();
            this.RenderTransform = m_translateTransform;

            m_lastCommandSentTimeMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

		public DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorWheel), new PropertyMetadata(false));

		public Color Color
		{
			get { return (Color)this.GetValue(ColorProperty); }
			set { this.SetValue(ColorProperty, value); }
		}

		//! @brief  handle the user starting to drive
		private void ColorWheel_PointerPressed(object sender, PointerRoutedEventArgs args) {
            Windows.UI.Input.PointerPoint pointer = args.GetCurrentPoint(null);
            if (pointer.Properties.IsLeftButtonPressed) {
                m_initialPoint = new Point(pointer.RawPosition.X - m_translateTransform.X, pointer.RawPosition.Y - m_translateTransform.Y);
                args.Handled = true;
                this.CapturePointer(args.Pointer);
            }
        }

        //! @brief  handle the user driving
        private void ColorWheel_PointerMoved(object sender, PointerRoutedEventArgs args) {
            Windows.UI.Input.PointerPoint pointer = args.GetCurrentPoint(null);
            if (pointer.Properties.IsLeftButtonPressed) {
                Point newPoint = pointer.RawPosition;
                Point delta = new Point(
                    newPoint.X - m_initialPoint.X,
                    newPoint.Y - m_initialPoint.Y);

                m_translateTransform.X = delta.X;
                m_translateTransform.Y = delta.Y;
                args.Handled = true;

                ConstrainToParent();

                SendRgbCommand();
            }
        }

        //! @brief  constrains the joystick to its parent
        private void ConstrainToParent() {
            FrameworkElement parent = this.Parent as FrameworkElement;
            float radius = (float)Math.Min(parent.ActualWidth, parent.ActualHeight) / 2f;

            float distanceSq = (float)(
                m_translateTransform.X * m_translateTransform.X
                + m_translateTransform.Y * m_translateTransform.Y);

            float radiusSq = radius * radius;
            if (distanceSq > radiusSq) {
                float length = (float)Math.Sqrt(distanceSq);
                Point positionNorm = new Point(m_translateTransform.X / length, m_translateTransform.Y / length);

                m_translateTransform.X = positionNorm.X * radius;
                m_translateTransform.Y = positionNorm.Y * radius;
            }
        }

        //! @brief  handle the user completing driving
        private void ColorWheel_PointerReleased(object sender, PointerRoutedEventArgs args) {
            SendRgbCommand();
            this.ReleasePointerCapture(args.Pointer);
            args.Handled = true;
        }

        /*!
         * @brief	sends an rgb command to the sphero given the current translation
         */
        private void SendRgbCommand() {
            FrameworkElement parent = this.Parent as FrameworkElement;
            if (parent == null) {
                return;
            }

            Size size = new Size(parent.ActualWidth, parent.ActualHeight);

            float x = (float)m_translateTransform.X;
            float y = (float)m_translateTransform.Y;

            x /= (float)size.Width * .5f;
            y /= (float)size.Height * .5f;

            float speed = x * x + y * y;
            speed = (speed == 0) ? 0 : (float)Math.Sqrt(speed);
            if (speed > 1f) {
                speed = 1f;
            }

            double rad = Math.Atan2((double)y, (double)x);
            rad += Math.PI / 2.0;
            double degrees = rad * 180.0 / Math.PI;
            int degreesCapped = (((int)degrees) + 360) % 360;

            Color RgbColor = ColorFromHSV(degreesCapped, speed, 1.0);

            // Send RGB command and limit to 10 Hz
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if ((milliseconds - m_lastCommandSentTimeMs) > 100) {
				this.Color = RgbColor;
                m_lastCommandSentTimeMs = milliseconds;
            }
        }

        private Color ColorFromHSV(double hue, double saturation, double value) {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = (byte)Convert.ToInt32(value);
            byte p = (byte)Convert.ToInt32(value * (1 - saturation));
            byte q = (byte)Convert.ToInt32(value * (1 - f * saturation));
            byte t = (byte)Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
