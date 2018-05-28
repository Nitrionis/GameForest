using System;
using System.Diagnostics;
using Graphics;

namespace Scene
{
	public class SnackMap : SnackGroup
	{
		private int[][] map;

		protected int[] sortBuffer;

		private ExplosionsGroup explosionsGroup;

		private Random random = new Random();

		public Stopwatch sw { get; private set; }

		public SnackMap(Game.Scene scene,ExplosionsGroup explosionsGroup, Texture texture, int sizeX, int sizeY)
			: base(scene, texture, sizeX, sizeY)
		{
			this.explosionsGroup = explosionsGroup;

			sortBuffer = new int[sizeY];

			map = new int[sizeX][];
			for (int snackIndex = 0, x = 0; x < sizeX; x++)
			{
				map[x] = new int[sizeY];
				for (int y = 0; y < sizeX; y++, snackIndex++)
					map[x][y] = snackIndex;
			}

			sw = new Stopwatch();
		}

		public override void Update()
		{
			if (sw.IsRunning && sw.ElapsedMilliseconds > 3000)
			{
				if (CheckSequence())
				{
					sw.Restart();
				}
				else
				{
					sw.Stop();
				}
			}
		}

		public Snack GetSnack(int x, int y)
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
				for (int falseIndex = 0, trueIndex = sizeY - 1, y = 0; y < sizeY; y++)
				{
					if (GetSnack(x, y).deleteFlag)
					{
						sortBuffer[trueIndex] = map[x][y];
						trueIndex--;

						Snack snack = GetSnack(x, y);
						explosionsGroup.CreateExplosionIn(x, y);
						snack.snackId = random.Next(5);
						snack.UpdateUvOffsetUsingId();
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

		public bool CheckSequence()
		{
			bool removeSnacks = false;
			int adjacentCount;
			Snack prevSnack;
			for (int y = 0; y < sizeY; y++)
			{
				adjacentCount = 1;
				prevSnack = GetSnack(0, y);
				for (int x = 1; x < sizeX; x++)
				{
					Snack currSnack = GetSnack(x, y);
					if (currSnack.snackId == prevSnack.snackId)
					{
						adjacentCount++;
					}
					else
					{
						if (adjacentCount >= 3)
						{
							removeSnacks = true;
							for (int end = x - adjacentCount, i = x - 1; i >= end; i--)
							{
								DeleteSnack(i, y);
							}
						}
						adjacentCount = 1;
					}
					prevSnack = currSnack;
				}
				if (adjacentCount >= 3)
				{
					removeSnacks = true;
					for (int end = sizeX - adjacentCount, i = sizeX - 1; i >= end; i--)
					{
						DeleteSnack(i, y);
					}
				}
			}

			for (int x = 0; x < sizeX; x++)
			{
				adjacentCount = 1;
				prevSnack = GetSnack(x, 0);
				for (int y = 1; y < sizeY; y++)
				{
					Snack currSnack = GetSnack(x, y);
					if (currSnack.snackId == prevSnack.snackId)
					{
						adjacentCount++;
					}
					else
					{
						if (adjacentCount >= 3)
						{
							removeSnacks = true;
							for (int end = y - adjacentCount, i = y - 1; i >= end; i--)
							{
								DeleteSnack(x, i);
							}
						}
						adjacentCount = 1;
					}
					prevSnack = currSnack;
				}
				if (adjacentCount >= 3)
				{
					removeSnacks = true;
					for (int end = sizeY - adjacentCount, i = sizeY - 1; i >= end; i--)
					{
						DeleteSnack(x, i);
					}
				}
			}

			if (removeSnacks)
			{
				System.Console.WriteLine("Remove snacks start");
				PrintMap();
				System.Console.WriteLine("Remove snacks del");
				DeleteSnacks();
				PrintMap();
				System.Console.WriteLine("Remove snacks end");
			}

			return removeSnacks;
		}

		public void PrintMap()
		{
			for (int y = sizeY - 1; y >= 0; y--)
			{
				for (int x = 0; x < sizeX; x++)
				{
					Snack snack = GetSnack(x, y);
					System.Console.Write(snack.snackId + " ");
				}
				System.Console.WriteLine();
			}
		}
	}
}