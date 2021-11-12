namespace Ascii
{
    public struct Coord
    {
        public double X { get; set; }
        public double Y { get; set; }

        public bool Schmequals(Coord other)
        {
            return ((int) X == (int) other.X) && ((int) Y == (int) other.Y);
        }
    }
}