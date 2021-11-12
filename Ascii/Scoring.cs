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
            if (_state.ReadMap(_state.PlayerCoord) == 'o')
            {
                _state.Redness = 0;
                _state.SetMap(_state.PlayerCoord, '.');
            }


        }
    }
}