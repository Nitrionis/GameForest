using System;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Scene;
using Button = Scene.Button;

namespace Game
{
	public class MainScene : Scene
	{
		private DateTime endTime = DateTime.Now.AddMinutes(1);

		private Button[] buttons = new Button[64];
		private TexturedRectangle[] texturedRectangles = new TexturedRectangle[64];

		private TextObject textObject;

		class ButtonCheker : IButtonAction
		{
			private TexturedRectangle texturedRectangle;

			public ButtonCheker(TexturedRectangle texturedRectangle)
			{
				this.texturedRectangle = texturedRectangle;
			}

			public void Event(int state)
			{
				texturedRectangle.offsetU = 0.125f * state;
				texturedRectangle.updateFlaf = true;
				System.Console.WriteLine("MainScene Event(int state)");
			}
		}

		public MainScene()
		{
			Texture mainAtlas = new Texture("D:/pdf_sit/GameForest/Console/resources/MainScene/atlas.png");

			Random random = new Random();

			for (int i = 0; i < buttons.Length; i++)
			{
				float startX = i % 8 * 0.2f - 1, startY = i / 8 * 0.2f - 1;
				float endX = startX + 0.2f, endY = startY + 0.2f;

				int eatId = random.Next(5);

				texturedRectangles[i] = new TexturedRectangle(
					new PosSegment(startX,  startY, endX, endY),
					new UvSegment(0.125f * eatId, 0.0f, 0.125f * (eatId+1), 0.125f));
				texturedRectangles[i].texture = mainAtlas;

				buttons[i] = new Button(texturedRectangles[i]);

				Instantiate(texturedRectangles[i]);
				Instantiate(buttons[i]);

				ButtonCheker buttonCheker = new ButtonCheker(texturedRectangles[i]);
				buttons[i].listeners.Add(buttonCheker);
				Instantiate(buttons[i]);
			}

			textObject = new TextObject(
				new PosSegment(0.0f,  0.8f, 0.1f, 0.9f), "0123456789");
			Instantiate(textObject);

			GL.ClearColor(new Color4(34, 34, 34, 255));
		}

		private int seconds = 0;

		public override void Update()
		{
			base.Update();
			if (endTime > DateTime.Now)
			{
				CheckEvents();
				textObject.SetText((endTime - DateTime.Now).Seconds.ToString());
			}
		}

		private void CheckEvents()
		{

		}
	}
}