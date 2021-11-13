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
                X = 1,
                Y = 1
            };
            FOV = 3.14159f / 4.0f;
        }
    }

    public class Mob: Entity
    {
        public  string prevInput { get; set; }= "";
        public char MobType { get; set; }

        public Mob()
        {
            Speed = 1;
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
        public char[] ScreenBuffer { get; set; } = Array.Empty<char>();
        public double RenderDepth { get; set; }= 16.0f;           // Maximum rendering distance

        public List<Mob> Mobs { get; set; } = new List<Mob>();
        public Player Player { get; set; } = new Player();
        private Random random = new Random();
        public DateTime StartTime { get; set; }


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
        public bool NightHasFallen { get; set; }
        public double SunHeight { get; set; }
        public string SunlightColor { get; set; }
        public int InitialMobCount { get; set; }


        public int SBP(int x, int y)
        {
            return x + (ScreenWidth + 1) * y;
        }

        public void ReadStartingPositionFromMap()
        {
            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    if (Map[y][x] == 'p')
                    {
                        Player.Coord = new Coord {X = x, Y = y};
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

        public void Initialize(string mapString)
        {
            StartTime = DateTime.Now.AddSeconds(-30);
            NightHasFallen = false;
                
            Map = mapString.Replace("\r", "").Split("\n").Where(l=>!string.IsNullOrWhiteSpace(l)).Select(line => line.ToCharArray()).ToArray();
            ReadStartingPositionFromMap();
            Mobs.Clear();
            InitialMobCount = 0;
            foreach (var mob in ReadAndRemoveMobs())
            {
                InitialMobCount++;
                Mobs.Add(mob);
            }
        }


        public List<Mob> ReadAndRemoveMobs()
        {
            var ret = new List<Mob>();
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if (Map[y][x] == 'c' || Map[y][x] == 'b')
                    {
                        var mob = new Mob
                        {
                            Coord = new Coord
                            {
                                Y = y,
                                X = x
                            },
                            ViewAngle = random.NextDouble() * Math.PI * 2,
                            MobType = Map[y][x]
                        };
                        if (mob.MobType == 'b')
                        {
                            mob.Speed = 5;
                        }
                        ret.Add(mob);
                        Map[y][x] = '.';
                    }
                }
            }

            return ret;
        }
    }
}