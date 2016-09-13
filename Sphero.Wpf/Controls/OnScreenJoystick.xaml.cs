using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SpheroController.Wpf.Controls
{
	/// <summary>Interaction logic for Joystick.xaml</summary>
	public partial class OnScreenJoystick : UserControl
	{
		/// <summary>Current angle in degrees from 0 to 360</summary>
		public static readonly DependencyProperty AngleProperty =
			DependencyProperty.Register("Angle", typeof(double), typeof(OnScreenJoystick), null);

		/// <summary>Current distance (or "power"), from 0 to 100</summary>
		public static readonly DependencyProperty DistanceProperty =
			DependencyProperty.Register("Distance", typeof(double), typeof(OnScreenJoystick), null);

		/// <summary>How often should be raised StickMove event in degrees</summary>
		public static readonly DependencyProperty AngleStepProperty =
			DependencyProperty.Register("AngleStep", typeof(double), typeof(OnScreenJoystick), new PropertyMetadata(1.0));

		/// <summary>How often should be raised StickMove event in distance units</summary>
		public static readonly DependencyProperty DistanceStepProperty =
			DependencyProperty.Register("DistanceStep", typeof(double), typeof(OnScreenJoystick), new PropertyMetadata(1.0));

		/* Unstable - needs work */
		///// <summary>Indicates whether the joystick knob resets its place after being released</summary>
		//public static readonly DependencyProperty ResetKnobAfterReleaseProperty =
		//    DependencyProperty.Register(nameof(ResetKnobAfterRelease), typeof(bool), typeof(VirtualJoystick), new PropertyMetadata(true));

		/// <summary>Current angle in degrees from 0 to 360</summary>
		public double Angle
		{
			get { return Convert.ToDouble(GetValue(AngleProperty)); }
			set { SetValue(AngleProperty, value); }
		}

		/// <summary>current distance (or "power"), from 0 to 100</summary>
		public double Distance
		{
			get { return Convert.ToDouble(GetValue(DistanceProperty)); }
			set { SetValue(DistanceProperty, value); }
		}

		/// <summary>How often should be raised StickMove event in degrees</summary>
		public double AngleStep
		{
			get { return Convert.ToDouble(GetValue(AngleStepProperty)); }
			set
			{
				if (value < 1) value = 1; else if (value > 90) value = 90;
				SetValue(AngleStepProperty, Math.Round(value));
			}
		}

		/// <summary>How often should be raised StickMove event in distance units</summary>
		public double DistanceStep
		{
			get { return Convert.ToDouble(GetValue(DistanceStepProperty)); }
			set
			{
				if (value < 1) value = 1; else if (value > 50) value = 50;
				SetValue(DistanceStepProperty, value);
			}
		}

		/// <summary>Indicates whether the joystick knob resets its place after being released</summary>
		//public bool ResetKnobAfterRelease
		//{
		//    get { return Convert.ToBoolean(GetValue(ResetKnobAfterReleaseProperty)); }
		//    set { SetValue(ResetKnobAfterReleaseProperty, value); }
		//}

		/// <summary>Delegate holding data for joystick state change</summary>
		/// <param name="sender">The object that fired the event</param>
		/// <param name="args">Holds new values for angle and distance</param>
		public delegate void OnScreenJoystickEventHandler(OnScreenJoystick sender, VirtualJoystickEventArgs args);

		/// <summary>Delegate for joystick events that hold no data</summary>
		/// <param name="sender">The object that fired the event</param>
		public delegate void EmptyJoystickEventHandler(OnScreenJoystick sender);

		/// <summary>This event fires whenever the joystick moves</summary>
		public event OnScreenJoystickEventHandler Moved;

		/// <summary>This event fires once the joystick is released and its position is reset</summary>
		public event EmptyJoystickEventHandler Released;

		/// <summary>This event fires once the joystick is captured</summary>
		public event EmptyJoystickEventHandler Captured;

		private PointerPoint _startPos;
		//private Point _startPos;
		private double _prevAngle, _prevDistance;
		//private readonly Storyboard centerKnob;

		public OnScreenJoystick()
		{
			InitializeComponent();

			Knob.PointerPressed += Knob_PointerPressed;
			Knob.PointerReleased += Knob_PointerReleased;
			Knob.PointerMoved += Knob_PointerMoved;
			//Knob.MouseLeftButtonDown += Knob_MouseLeftButtonDown;
			//Knob.MouseLeftButtonUp += Knob_MouseLeftButtonUp;
			//Knob.MouseMove += Knob_MouseMove;

			//centerKnob = Knob.Resources["CenterKnob"] as Storyboard;
		}

		private void Knob_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			//_startPos = e.GetPosition(Base);
			_startPos = e.GetCurrentPoint(Base);
			_prevAngle = _prevDistance = 0;

			Captured?.Invoke(this);
			Knob.CapturePointer(e.Pointer);

			centerKnob.Stop();
		}

		/*		private void Knob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
				{
					_startPos = e.GetPosition(Base);
					_prevAngle = _prevDistance = 0;

					Captured?.Invoke(this);
					Knob.CaptureMouse();

					centerKnob.Stop();
				}*/

		private void Knob_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (Knob.PointerCaptures == null) return;
			if (Knob.PointerCaptures.Count == 0) return;
			//if (!Knob.PointerCaptures.Contains(e.Pointer)) return;

			PointerPoint newPos = e.GetCurrentPoint(Base);

			Point deltaPos = new Point(newPos.Position.X - _startPos.Position.X, newPos.Position.Y - _startPos.Position.Y);

			double angle = Math.Atan2(deltaPos.Y, deltaPos.X) * 180 / Math.PI;
			if (angle > 0)
				angle += 90;
			else
			{
				angle = 270 + (180 + angle);
				if (angle >= 360) angle -= 360;
			}

			double distance = Math.Round(Math.Sqrt(deltaPos.X * deltaPos.X + deltaPos.Y * deltaPos.Y) / 135 * 100);
			if (distance <= 100)
			{
				Angle = angle;
				Distance = distance;

				knobPosition.X = deltaPos.X;
				knobPosition.Y = deltaPos.Y;

				if (Moved == null ||
					(!(Math.Abs(_prevAngle - angle) > AngleStep) && !(Math.Abs(_prevDistance - distance) > DistanceStep)))
					return;

				Moved?.Invoke(this, new VirtualJoystickEventArgs { Angle = Angle, Distance = Distance });
				_prevAngle = Angle;
				_prevDistance = Distance;
			}
		}

		/*
private void Knob_MouseMove(object sender, MouseEventArgs e)
{
	if (!Knob.IsMouseCaptured) return;

	Point newPos = e.GetPosition(Base);

	Point deltaPos = new Point(newPos.X - _startPos.X, newPos.Y - _startPos.Y);

	double angle = Math.Atan2(deltaPos.Y, deltaPos.X) * 180 / Math.PI;
	if (angle > 0)
		angle += 90;
	else
	{
		angle = 270 + (180 + angle);
		if (angle >= 360) angle -= 360;
	}

	double distance = Math.Round(Math.Sqrt(deltaPos.X * deltaPos.X + deltaPos.Y * deltaPos.Y) / 135 * 100);
	if (distance <= 100)
	{
		Angle = angle;
		Distance = distance;

		knobPosition.X = deltaPos.X;
		knobPosition.Y = deltaPos.Y;

		if (Moved == null ||
			(!(Math.Abs(_prevAngle - angle) > AngleStep) && !(Math.Abs(_prevDistance - distance) > DistanceStep)))
			return;

		Moved?.Invoke(this, new VirtualJoystickEventArgs { Angle = Angle, Distance = Distance });
		_prevAngle = Angle;
		_prevDistance = Distance;
	}
}*/

		/*private void Knob_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Knob.ReleaseMouseCapture();
			centerKnob.Begin();
		}*/

		private void Knob_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			Knob.ReleasePointerCapture(e.Pointer);
			centerKnob.Begin();
		}

		private void centerKnob_Completed(object sender, object e)
		{
			Angle = Distance = _prevAngle = _prevDistance = 0;
			Released?.Invoke(this);
		}

		/*private void centerKnob_Completed(object sender, EventArgs e)
		{
			Angle = Distance = _prevAngle = _prevDistance = 0;
			Released?.Invoke(this);
		}*/
	}
}
