namespace Ascii
{
    public class Inventory
    {
        private GameState _state;

        public Inventory(GameState state)
        {
            _state = state;
        }

        public void TryToPickUp()
        {
            if (_state.PlayerCurrentlyHolding != '.')
            {
                return;
            }

            Coord nextCoord = _state.FigureOutNextMapCoord();
            if (_state.ReadMap(nextCoord) != '.')
            {
                _state.PlayerCurrentlyHolding = (char)_state.ReadMap(nextCoord);
                _state.SetMap(nextCoord,'.');
            }
        }

        public void TryToPutDown()
        {
            if (_state.PlayerCurrentlyHolding == '.')
            {
                return;
            }
            Coord nextCoord = _state.FigureOutNextMapCoord();
            if (_state.ReadMap(nextCoord) == '.')
            {
                _state.SetMap(nextCoord, _state.PlayerCurrentlyHolding);
                _state.PlayerCurrentlyHolding = '.';
            }
        }
    }
}