﻿using System;

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

        public bool IsNear(Coord other, int delta)
        {
            return Math.Abs(other.X - X) < delta && Math.Abs(other.Y - Y) < delta;
        }
    }
}