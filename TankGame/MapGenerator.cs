
using System;

public class MapGenerator
{
    private Random random = new Random();

    public GameMap Generate(int width, int height)
    {
        GameMap map = new GameMap(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || y == 0 ||
                    x == width - 1 || y == height - 1)
                {
                    map.SetTile(x, y, TileType.Wall);
                    map.SetWallHealth(x, y, 3);
                    continue;
                }

                int chance = random.Next(100);

                if (chance < 15)
                {
                    map.SetTile(x, y, TileType.Wall);

                    int health = random.Next(1, 4);

                    map.SetWallHealth(x, y, health);
                }
                else if (chance < 25)
                {
                    map.SetTile(x, y, TileType.Water);
                }
                else
                {
                    map.SetTile(x, y, TileType.Empty);
                }
            }
        }

        // Создаём горизонтальный проход
        for (int x = 1; x < width - 1; x++)
        {
            map.SetTile(
                x,
                height / 2,
                TileType.Empty
            );
        }

        // Создаём вертикальный проход
        for (int y = 1; y < height - 1; y++)
        {
            map.SetTile(
                width / 2,
                y,
                TileType.Empty
            );
        }

        return map;
    }
}
