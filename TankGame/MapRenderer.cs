
using System;
using System.Collections.Generic;

public class MapRenderer
{
    public void Draw(
        GameMap map,
        PlayerTank? player = null,
        List<Projectile>? projectiles = null,
        List<EnemyTank>? enemies = null)
    {
        Console.Clear();

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                bool projectileDrawn = false;

                if (projectiles != null)
                {
                    foreach (Projectile projectile in projectiles)
                    {
                        if (projectile.IsActive &&
                            projectile.X == x &&
                            projectile.Y == y)
                        {
                            Console.ForegroundColor =
                                projectile.IsPlayerShot
                                ? ConsoleColor.Yellow
                                : ConsoleColor.Red;

                            Console.Write("**");

                            projectileDrawn = true;
                            break;
                        }
                    }
                }

                if (projectileDrawn)
                    continue;

                if (player != null &&
                    player.X == x &&
                    player.Y == y)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    DrawTank(player.Direction);

                    continue;
                }

                bool enemyDrawn = false;

                if (enemies != null)
                {
                    foreach (EnemyTank enemy in enemies)
                    {
                        if (enemy.IsAlive &&
                            enemy.X == x &&
                            enemy.Y == y)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                            DrawTank(enemy.Direction);

                            enemyDrawn = true;
                            break;
                        }
                    }
                }

                if (enemyDrawn)
                    continue;

                TileType tile = map.GetTile(x, y);

                switch (tile)
                {
                    case TileType.Wall:

                        int health = map.GetWallHealth(x, y);

                        if (health >= 3)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("##");
                        }
                        else if (health == 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("++");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("::");
                        }

                        break;

                    case TileType.Water:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("~~");
                        break;

                    case TileType.Empty:
                        Console.Write("  ");
                        break;
                }
            }

            Console.WriteLine();
        }

        Console.ResetColor();
    }

    private void DrawTank(TankDirection direction)
    {
        switch (direction)
        {
            case TankDirection.Up:
                Console.Write("^^");
                break;

            case TankDirection.Down:
                Console.Write("vv");
                break;

            case TankDirection.Left:
                Console.Write("<<");
                break;

            case TankDirection.Right:
                Console.Write(">>");
                break;
        }
    }
}
