using System;
using System.Collections.Generic;
using System.Linq;

namespace Ascii
{

    public struct ABC
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
    }

    public class Lazer
    {
        public Lazer(GameState state)
        {
            State = state;
        }

        private GameState State { get; set; }


        private Coord RayTrace(Coord fromCoord, double rayAngle, List<Coord> coordBucket = null)
        {
            var rayResolution = 0.1;
            var distanceToWall = 0.0;

            var xFactor = Math.Sin(rayAngle);
            var yFactor = Math.Cos(rayAngle);

            var rayEndX = 0;
            var rayEndY = 0;

            while (distanceToWall < State.RenderDepth)
            {
                distanceToWall += rayResolution;
                rayEndX = (int)(fromCoord.X + (xFactor * distanceToWall));
                rayEndY = (int)(fromCoord.Y + (yFactor * distanceToWall));
                if (coordBucket != null && !coordBucket.Any(c => (int)c.X == rayEndX && (int)c.Y == rayEndY))
                {
                    coordBucket.Add(new Coord{X = rayEndX, Y = rayEndY});
                }
                if (rayEndX < 0 || rayEndX >= State.MapWidth || rayEndY < 0 || rayEndY >= State.MapHeight)
                {
                    break;
                }
                if (State.Map[rayEndY][rayEndX] == '#')
                {
                    break;
                }
            }

            return new Coord {X = rayEndX, Y = rayEndY};
        }

        private double GetDistanceBetween(double fromX, double fromY, double toX, double toY)
        {
            return Math.Sqrt((fromX - toX) * (fromX - toX) + (fromY - toY) * (fromY - toY));
        }


        public void DrawLazer()
        {

           

        }

    }
}