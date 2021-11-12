using System.Runtime.Intrinsics.X86;

namespace Ascii
{
    public class Helpers
    {



        public static Coord? get_line_intersection(Coord aFrom, Coord aTo, Coord bFrom, Coord bTo)
        {
            double p0_x = aFrom.X;
            double p0_y = aFrom.Y;
            
            double p1_x = aTo.X;
            double p1_y = aTo.Y;
            double p2_x = bFrom.X;
            double p2_y = bFrom.X;
            double p3_x = bTo.X;
            double p3_y = bTo.Y;

            double s1_x, s1_y, s2_x, s2_y;
            s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
            s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

            double s, t;
            var det = (-s2_x * s1_y + s1_x * s2_y);
            if (det == 0)
            {
                return null;
            }
            s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / det;
            t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x))  / det;

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return new Coord {X = (int) (p0_x + (t * s1_x)), Y = (int) (p0_y + (t * s1_y))};
            }

            return null; // No collision
        }

    }
}