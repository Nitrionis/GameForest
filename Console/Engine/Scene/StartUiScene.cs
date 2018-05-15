
using System.Drawing;
using System.Windows.Forms;
using Graphics;
using OpenTK.Input;
using System.Windows.Input;
using Game;
using OpenTK.Graphics.ES10;
using static Game.GlobalRef;

namespace Scene
{
	public class StartUiScene : Scene
	{
		private Button button;

		public StartUiScene()
		{
			TexturedRectangle texturedRectangle = new TexturedRectangle(
				new PosSegment(-0.3f,  -0.1f, 0.3f, 0.1f),
				new UvSegment(0.0f, 0.0f, 0.125f*3, 0.125f));

			texturedRectangle.texture = new Texture("D:/pdf_sit/GameForest/Console/resources/StartUiScene/atlas.png");
			Instantiate(texturedRectangle);

			button = new Button(texturedRectangle);
			Instantiate(button);
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