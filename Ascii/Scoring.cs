using System.Collections.Generic;
using System.IO;

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
            var left = File.ReadAllLines($"Numbers/{_state.Mobs.Count}.txt");
            var total = File.ReadAllLines($"Numbers/{_state.InitialMobCount}.txt");
            var of = File.ReadAllLines($"Numbers/of.txt");

            var lines = new List<string>();
            for (int i = 0; i < 8; i++)
            {

            }
        }
    }
}