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

        public void CalculateTimeOfDay()
        {
            var time = DateTime.Now - State.StartTime;
            var levelTime = TimeSpan.FromMinutes(Math.PI);
            var input = time.TotalMinutes;
            State.SunHeight = Math.Sin(input);

            if (time > levelTime)
            {
                State.NightHasFallen = true;
            }

            //State.SunlightColor = $"#{State.R}{}{}";


        }

    }
}