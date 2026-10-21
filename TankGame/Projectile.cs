
public class Projectile
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public TankDirection Direction { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsPlayerShot { get; private set; }

    public Projectile(int x, int y, TankDirection direction, bool isPlayerShot)
    {
        X = x;
        Y = y;
        Direction = direction;
        IsPlayerShot = isPlayerShot;
        IsActive = true;
    }

    public void Move()
    {
        if (!IsActive)
            return;

        switch (Direction)
        {
            case TankDirection.Up:
                Y--;
                break;

            case TankDirection.Down:
                Y++;
                break;

            case TankDirection.Left:
                X--;
                break;

            case TankDirection.Right:
                X++;
                break;
        }
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
