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
			map = new int[y,x];
			ResetMap();
			NewMap();
		}

		public void ResetMap()
		{
			for (int y = 0; y < size_by_y; y++)
			{
				for (int x = 0; x < size_by_x; x++)
				{
					map[y, x] = -1;
				}
			}
		}

		public int NewSnack(int x, int y, bool autoset = true)
		{
			int snack;

			while (true)
			{
				snack = random.Next(UniqueSnacksCount);

				if (x != 0 && map[y, x-1] == snack)
					continue;

				if (x != size_by_x - 1 && map[y, x+1] == snack)
					continue;

				if (y != 0 && map[y-1, x] == snack)
					continue;

				if (y != size_by_y - 1 && map[y+1, x] == snack)
					continue;

				break;
			}

			map[y, x] = autoset ? snack : map[y, x];
			return snack;
		}

		public void NewMap()
		{
			for (int y = 0; y < size_by_y; y++)
			{
				for (int x = 0; x < size_by_x; x++)
				{
					NewSnack(x, y);
				}
			}
		}

		public void UpdateMap()
		{
			for (int y = 0; y < size_by_y; y++)
			{
				for (int x = 0; x < size_by_x; x++)
				{
					if (map[y, x] == -1)
						//NewSnack(x, y);
						map[y, x] = random.Next(UniqueSnacksCount);
				}
			}
		}
	}
}