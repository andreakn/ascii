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
        private readonly GameState _state;

        public Scoring(GameState state)
        {
            this._state = state;
        }


        public void DisplayScore()
        {
            var leftNumbers = (_state.InitialMobCount - _state.Mobs.Count).ToString().ToCharArray().Select(ln=>File.ReadAllLines($"Numbers/{ln}.txt")).ToList();
            var totalNumbers = _state.InitialMobCount.ToString().ToCharArray().Select(tn => File.ReadAllLines($"Numbers/{tn}.txt"));
            var left = new List<string>();
            var total = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                var i1 = i;
                left.Add(string.Join(" ",leftNumbers.Select(ln=>ln[i1])));
                total.Add(string.Join(" ", totalNumbers.Select(tn=>tn[i1])));
            }

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