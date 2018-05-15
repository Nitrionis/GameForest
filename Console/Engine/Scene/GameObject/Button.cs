using OpenTK.Input;
using static Game.GlobalRef;

namespace Scene
{
	public class Button : GameObject
	{
		public int state = 0;

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
			PosSegment location = texturedRectangle.posSegment;
			double x = cursorPos.X / (double)window.Width * 2 - 1;
			double y = cursorPos.Y / (double)window.Height * 2 - 1;
			if (location.startX <= x
			    && location.endX >= x
			    && location.startY <= y
			    && location.endY >= y)
			{
				state = 1;
				MouseState mouseState = OpenTK.Input.Mouse.GetState();
				bool leftMouseDown = mouseState.IsButtonDown(MouseButton.Left);
				if (leftMouseDown)
				{
					state = 2;
				}
			}
			else
			{
				state = 0;
			}

			if (texturedRectangle.offsetV != 0.125f * state)
			{
				texturedRectangle.offsetV = 0.125f * state;
				texturedRectangle.updateFlaf = true;
			}
		}
	}
}