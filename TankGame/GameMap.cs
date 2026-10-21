
public class GameMap
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private TileType[,] tiles;
    private int[,] wallHealth;

    public GameMap(int width, int height)
    {
        Width = width;
        Height = height;

        tiles = new TileType[width, height];
        wallHealth = new int[width, height];
    }

    public TileType GetTile(int x, int y)
    {
        if (!IsInside(x, y))
            return TileType.Wall;

        return tiles[x, y];
    }

    public void SetTile(int x, int y, TileType type)
    {
        if (!IsInside(x, y))
            return;

        tiles[x, y] = type;

        if (type == TileType.Wall)
        {
            wallHealth[x, y] = 2;
        }
        else
        {
            wallHealth[x, y] = 0;
        }
    }

    public int GetWallHealth(int x, int y)
    {
        if (!IsInside(x, y))
            return 0;

        return wallHealth[x, y];
    }

    public void SetWallHealth(int x, int y, int health)
    {
        if (GetTile(x, y) != TileType.Wall)
            return;

        wallHealth[x, y] = health;
    }

    public void DamageWall(int x, int y)
    {
        if (GetTile(x, y) != TileType.Wall)
            return;

        wallHealth[x, y]--;

        if (wallHealth[x, y] <= 0)
        {
            SetTile(x, y, TileType.Empty);
        }
    }

    public bool IsInside(int x, int y)
    {
        return x >= 0 &&
               x < Width &&
               y >= 0 &&
               y < Height;
    }
}
