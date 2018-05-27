using System;
using System.Drawing;
using Game;
using Graphics;
using OpenTK.Input;

namespace Scene
{
	public class MovableQuads : GameObject
	{
		public enum ActiveQuad {Noactive, FirstActive, SecondActive};

		public ActiveQuad activeQuad { get; private set; } = ActiveQuad.Noactive;

		private const float UvQuadSize = 0.0625f;
		private const float XyQuadSize = 0.2f;

		private SnackMap snackMap;
		private ExplosionsGroup explosionsGroup;

		private TexturedRectangle rectangleOne;
		private TexturedRectangle rectangleTwo;

		public int firstX { get; private set; }
		public int firstY { get; private set; }
		public int secondX { get; private set; }
		public int secondY { get; private set; }

		public MovableQuads(Game.Scene scene, SnackMap snackMap, ExplosionsGroup explosionsGroup, Texture texture)
		{
			this.snackMap = snackMap;
			this.explosionsGroup = explosionsGroup;

			rectangleOne = new TexturedRectangle(
				new RectLocation(-1.0f, -1.0f, -1.0f, -1.0f),
				new RectUv(2*UvQuadSize, 15*UvQuadSize, 3*UvQuadSize, 16*UvQuadSize));
			rectangleOne.texture = texture;
			scene.Instantiate(rectangleOne);

			rectangleTwo = new TexturedRectangle(
				new RectLocation(-1.0f, -1.0f, -1.0f, -1.0f),
				new RectUv(2*UvQuadSize, 15*UvQuadSize, 3*UvQuadSize, 16*UvQuadSize));
			rectangleTwo.texture = texture;
			scene.Instantiate(rectangleTwo);

			firstX = -1;
			firstY = -1;
			secondX = -1;
			secondY = -1;
		}

		public void SetFirstQuadPos(int x, int y)
		{
			firstX = x;
			firstY = y;

			rectangleOne.pos.startX = -1.0f + x * XyQuadSize;
			rectangleOne.pos.endX = rectangleOne.pos.startX + XyQuadSize;

			rectangleOne.pos.startY = -1.0f + y * XyQuadSize;
			rectangleOne.pos.endY = rectangleOne.pos.startY + XyQuadSize;

			rectangleOne.updateFlaf = true;
		}

		public void SetSecondQuadPos(int x, int y)
		{
			secondX = x;
			secondY = y;

			rectangleTwo.pos.startX = -1.0f + x * XyQuadSize;
			rectangleTwo.pos.endX = rectangleTwo.pos.startX + XyQuadSize;

			rectangleTwo.pos.startY = -1.0f + y * XyQuadSize;
			rectangleTwo.pos.endY = rectangleTwo.pos.startY + XyQuadSize;

			rectangleTwo.updateFlaf = true;
		}

		public override void Update()
		{
			Point newPos = GlobalReference.cursorPos;
			newPos.X /= (GlobalReference.window.Width / 10);
			newPos.Y /= (GlobalReference.window.Height / 10);
			newPos.Y = 9 - newPos.Y;

			if (activeQuad == ActiveQuad.FirstActive)
			{
				if (newPos.X < 8 && newPos.Y < 8)
				{
					SetSecondQuadPos(newPos.X, newPos.Y);
				}
			}
		}

		public override void MbdDawn()
		{
			Point newPos = GlobalReference.cursorPos;
			newPos.X /= (GlobalReference.window.Width / 10);
			newPos.Y /= (GlobalReference.window.Height / 10);
			newPos.Y = 9 - newPos.Y;

			if (activeQuad == ActiveQuad.Noactive)
			{
				if (newPos.X < 8 && newPos.Y < 8)
				{
					activeQuad = ActiveQuad.FirstActive;
					SetFirstQuadPos(newPos.X, newPos.Y);
				}
			}
			else if (activeQuad == ActiveQuad.FirstActive)
			{
				if (newPos.X < 8 && newPos.Y < 8)
				{
					if (newPos.X != firstX || newPos.Y != firstY)
					{
						int xDist = Math.Abs(newPos.X - firstX),
							yDist = Math.Abs(newPos.Y - firstY);

						if (xDist + yDist <= 1)
						{
							var snackOne = snackMap.GetSnack(newPos.X, newPos.Y);
							var snackTwo = snackMap.GetSnack(firstX, firstY);

							int value = snackOne.snackId;
							snackOne.snackId = snackTwo.snackId;
							snackTwo.snackId = value;

							if (snackMap.CheckSequence())
							{
								snackOne.UpdateUvOffsetUsingId();
								snackTwo.UpdateUvOffsetUsingId();

								activeQuad = ActiveQuad.Noactive;
								SetFirstQuadPos(-1, -1);
								SetSecondQuadPos(-1, -1);

								//while (snackMap.CheckSequence());
							}
							else
							{
								System.Console.WriteLine("CheckSequence() false");
								value = snackOne.snackId;
								snackOne.snackId = snackTwo.snackId;
								snackTwo.snackId = value;
							}
						}
					}
					else
					{
						activeQuad = ActiveQuad.Noactive;
						SetFirstQuadPos(-1, -1);
						SetSecondQuadPos(-1, -1);
					}
				}
			}
		}
	}
}