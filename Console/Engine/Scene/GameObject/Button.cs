using System.Collections.Generic;
using OpenTK.Input;
using static Game.GlobalReference;

namespace Scene
{
	public interface IButtonAction
	{
		void Event(int state);
	}

	public class Button : GameObject
	{
		public volatile int state;

		public List<IButtonAction> listeners { get; private set; } = new List<IButtonAction>();

		private IRectangleGetter texturedRectangle;

		public Button(IRectangleGetter texturedRectangle)
		{
			this.texturedRectangle = texturedRectangle;
		}

		public override void FixedApdate()
		{
			if (active)
				CheckEvents();
		}

		private void CheckEvents()
		{
			int newState;
			RectLocation location = texturedRectangle.GetPosSegment();

			double x = cursorPos.X / (double)window.Width * 2 - 1;
			double y = (window.Height - cursorPos.Y) / (double)window.Height * 2 - 1;

			if (location.startX <= x
			    && location.endX >= x
			    && location.startY <= y
			    && location.endY >= y)
			{
				newState = 1;
				MouseState mouseState = Mouse.GetState();
				bool leftMouseDown = mouseState.IsButtonDown(MouseButton.Left);
				if (leftMouseDown)
				{
					newState = 2;
				}
			}
			else
			{
				newState = 0;
			}

			if (newState != state)
			{
				state = newState;
				foreach (var listener in listeners)
				{
					listener.Event(state);
				}
			}
		}
	}
}