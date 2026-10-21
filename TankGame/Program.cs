
using System;
using System.Collections.Generic;
using System.Threading;

public class Program
{
    private static Random random = new Random();

    public static void Main()
    {
        MapGenerator generator = new MapGenerator();
        MapRenderer renderer = new MapRenderer();

        int level = 1;

        GameMap map = generator.Generate(30, 20);
        PlayerTank player = new PlayerTank(map);

        List<Projectile> projectiles = new List<Projectile>();
        List<EnemyTank> enemies = SpawnEnemies(map, player, 3);

        bool playerAlive = true;
        bool levelComplete = false;

        DateTime levelCompleteTime = DateTime.UtcNow;

        DateTime lastProjectileMove = DateTime.UtcNow;
        DateTime lastEnemyMove = DateTime.UtcNow;
        DateTime lastEnemyShot = DateTime.UtcNow;

        Console.CursorVisible = false;

        renderer.Draw(map, player, projectiles, enemies);
        ShowControls(playerAlive, levelComplete, level, enemies);

        while (true)
        {
            bool needRedraw = false;

            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                    break;

                if (key == ConsoleKey.R)
                {
                    level = 1;

                    map = generator.Generate(30, 20);
                    player = new PlayerTank(map);

                    projectiles.Clear();
                    enemies = SpawnEnemies(map, player, 3);

                    playerAlive = true;
                    levelComplete = false;

                    lastProjectileMove = DateTime.UtcNow;
                    lastEnemyMove = DateTime.UtcNow;
                    lastEnemyShot = DateTime.UtcNow;

                    needRedraw = true;
                }
                else if (playerAlive && !levelComplete)
                {
                    if (key == ConsoleKey.Spacebar ||
                        key == ConsoleKey.Enter)
                    {
                        Projectile projectile = new Projectile(
                            player.X,
                            player.Y,
                            player.Direction,
                            true
                        );

                        projectile.Move();

                        if (CheckProjectile(
                            projectile,
                            map,
                            enemies,
                            player,
                            ref playerAlive))
                        {
                            projectiles.Add(projectile);
                        }
                    }
                    else
                    {
                        player.Move(key, enemies);
                    }

                    needRedraw = true;
                }
            }

            DateTime now = DateTime.UtcNow;

            if (playerAlive && !levelComplete)
            {
                if ((now - lastProjectileMove).TotalMilliseconds >= 120)
                {
                    lastProjectileMove = now;

                    for (int i = projectiles.Count - 1; i >= 0; i--)
                    {
                        Projectile projectile = projectiles[i];

                        projectile.Move();

                        if (!CheckProjectile(
                            projectile,
                            map,
                            enemies,
                            player,
                            ref playerAlive))
                        {
                            projectiles.RemoveAt(i);
                        }
                    }

                    needRedraw = true;
                }

                if (playerAlive &&
                    (now - lastEnemyMove).TotalMilliseconds >= 500)
                {
                    lastEnemyMove = now;

                    foreach (EnemyTank enemy in enemies)
                    {
                        if (enemy.IsAlive)
                        {
                            enemy.Move(player, enemies);
                        }
                    }

                    needRedraw = true;
                }

                if (playerAlive &&
                    (now - lastEnemyShot).TotalMilliseconds >= 1200)
                {
                    lastEnemyShot = now;

                    foreach (EnemyTank enemy in enemies)
                    {
                        if (!enemy.IsAlive)
                            continue;

                        // Стреляем только при прямой видимости игрока
                        if (!enemy.TryAimAtPlayer(player))
                            continue;

                        Projectile projectile = enemy.Fire();

                        if (CheckProjectile(
                            projectile,
                            map,
                            enemies,
                            player,
                            ref playerAlive))
                        {
                            projectiles.Add(projectile);
                        }

                        if (!playerAlive)
                            break;
                    }

                    needRedraw = true;
                }

                if (playerAlive && AllEnemiesDestroyed(enemies))
                {
                    levelComplete = true;
                    levelCompleteTime = DateTime.UtcNow;

                    projectiles.Clear();

                    needRedraw = true;
                }
            }

            if (levelComplete &&
                (now - levelCompleteTime).TotalSeconds >= 2)
            {
                level++;

                map = generator.Generate(30, 20);
                player = new PlayerTank(map);

                projectiles.Clear();

                int enemyCount = level + 2;

                enemies = SpawnEnemies(map, player, enemyCount);

                playerAlive = true;
                levelComplete = false;

                lastProjectileMove = DateTime.UtcNow;
                lastEnemyMove = DateTime.UtcNow;
                lastEnemyShot = DateTime.UtcNow;

                needRedraw = true;
            }

            if (needRedraw)
            {
                renderer.Draw(map, player, projectiles, enemies);

                ShowControls(
                    playerAlive,
                    levelComplete,
                    level,
                    enemies
                );
            }

            Thread.Sleep(10);
        }

        Console.CursorVisible = true;
    }

    private static bool AllEnemiesDestroyed(List<EnemyTank> enemies)
    {
        foreach (EnemyTank enemy in enemies)
        {
            if (enemy.IsAlive)
                return false;
        }

        return true;
    }

    private static List<EnemyTank> SpawnEnemies(
        GameMap map,
        PlayerTank player,
        int count)
    {
        List<EnemyTank> enemies = new List<EnemyTank>();

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < 300; attempt++)
            {
                int x = random.Next(1, map.Width - 1);
                int y = random.Next(1, map.Height - 1);

                if (map.GetTile(x, y) != TileType.Empty)
                    continue;

                if (Math.Abs(x - player.X) +
                    Math.Abs(y - player.Y) < 6)
                    continue;

                bool occupied = false;

                foreach (EnemyTank enemy in enemies)
                {
                    if (enemy.X == x && enemy.Y == y)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (occupied)
                    continue;

                enemies.Add(new EnemyTank(map, x, y));
                break;
            }
        }

        return enemies;
    }

    private static bool CheckProjectile(
        Projectile projectile,
        GameMap map,
        List<EnemyTank> enemies,
        PlayerTank player,
        ref bool playerAlive)
    {
        if (!map.IsInside(projectile.X, projectile.Y))
        {
            projectile.Deactivate();
            return false;
        }

        TileType tile = map.GetTile(
            projectile.X,
            projectile.Y
        );

        if (tile == TileType.Wall)
        {
            map.DamageWall(
                projectile.X,
                projectile.Y
            );

            projectile.Deactivate();
            return false;
        }

        if (projectile.IsPlayerShot)
        {
            foreach (EnemyTank enemy in enemies)
            {
                if (!enemy.IsAlive)
                    continue;

                if (enemy.X == projectile.X &&
                    enemy.Y == projectile.Y)
                {
                    enemy.Destroy();
                    projectile.Deactivate();
                    return false;
                }
            }
        }
        else
        {
            if (playerAlive &&
                projectile.X == player.X &&
                projectile.Y == player.Y)
            {
                playerAlive = false;
                projectile.Deactivate();
                return false;
            }
        }

        return true;
    }

    private static void ShowControls(
        bool playerAlive,
        bool levelComplete,
        int level,
        List<EnemyTank> enemies)
    {
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("LEVEL: " + level);

        int remaining = 0;

        foreach (EnemyTank enemy in enemies)
        {
            if (enemy.IsAlive)
                remaining++;
        }

        Console.WriteLine("ENEMIES LEFT: " + remaining);
        Console.ResetColor();

        if (!playerAlive)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("GAME OVER");
            Console.ResetColor();

            Console.WriteLine("R - Restart");
        }
        else if (levelComplete)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("LEVEL COMPLETE!");
            Console.ResetColor();

            Console.WriteLine("Next level starting...");
        }
        else
        {
            Console.WriteLine("ARROWS - Move tank");
            Console.WriteLine("SPACE / ENTER - Fire");
            Console.WriteLine("R - Restart");
        }

        Console.WriteLine("ESC - Exit");
    }
}
