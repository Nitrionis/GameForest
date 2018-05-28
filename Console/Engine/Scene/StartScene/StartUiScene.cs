using System.IO;
using Game;
using Scene;
using Graphics;
using OpenTK.Input;
using static Game.GlobalReference;

namespace Game
{
	public class StartUiScene : Scene
	{
		private Button button;
		private Button buttonTwo;

		class ButtonCheker : IButtonAction
		{
			private TexturedRectangle texturedRectangle;

			public ButtonCheker(TexturedRectangle texturedRectangle)
			{
				this.texturedRectangle = texturedRectangle;
			}

			public void Event(int state)
			{
				texturedRectangle.offsetV = 0.125f * state;
				texturedRectangle.updateFlaf = true;
				if (state == 2 && !window.sceneChanging)
					window.ChangeScene<MainScene>();
			}
		}

		public StartUiScene()
		{
			TexturedRectangle texturedRectangle = new TexturedRectangle(
				new RectLocation(-0.3f,  -0.1f, 0.3f, 0.1f),
				new RectUv(0.0f, 0.0f, 0.125f*3, 0.125f));

			texturedRectangle.texture = new Texture(
				".\\resources\\StartUiScene\\atlas.png");
			Instantiate(texturedRectangle);

			button = new Button(texturedRectangle);
			ButtonCheker buttonCheker = new ButtonCheker(texturedRectangle);
			button.listeners.Add(buttonCheker);
			Instantiate(button);
		}
	}
}