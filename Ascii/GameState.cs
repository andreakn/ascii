using System.Collections.Generic;
using System.Linq;

namespace Ascii
{
    public class GameState
    {
        private char[][] _map;
        public char[] ScreenBuffer { get; set; }
        public double FOV { get; set; }= 3.14159f / 4.0f;   // Field of View
        public double RenderDepth { get; set; }= 16.0f;           // Maximum rendering distance




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

       
        public double PlayerViewAngle { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight{ get; set; }
        public int Redness { get; set; }
        public List<Coord> LazerMapCoords { get; set; } = new List<Coord>();
        public List<LazerVector> LazerVectors { get; set; } = new List<LazerVector>();
        public Coord PlayerCoord { get; set; } = new Coord();
        public int PlayerNumber { get; set; }


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
                        PlayerCoord = new Coord {X = x, Y = y};
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
            return ReadMap(coord) != '#';
        }
    }
}