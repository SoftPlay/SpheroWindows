using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SpheroController.Common;

namespace SpheroController.Tests.Unit
{
	[TestClass]
	public class JoystickPositionFacts
	{
		[TestMethod]
		public void Ctor_Always_SetsXandY()
		{
			var sut = new JoystickPosition(0.25, 0.5);

			Assert.AreEqual(0.25, sut.X);
			Assert.AreEqual(0.5, sut.Y);
		}

		[TestMethod]
		public void Distance_WhenStraightAheadFullThrottle_EqualsOne()
		{
			var sut = new JoystickPosition(0, 1);

			Assert.AreEqual(1, sut.Distance);
		}

		[TestMethod]
		public void Angle_WhenStraightAheadFullThrottle_EqualsZero()
		{
			var sut = new JoystickPosition(0, 1);

			Assert.AreEqual(0, sut.Angle);
		}

		[TestMethod]
		public void Distance_WhenRightFullThrottle_EqualsOne()
		{
			var sut = new JoystickPosition(1, 0);

			Assert.AreEqual(1, sut.Distance);
		}

		[TestMethod]
		public void Angle_WhenRightFullThrottle_Equals90()
		{
			var sut = new JoystickPosition(1, 0);

			Assert.AreEqual(90, sut.Angle);
		}

		[TestMethod]
		public void Distance_WhenReverseFullThrottle_EqualsOne()
		{
			var sut = new JoystickPosition(0, -1);

			Assert.AreEqual(1, sut.Distance);
		}

		[TestMethod]
		public void Angle_WhenReverseFullThrottle_Equals180()
		{
			var sut = new JoystickPosition(0, -1);

			Assert.AreEqual(180, sut.Angle);
		}

		[TestMethod]
		public void Distance_WhenLeftFullThrottle_EqualsOne()
		{
			var sut = new JoystickPosition(-1, 0);

			Assert.AreEqual(1, sut.Distance);
		}

		[TestMethod]
		public void Angle_WhenLeftFullThrottle_Equals270()
		{
			var sut = new JoystickPosition(-1, 0);

			Assert.AreEqual(270, sut.Angle);
		}

		[TestMethod]
		public void Distance_WhenHalfThrottleToTopRight_EqualsHalf()
		{
			var sut = new JoystickPosition(0.5, 0.5);

			Assert.AreEqual(0.707, Math.Round(sut.Distance, 3));
		}

		[TestMethod]
		public void Distance_ForwardsAndSlightlyRight_3Degrees()
		{
			var sut = new JoystickPosition(0.05, 1);
			
			Assert.AreEqual(3, Math.Round(sut.Angle, 0));
		}

		[TestMethod]
		public void Distance_ForwardsAndSlightlyLeft_357()
		{
			var sut = new JoystickPosition(-0.05, 1);

			Assert.AreEqual(357, Math.Round(sut.Angle, 0));
		}

		[TestMethod]
		public void Distance_ReverseAndSlightlyRight_177()
		{
			var sut = new JoystickPosition(0.05, -1);

			Assert.AreEqual(177, Math.Round(sut.Angle, 0));
		}

		[TestMethod]
		public void Distance_ReverseAndSlightlyLeft_183()
		{
			var sut = new JoystickPosition(-0.05, -1);

			Assert.AreEqual(183, Math.Round(sut.Angle, 0));
		}

		[TestMethod]
		public void Angle_WhenTopRight_Equals45()
		{
			var sut = new JoystickPosition(1, 1);

			Assert.AreEqual(45, sut.Angle);
		}

		[TestMethod]
		public void Angle_WhenBottomRight_Equals135()
		{
			var sut = new JoystickPosition(1, -1);

			Assert.AreEqual(135, sut.Angle);
		}

		[TestMethod]
		public void Angle_WhenBottomLeft_Equals225()
		{
			var sut = new JoystickPosition(-1, -1);

			Assert.AreEqual(225, sut.Angle);
		}

		[TestMethod]
		public void Angle_WhenTopLeft_Equals315()
		{
			var sut = new JoystickPosition(-1, 1);

			Assert.AreEqual(315, sut.Angle);
		}
	}
}
