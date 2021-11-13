using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Ascii
{
    public class Scoring
    {
        private GameState _state;

        public Scoring(GameState state)
        {
            this._state = state;
        }


        public void DisplayScore()
        {
            var left = File.ReadAllLines($"Numbers/{_state.InitialMobCount - _state.Mobs.Count}.txt");
            var total = File.ReadAllLines($"Numbers/{_state.InitialMobCount}.txt");
            var of = File.ReadAllLines($"Numbers/of.txt");

            var lines = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                lines.Add($"{left[i]} {of[i]} {total[i]}");
            }

            var startY = 10;
            var startX = _state.ScreenWidth - 10 - lines.Max(x => x.Length);

            for(int i = 0; i<lines.Count; i++)
            {
                Console.SetCursorPosition(startX,startY+i);
                Console.Write(lines[i].Pastel(Color.BurlyWood));
            }
        }
    }
}