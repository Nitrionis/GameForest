using Game;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Scene
{
	public class MainScene : Scene
	{
		private Button button;

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
				System.Console.WriteLine("MainScene Event(int state)");
			}
		}

		public MainScene()
		{
			GL.ClearColor(new Color4(255, 34, 34, 255));

			TexturedRectangle texturedRectangle = new TexturedRectangle(
				new PosSegment(-0.3f,  -0.1f, 0.3f, 0.1f),
				new UvSegment(0.0f, 0.0f, 0.125f*3, 0.125f));

			texturedRectangle.texture = new Texture("D:/pdf_sit/GameForest/Console/resources/StartUiScene/atlas.png");
			Instantiate(texturedRectangle);

			button = new Button(texturedRectangle);
			ButtonCheker buttonCheker = new ButtonCheker(texturedRectangle);
			button.listeners.Add(buttonCheker);
			Instantiate(button);
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