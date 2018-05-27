using System;

namespace Scene
{
	public class MapGenerator
	{
		public const int UniqueSnacksCount = 5;

		public const int DeletedSnack = -1;

		public int size_by_x, size_by_y;

		public int[,] map { get; private set; }

		Random random = new Random();

		public MapGenerator(int x = 8, int y = 8)
		{
			size_by_x = x;
			size_by_y = y;
			map = new int[x,y];
			ResetMap();
			NewMap();
		}

		public void CheckSequence()
		{
			for (int y = 0; y < size_by_y; y++)
			{
				int adjacentCount = 0;
				for (int x = 0; x < size_by_x; x++)
				{

				}
			}
		}

		public void ResetMap()
		{
			for (int x = 0; x < size_by_y; x++)
			{
				for (int y = 0; y < size_by_x; y++)
				{
					map[x, y] = -1;
				}
			}
		}

		public int NewSnack(int x, int y, bool autoset = true)
		{
			int snack;

			while (true)
			{
				snack = random.Next(UniqueSnacksCount);

				if (y != 0 && map[x, y-1] == snack)
					continue;

				if (y != size_by_y - 1 && map[x, y+1] == snack)
					continue;

				if (x != 0 && map[x-1, y] == snack)
					continue;

				if (x != size_by_x - 1 && map[x+1, y] == snack)
					continue;

				break;
			}

			map[x, y] = autoset ? snack : map[x, y];
			return snack;
		}

		public void NewMap()
		{
			for (int x = 0; x < size_by_x; x++)
			{
				for (int y = 0; y < size_by_y; y++)
				{
					NewSnack(x, y);
				}
			}
		}

		public void UpdateMap()
		{
			for (int x = 0; x < size_by_x; x++)
			{
				for (int y = 0; y < size_by_y; y++)
				{
					if (map[x, y] == -1)
						//NewSnack(x, y);
						map[x, y] = random.Next(UniqueSnacksCount);
				}
			}
		}
	}
}