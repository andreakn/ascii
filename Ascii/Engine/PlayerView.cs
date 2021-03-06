using System;
using System.Collections.Generic;
using System.Linq;

namespace Ascii
{
    public class PlayerView
    {
        private bool enhanceEdges = false;
        public PlayerView(GameState state)
        {
            this.State = state;
            enhanceEdges = true;
        }

        public GameState State { get; set; }


        //calculate current view through eyes of player (one column at a time horizontally)
        public void RenderViewToScreenBuffer()
        {
            var screenBuffer = State.ScreenBuffer;
            var screenWidth = State.ScreenWidth;
            var screenHeight = State.ScreenHeight;
            var renderDepth = State.RenderDepth;
            for (int x = 0; x < screenWidth; x++)
            {
                double rayAngle = (State.Player.ViewAngle - (State.Player.FOV / 2.0)) + (x * State.Player.FOV / (double)screenWidth);
                var rayResolution = 0.1;
                var distanceToWall = 0.0;

                var wallHit = false;
                var edgeHit = false;
                var eyeX = Math.Sin(rayAngle);
                var eyeY = Math.Cos(rayAngle);

                while (!wallHit && distanceToWall < State.RenderDepth)
                {
                    distanceToWall += rayResolution;
                    var rayTestX = (int)(State.Player.Coord.X + (eyeX * distanceToWall));
                    var rayTestY = (int)(State.Player.Coord.Y + (eyeY * distanceToWall));
                    if (rayTestX < 0 || rayTestX >= State.MapWidth || rayTestY < 0 || rayTestY >= State.MapHeight)
                    {
                        wallHit = true; //Nothing more to see here
                        distanceToWall = State.RenderDepth;
                    }
                    else
                    {
                        if (State.Map[rayTestY][rayTestX] == '#')
                        {
                            wallHit = true;
                            List<Tuple<double, double>> p = new List<Tuple<double, double>>();

                            for (var tx = 0; tx < 2; tx++)
                            {
                                for (var ty = 0; ty < 2; ty++)
                                {
                                    var vy = rayTestY + ty - State.Player.Coord.Y;
                                    var vx = rayTestX + tx - State.Player.Coord.X;
                                    var d = Math.Sqrt(vx * vx + vy * vy);
                                    var dot = (eyeX * vx / d) + (eyeY * vy) / d;
                                    p.Add(new Tuple<double, double>(d,dot));
                                }
                            }

                            if (enhanceEdges)
                            {
                                var sortedP = p.OrderBy(t => t.Item1).ToList();
                                double bound = 0.005;
                                if (Math.Acos(sortedP[0].Item2) < bound) edgeHit = true;
                                if (Math.Acos(sortedP[1].Item2) < bound) edgeHit = true;
                                if (Math.Acos(sortedP[2].Item2) < bound) edgeHit = true;
                            }
                        }
                    }
                }

                var ceiling = (int)(((double)screenHeight / 2.0) - ((double)screenHeight / distanceToWall));
                var floor = screenHeight - ceiling;
                char nShade = ' ';
                if (distanceToWall <= renderDepth / 4.0f) nShade = (char)0x2588;     //veldig nær	
                else if (distanceToWall < renderDepth / 3.0f) nShade = (char)0x2593; //nær
                else if (distanceToWall < renderDepth / 2.0f) nShade = (char)0x2592; //litt nær
                else if (distanceToWall < renderDepth) nShade = (char)0x2591;        //et stykke unna
                else nShade = ' ';

                if (edgeHit) nShade = '|'; // Gjør kantene tydeligere

                for (int y = 0; y < screenHeight; y++)
                {
                    var buffercoord = State.SBP(x, y);
                    if (y <= ceiling)
                    {
                        screenBuffer[buffercoord] = ' ';
                    }
                    else if (y > ceiling && y <= floor)
                    {
                        screenBuffer[buffercoord] = nShade;
                    }
                    else
                    {
                        var b = 1.0 - ((double)y - screenHeight / 2.0) / (screenHeight / 2.0);
                        if (b < 0.25) screenBuffer[buffercoord] = '#';
                        else if (b < 0.5) screenBuffer[buffercoord] = 'x';
                        else if (b < 0.75) screenBuffer[buffercoord] = '.';
                        else if (b < 0.9) screenBuffer[buffercoord] = '-';
                        else screenBuffer[buffercoord] = ' ';
                    }
                }
            }
        }
    }
}