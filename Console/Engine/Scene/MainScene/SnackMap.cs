using System;
using System.Net;
using Graphics;

namespace Scene
{
	public class SnackMap : SnackGroup
	{
		private int[][] map;

		protected int[] sortBuffer;

		public SnackMap(Game.Scene scene, Texture texture, int sizeX, int sizeY)
			: base(scene, texture, sizeX, sizeY)
		{
			sortBuffer = new int[sizeY];

			map = new int[sizeX][];
			for (int snackIndex = 0, x = 0; x < sizeX; x++)
			{
				map[x] = new int[sizeY];
				for (int y = 0; y < sizeX; y++, snackIndex++)
					map[x][y] = snackIndex;
			}
		}

		private int drawCallCount = 0;

		public override void Draw()
		{
			drawCallCount++;
			if (drawCallCount % 300 == 0)
			{
				System.Console.WriteLine("drawCallCount: " + drawCallCount / 300);

				/*DeleteSnack(0, 0);
				DeleteSnack(0, 1);
				DeleteSnack(0, 2);
				DeleteSnack(0, 3);*/
				//DeleteSnacks();
			}
			base.Draw();
		}

		private Snack GetSnack(int x, int y)
		{
			return snacks[map[x][y]];
		}

		public void DeleteSnack(int x, int y)
		{
			GetSnack(x, y).deleteFlag = true;
		}

		public void DeleteSnacks()
		{
			for (int x = 0; x < sizeX; x++)
			{
				/*if (x != 0)
					continue;// TODO*/
				for (int falseIndex = 0, trueIndex = sizeY - 1, y = 0; y < sizeY; y++)
				{
					if (GetSnack(x, y).deleteFlag)
					{
						sortBuffer[trueIndex] = map[x][y];
						trueIndex--;
					}
					else
					{
						sortBuffer[falseIndex] = map[x][y];
						falseIndex++;
					}
				}

				int[] value = map[x];
				map[x] = sortBuffer;
				sortBuffer = value;

				/*System.Console.WriteLine("V Line");

				if (drawCallCount == 5)
					System.Console.WriteLine("Error");*/

				for (int y = sizeY - 1; y >= 0; y--)
				{
					var snack = GetSnack(x, y);

					//System.Console.WriteLine("PrevPos: " + map[x][y]);

					snack.pos.startY = y * XySnackSize - 1f;
					snack.pos.endY = (y + 1) * XySnackSize - 1f;

					if (snack.height != y || snack.deleteFlag)
					{
						if (snack.deleteFlag || snack.height < y)
						{
							snack.animationTime = 1000;
						}
						else
						{
							snack.animationTime = (snack.height - y) * 100;
						}

						snack.sw.Restart();
						snack.height = y;
					}
					snack.deleteFlag = false;
					//System.Console.WriteLine("NewPos: " + map[x][y]);
				}
			}
		}
	}
}