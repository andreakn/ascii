﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Ascii
{
    public class Map
    {

        private Random random = new Random();

        private bool mPressed = false;
        private bool showMap = false;


        private int SBP(int x, int y)
        {
            return x + (State.ScreenWidth + 1) * y;
        }


        public void RenderMapToScreenBuffer()
        {
            int mapHeight = State.Map.Length;
            int mapWidth = State.Map[0].Length;             // World Dimensions
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    var le_x = (mapWidth - 1) - x;
                    var b = SBP(x, y + 1);
                    State.ScreenBuffer[b] = State.Map[y][le_x];

                    if (State.LazerMapCoords.Contains(new Coord { X = le_x, Y = y }))
                    {
                        State.ScreenBuffer[b] = '-';
                    }

                    foreach (var mob in State.Mobs)
                    {
                        if (le_x == (int)mob.Coord.X && y == (int)mob.Coord.Y)
                        {
                            State.ScreenBuffer[b] = 'X';
                        }
                    }

                    if (le_x == (int)State.Player.Coord.X && y == (int)State.Player.Coord.Y)
                    {
                        State.ScreenBuffer[b] = CalculatePlayerChar(State.Player.ViewAngle);
                    }

                }
            }
        }

        private char CalculatePlayerChar(double pAngle)
        {
            return Map.CalculateArrow(pAngle);
        }


        private static char[] _directions = new[] { 'V', '└', '<', '┌', '^', '┐', '>', '┘', };
        private static double _directionIncrement = 2.0 * Math.PI / _directions.Length;

        public Map(GameState state)
        {
            this.State = state;
        }

        public GameState State { get; set; }

        public static char CalculateArrow(double a)
        {
            var o = 2.0 * Math.PI;
            while (a < 0)
            {
                a = a + 2.0 * Math.PI;
            }
            var angle = a % (o);
            var start = o - _directionIncrement / 2.0;
            if (angle < _directionIncrement / 2.0 || angle >= (o - _directionIncrement / 2.0))
            {
                return _directions[0];
            }

            for (int i = 1; i < _directions.Length; i++)
            {
                if (angle > (start + ((double)i * _directionIncrement)) % o && angle <= (start + (((double)i + 1) * _directionIncrement)) % o)
                {
                    return _directions[i];
                }
            }
            return 'O';
            //            if(pAngle > Math.PI / 4.0)

        }


        public void ShowMapIfAppropriate()
        {
            var m = NativeKeyboard.IsKeyDown(KeyCode.M);
            if (m && !mPressed)
            {
                mPressed = true;
            }
            else if (!m && mPressed)
            {
                mPressed = false;
                showMap = !showMap;
            }

            if (showMap)
            {
                RenderMapToScreenBuffer();
            }


        }

        
    }
}