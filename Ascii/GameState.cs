using System;
using System.Collections.Generic;
using System.Linq;

namespace Ascii
{
    public class Entity
    {
        public Coord Coord { get; set; }
        public double ViewAngle { get; set; }
        public double FOV { get; set; }

        public int Speed { get; set; } = 7;
    }

    public class Player : Entity
    {
        public Player()
        {
            Coord = new Coord
            {
                X = 20,
                Y = 20
            };
            FOV = 3.14159f / 4.0f;
        }
    }

    public class Mob: Entity
    {
        private double RenderDepth = 7;
        public  string prevInput { get; set; }= "";

        public Mob()
        {
            Speed = 1;
        }

        public double? CanSeePlayerAtAngle(Player statePlayer, GameState state)
        {
            double? foundAt = null;
            var testAngle = 0.0;
            
            while (foundAt == null && testAngle < Math.PI * 2)
            {
                foundAt = RayTrace(state, Coord, testAngle, statePlayer.Coord);
                testAngle += 0.2;
            }
            return null;
        }


        private double? RayTrace(GameState state, Coord fromCoord, double rayAngle, Coord targetCoord)
        {
            var rayResolution = 0.1;
            var distanceToWall = 0.0;

            var xFactor = Math.Sin(rayAngle);
            var yFactor = Math.Cos(rayAngle);

            var rayEndX = 0;
            var rayEndY = 0;

            while (distanceToWall < RenderDepth)
            {
                distanceToWall += rayResolution;
                rayEndX = (int)(fromCoord.X + (xFactor * distanceToWall));
                rayEndY = (int)(fromCoord.Y + (yFactor * distanceToWall));

                if (rayEndY == (int)targetCoord.Y && rayEndX == (int)targetCoord.X)
                {
                    return rayAngle;
                }
               
                if (rayEndX < 0 || rayEndX >= state.MapWidth || rayEndY < 0 || rayEndY >= state.MapHeight)
                {
                    break;
                }
                if (state.Map[rayEndY][rayEndX] == '#')
                {
                    break;
                }
            }

            return null;
        }

        public bool IsNear(double x, double y)
        {
            return Math.Abs(Coord.X - x) <= 0.5
                   && Math.Abs(Coord.Y - y) <= 0.5;
        }
    }

  
    public class GameState
    {
        private char[][] _map;
        public char[] ScreenBuffer { get; set; }
        public double RenderDepth { get; set; }= 16.0f;           // Maximum rendering distance

        public List<Mob> Mobs { get; set; } = new List<Mob>();
        public Player Player { get; set; } = new Player();


        public void RecalculateScreenBuffer()
        {
            if (ScreenBuffer.Length != ScreenHeight * (ScreenWidth + 1))
            {
                ScreenBuffer = Enumerable.Repeat('\n', ScreenHeight * (ScreenWidth + 1)).ToArray();
            }
        }

        public int ScreenHeight{ get; set; }
        public int ScreenWidth { get; set; }

        public char[][] Map
        {
            get => _map;
            set
            {
                _map = value;
                MapHeight = _map.Length;
                MapWidth = _map[0].Length;
            }
        }

       
        public int MapWidth { get; set; }
        public int MapHeight{ get; set; }
        //public int Redness { get; set; }
        public List<Coord> LazerMapCoords { get; set; } = new List<Coord>();
        public List<LazerVector> LazerVectors { get; set; } = new List<LazerVector>();
        public char PlayerCurrentlyHolding { get; set; } = '.';


        public int SBP(int x, int y)
        {
            return x + (ScreenWidth + 1) * y;
        }

        public void ReadStartingPositionFromMap(int playerNumber)
        {
            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    if (Map[y][x] == playerNumber.ToString()[0])
                    {
                        Player.Coord = new Coord {X = x, Y = y};
                        Map[y][x] = '.';
                    }
                    else if ("123456789".Contains(Map[y][x]))
                    {
                        Map[y][x] = '.';
                    }
                }
            }
        }

        public int ReadMap(Coord coord)
        {
            return Map[(int) coord.Y][(int) coord.X];
        }

        public void SetMap(Coord coord, char c)
        {
            Map[(int) coord.Y][(int) coord.X] = c;
        }

        public bool IsValidPlayerPosition(Coord coord)
        {
            if (Map.Length > coord.Y && coord.Y >= 0)
            {
                if(Map[(int)coord.Y].Length > coord.X && coord.X >= 0)
                {
                    return ReadMap(coord) != '#';
                }
            }

            return false;
        }

        public Coord FigureOutNextMapCoord()
        {

            var rayResolution = 0.1;
            var distance = 0.0;

            var xFactor = Math.Sin(Player.ViewAngle);
            var yFactor = Math.Cos(Player.ViewAngle);

            var rayEndX = Player.Coord.X;
            var rayEndY = Player.Coord.Y;

            while ( (int)rayEndX==(int)Player.Coord.X && (int)rayEndY==(int)Player.Coord.Y)
            {
                distance += rayResolution;
                rayEndX = (int)(Player.Coord.X + (xFactor * distance));
                rayEndY = (int)(Player.Coord.Y + (yFactor * distance));
            }

            return new Coord { X = rayEndX, Y = rayEndY };

        }
    }
}