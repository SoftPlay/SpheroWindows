namespace SpheroController.Wpf.Robots
{
	public interface ISphero : IRobot
	{
		void Roll(int heading, float speed);
		void SetBackLED(float intensity);
		void SetHeading(int heading);
		void SetRGBLED(int red, int green, int blue);
	}
}