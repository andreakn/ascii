using System;

namespace Ascii
{
    public class PlayerMovement
    {
        private GameState _state;

        private double speed = 5;
        private double turningspeed = 2;

        public PlayerMovement(GameState state)
        {
            _state = state;
        }


        public void HandleMovementForPlayer(double elapsed)
        {
            var deltaX = 0.0;
            var deltaY = 0.0;

            var wasSafe = _state.ReadMap(_state.PlayerCoord) != '#';
            if (NativeKeyboard.IsKeyDown(KeyCode.Ctrl) && NativeKeyboard.IsKeyDown(KeyCode.Left))
            {
                deltaX -= Math.Cos(_state.PlayerViewAngle) * speed * elapsed;
                deltaY += Math.Sin(_state.PlayerViewAngle) * speed * elapsed;
            }
            else if (NativeKeyboard.IsKeyDown(KeyCode.Left))
            {
                _state.PlayerViewAngle -= (turningspeed * 0.75) * elapsed;
            }

            if (NativeKeyboard.IsKeyDown(KeyCode.Ctrl) && NativeKeyboard.IsKeyDown(KeyCode.Right))
            {
                deltaX += Math.Cos(_state.PlayerViewAngle) * speed * elapsed;
                deltaY -= Math.Sin(_state.PlayerViewAngle) * speed * elapsed;
            }
            else if (NativeKeyboard.IsKeyDown(KeyCode.Right))
            {
                _state.PlayerViewAngle += (turningspeed * 0.75) * elapsed;
            }

            if (NativeKeyboard.IsKeyDown(KeyCode.Up))
            {
                deltaX += Math.Sin(_state.PlayerViewAngle) * speed * elapsed;
                deltaY += Math.Cos(_state.PlayerViewAngle) * speed * elapsed;
            }

            if (NativeKeyboard.IsKeyDown(KeyCode.Down))
            {
                deltaX -= Math.Sin(_state.PlayerViewAngle) * speed * elapsed;
                deltaY -= Math.Cos(_state.PlayerViewAngle) * speed * elapsed;
            }

            var newCoord = new Coord
            {
                X = _state.PlayerCoord.X + deltaX,
                Y = _state.PlayerCoord.Y + deltaY
            };
            var newCoordXOnly = new Coord
            {
                X = _state.PlayerCoord.X + deltaX,
                Y = _state.PlayerCoord.Y
            };
            var newCoordYOnly = new Coord
            {
                X = _state.PlayerCoord.X,
                Y = _state.PlayerCoord.Y + deltaY
            };
            if (_state.IsValidPlayerPosition(newCoord))
            {
                _state.PlayerCoord = newCoord;
            }
            else if (_state.IsValidPlayerPosition(newCoordXOnly))
            {
                _state.PlayerCoord = newCoordXOnly;
            }
            else if (_state.IsValidPlayerPosition(newCoordYOnly))
            {
                _state.PlayerCoord = newCoordYOnly;
            }




            if (NativeKeyboard.IsKeyDown(KeyCode.Spacebar))
            {
                _state.LazerVectors.Add(new LazerVector
                {
                    Coord = _state.PlayerCoord,
                    Angle = _state.PlayerViewAngle,
                    Ticks = DateTime.UtcNow.Ticks
                });
            }

        }

    }
}