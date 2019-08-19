namespace Logy.Maps.Projections.Healpix
{
    public enum NeighborVert
    {
        North = 0,
        South = 1,
    }
    public enum NeighborHor
    {
        West = 0,
        East = 1,
    }

    public enum Direction
    {
        Nw = NeighborHor.West + (NeighborVert.North << 1), // 0
        Ne = NeighborHor.East + (NeighborVert.North << 1), // 1
        Sw = NeighborHor.West + (NeighborVert.South << 1), // 2
        Se = NeighborHor.East + (NeighborVert.South << 1), // 3
    }

    public enum Compass
    {
        West = 0,
        North = 1,
        South = 2,
        East = 3,
    }
}