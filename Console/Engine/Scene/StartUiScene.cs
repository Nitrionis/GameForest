
using Game;
using Graphics;
using OpenTK.Input;

namespace Scene
{
	public class StartUiScene : Scene
	{
		private Button buttonOne;
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
				if (state == 2)
					GlobalReference.scene = new MainScene();
			}
		}

		public StartUiScene()
		{
			TexturedRectangle texturedRectangle = new TexturedRectangle(
				new PosSegment(-0.3f,  -0.1f, 0.3f, 0.1f),
				new UvSegment(0.0f, 0.0f, 0.125f*3, 0.125f));

			texturedRectangle.texture = new Texture("D:/pdf_sit/GameForest/Console/resources/StartUiScene/atlas.png");
			Instantiate(texturedRectangle);

			buttonOne = new Button(texturedRectangle);
			ButtonCheker buttonCheker = new ButtonCheker(texturedRectangle);
			buttonOne.listeners.Add(buttonCheker);
			Instantiate(buttonOne);


			TexturedRectangle texturedRectangleTwo = new TexturedRectangle(
				new PosSegment(-0.3f,  -1.0f, 0.3f, -0.8f),
				new UvSegment(0.0f, 0.0f, 0.125f*3, 0.125f));

			texturedRectangleTwo.texture = texturedRectangle.texture;
			Instantiate(texturedRectangleTwo);

			buttonTwo = new Button(texturedRectangleTwo);
			ButtonCheker buttonChekerTwo = new ButtonCheker(texturedRectangleTwo);
			buttonTwo.listeners.Add(buttonChekerTwo);
			Instantiate(buttonTwo);
		}

		public override void Update()
		{
			base.Update();
			CheckEvents();
		}

		private void CheckEvents()
		{
			MouseState mouseState = OpenTK.Input.Mouse.GetState();
			bool leftMouseDown = mouseState.IsButtonDown(MouseButton.Left);
			bool rightMouseDown = mouseState.IsButtonDown(MouseButton.Right);
		}
	}
}