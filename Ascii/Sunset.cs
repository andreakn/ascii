using System;
using System.Windows.Forms;

namespace Ascii
{
    public class Sunset
    {
        public Sunset(GameState state)
        {
            State = state;
        }

        public GameState State { get; set; }
        public const int R = 238;
        public const int G = 39;
        public const int B = 50;

        public void CalculateTimeOfDay()
        {
            var time = DateTime.Now - State.StartTime;
            var levelTime = TimeSpan.FromMinutes(Math.PI);
            var input = time.TotalMinutes;
            var factor = Math.Abs(Math.Sin(time.TotalMinutes));
            State.SunHeight = factor;

            if (time > levelTime)
            {
                State.NightHasFallen = true;
            }

            State.SunlightColor = $"#{Hex(R,factor)}{Hex(G, factor)}{Hex(B, factor)}";


        }

        private string Hex(int color, double factor)
        {
            return ((int)(color*factor)).ToString("X2");
        }
    }
}