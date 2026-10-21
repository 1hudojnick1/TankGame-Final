
using System;
using System.Collections.Generic;

public enum TankDirection
{
    Up,
    Down,
    Left,
    Right
}

public class PlayerTank
{
    private GameMap map;

    public int X { get; private set; }
    public int Y { get; private set; }

    public TankDirection Direction { get; private set; }

    public PlayerTank(GameMap gameMap)
    {
        map = gameMap;

        X = map.Width / 2;
        Y = map.Height / 2;

        Direction = TankDirection.Up;
    }

    public void Move(
        ConsoleKey key,
        List<EnemyTank>? enemies = null)
    {
        int newX = X;
        int newY = Y;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                Direction = TankDirection.Up;
                newY--;
                break;

            case ConsoleKey.DownArrow:
                Direction = TankDirection.Down;
                newY++;
                break;

            case ConsoleKey.LeftArrow:
                Direction = TankDirection.Left;
                newX--;
                break;

            case ConsoleKey.RightArrow:
                Direction = TankDirection.Right;
                newX++;
                break;

            default:
                return;
        }

        if (map.GetTile(newX, newY) != TileType.Empty)
            return;

        if (enemies != null)
        {
            foreach (EnemyTank enemy in enemies)
            {
                if (!enemy.IsAlive)
                    continue;

                if (enemy.X == newX && enemy.Y == newY)
                    return;
            }
        }

        X = newX;
        Y = newY;
    }
}
