using Graphics;
using Scene;

namespace Game
{
	public class EndUiScene : Scene
	{
		private const float UvSnackSize = 0.125f;
		private const float XySnackSize = 0.2f;

		public EndUiScene()
		{
			Texture mainAtlas = new Texture(".\\resources\\MainScene\\atlas.png");

			TexturedRectangle scoreText = new TexturedRectangle(
				new RectLocation(-0.95f,  0.78f, -0.45f, 0.92f),
				new RectUv(4*UvSnackSize, UvSnackSize, 8*UvSnackSize, 2*UvSnackSize));
			scoreText.texture = mainAtlas;
			Instantiate(scoreText);

			TextObject scoreObject = new TextObject(
				new RectLocation(
					-2*XySnackSize,  4*XySnackSize, -2*XySnackSize + 0.1f, 0.90f),
				GlobalReference.score.ToString());
			Instantiate(scoreObject);
		}
	}
}