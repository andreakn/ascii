namespace Ascii
{
    public class Entity
    {
        public Coord Coord { get; set; }
        public double ViewAngle { get; set; }
        public double FOV { get; set; }

        public int Speed { get; set; } = 7;
    }
}