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
	}
}
