namespace Ascii
{
    public class Scoring
    {
        private GameState _state;

        public Scoring(GameState state)
        {
            this._state = state;
        }


        public void CalculateScore()
        {
            if (_state.ReadMap(_state.Player.Coord) == 'o')
            {
                //_state.Redness = 0;
                _state.SetMap(_state.Player.Coord, '.');
            }


        }
    }
}