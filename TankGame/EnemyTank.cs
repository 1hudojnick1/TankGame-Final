
using System;
using System.Collections.Generic;

public class EnemyTank
{
    private GameMap map;

    private static Random random = new Random();

    public int X { get; private set; }
    public int Y { get; private set; }

    public TankDirection Direction { get; private set; }

    public bool IsAlive { get; private set; }

    public EnemyTank(GameMap gameMap, int x, int y)
    {
        map = gameMap;

        X = x;
        Y = y;

        Direction = TankDirection.Down;

        IsAlive = true;
    }

    public void Move(
        PlayerTank? player = null,
        List<EnemyTank>? enemies = null)
    {
        if (!IsAlive)
            return;

        Direction = (TankDirection)random.Next(4);

        int newX = X;
        int newY = Y;

        switch (Direction)
        {
            case TankDirection.Up:
                newY--;
                break;

            case TankDirection.Down:
                newY++;
                break;

            case TankDirection.Left:
                newX--;
                break;

            case TankDirection.Right:
                newX++;
                break;
        }

        if (map.GetTile(newX, newY) != TileType.Empty)
            return;

        // Проверяем столкновение с игроком
        if (player != null &&
            player.X == newX &&
            player.Y == newY)
        {
            return;
        }

        // Проверяем столкновение с другими врагами
        if (enemies != null)
        {
            foreach (EnemyTank enemy in enemies)
            {
                if (enemy == this || !enemy.IsAlive)
                    continue;

                if (enemy.X == newX &&
                    enemy.Y == newY)
                {
                    return;
                }
            }
        }

        X = newX;
        Y = newY;
    }

    public bool TryAimAtPlayer(PlayerTank player)
    {
        if (!IsAlive)
            return false;

        TankDirection targetDirection;

        if (X == player.X && Y != player.Y)
        {
            targetDirection = player.Y < Y
                ? TankDirection.Up
                : TankDirection.Down;
        }
        else if (Y == player.Y && X != player.X)
        {
            targetDirection = player.X < X
                ? TankDirection.Left
                : TankDirection.Right;
        }
        else
        {
            return false;
        }

        int stepX = 0;
        int stepY = 0;

        switch (targetDirection)
        {
            case TankDirection.Up:
                stepY = -1;
                break;

            case TankDirection.Down:
                stepY = 1;
                break;

            case TankDirection.Left:
                stepX = -1;
                break;

            case TankDirection.Right:
                stepX = 1;
                break;
        }

        int checkX = X + stepX;
        int checkY = Y + stepY;

        while (checkX != player.X || checkY != player.Y)
        {
            if (map.GetTile(checkX, checkY) == TileType.Wall)
                return false;

            checkX += stepX;
            checkY += stepY;
        }

        Direction = targetDirection;

        return true;
    }

    public Projectile Fire()
    {
        Projectile projectile = new Projectile(
            X,
            Y,
            Direction,
            false
        );

        projectile.Move();

        return projectile;
    }

    public void Destroy()
    {
        IsAlive = false;
    }
}
