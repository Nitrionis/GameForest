
namespace Scene
{
	public class StartUiScene : Scene
	{
		private TexturedRectangle texturedRectangle;

		public StartUiScene()
		{
			texturedRectangle = new TexturedRectangle(
				-0.3f, 0.3f, -0.1f, 0.1f, // Screen space
				0.675f, 1.0f, 0.0f, 0.125f // UV
			);
			Instantiate(texturedRectangle);
		}

		public override void Update()
		{
			base.Update();
			CheckEvents();
		}

		private void CheckEvents()
		{

		}
	}
}