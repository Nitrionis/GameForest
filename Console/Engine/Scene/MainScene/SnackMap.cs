using System;
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
			for (int x = 0; x < sizeX; x++)
				map[x] = new int[sizeY];
		}

		private int deleteCallCount = 0;

		public override void Draw()
		{
			if ((DateTime.Now.Second % 5 == 0) && (DateTime.Now.Millisecond < 20))
			{
				deleteCallCount++;
				if (deleteCallCount > 1)
					System.Console.WriteLine("deleteCallCount > 1");

				DeleteSnack(0, 0);
				DeleteSnacks();
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

		private void DeleteSnacks()
		{
			for (int x = 0; x < sizeX; x++)
			{
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

				for (int y = sizeY - 1; y >= 0; y--)
				{
					var snack = GetSnack(x, y);

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
				}
			}
		}
	}
}