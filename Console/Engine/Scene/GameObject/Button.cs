namespace Scene
{
	public class Button : GameObject
	{
		private TexturedRectangle texturedRectangle;

		public Button(TexturedRectangle texturedRectangle)
		{
			this.texturedRectangle = texturedRectangle;
		}

		public override void FixedApdate()
		{
			if (active)
				CheckPressed();
		}

		private void CheckPressed()
		{

		}
	}
}