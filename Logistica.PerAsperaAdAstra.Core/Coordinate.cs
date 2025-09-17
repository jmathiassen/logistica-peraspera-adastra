namespace LogisticaPerAsperaAdAstra.Core;

public readonly record struct Coordinate(int X, int Y, ushort Height = 0)
{
    public double PlaneDistanceSquaredTo(Coordinate other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        return dx * dx + dy * dy;
    }
    public double DistanceTo(Coordinate other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        int dz = Height - other.Height;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
    public Coordinate OffsetBy(int dx, int dy, int dz = 0) => new(X + dx, Y + dy, (ushort)Math.Clamp(Height + dz, 0, ushort.MaxValue));

    public override string ToString() => $"({X}, {Y}, {Height}m)";
}