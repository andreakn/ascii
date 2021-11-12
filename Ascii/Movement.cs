using System;
using System.Text;

namespace Ascii
{
    public class Movement
    {
        private GameState _state;
        private readonly Inventory _inventory;
        private Random random = new Random();
        private double turningspeed = 2;

        public Movement(GameState state, Inventory inventory)
        {
            _state = state;
            _inventory = inventory;
        }


        public void HandleMovementForPlayer(double elapsed)
        {
            var inputs = ReadInputs();
            HandleMovementFor(_state.Player, inputs, elapsed);
            
        }

        public void HandleRandomMovementForMob(Mob mob, double elapsed)
        {
            var inputs = GenerateRandomInput(mob);
            HandleMovementFor(mob, inputs, elapsed);
        }

        private string GenerateRandomInput(Mob mob)
        {
            var ret = new StringBuilder();

            if (random.Next(100) > 50)
            {
                return mob.prevInput;
            }

            if (random.Next(100) > 40)
            {
                ret.Append("u");
            }
            if (random.Next(100) > 60)
            {
                ret.Append("l");
            }
            else if (random.Next(100) > 60)
            {
                ret.Append("r");
            }

            mob.prevInput = ret.ToString();
            return mob.prevInput;
        }

        private void HandleMovementFor(Entity entity, string inputs, double elapsed)
        {
            var deltaX = 0.0;
            var deltaY = 0.0;

            var wasSafe = _state.ReadMap(entity.Coord) != '#';
            if (inputs.Contains("L"))
            {
                deltaX -= Math.Cos(entity.ViewAngle) * entity.Speed * elapsed;
                deltaY += Math.Sin(entity.ViewAngle) * entity.Speed * elapsed;
            }
            else if (inputs.Contains("l"))
            {
                entity.ViewAngle -= (turningspeed * 0.75) * elapsed;
            }

            if (inputs.Contains("R"))
            {
                deltaX += Math.Cos(entity.ViewAngle) * entity.Speed * elapsed;
                deltaY -= Math.Sin(entity.ViewAngle) * entity.Speed * elapsed;
            }
            else if (inputs.Contains("r"))
            {
                entity.ViewAngle += (turningspeed * 0.75) * elapsed;
            }

            if (inputs.Contains("u"))
            {
                deltaX += Math.Sin(entity.ViewAngle) * entity.Speed * elapsed;
                deltaY += Math.Cos(entity.ViewAngle) * entity.Speed * elapsed;
            }

            if (inputs.Contains("d"))
            {
                deltaX -= Math.Sin(entity.ViewAngle) * entity.Speed * elapsed;
                deltaY -= Math.Cos(entity.ViewAngle) * entity.Speed * elapsed;
            }

            var newCoord = new Coord
            {
                X = entity.Coord.X + deltaX,
                Y = entity.Coord.Y + deltaY
            };
            var newCoordXOnly = new Coord
            {
                X = entity.Coord.X + deltaX,
                Y = entity.Coord.Y
            };
            var newCoordYOnly = new Coord
            {
                X = entity.Coord.X,
                Y = entity.Coord.Y + deltaY
            };
            if (_state.IsValidPlayerPosition(newCoord))
            {
                entity.Coord = newCoord;
            }
            else if (_state.IsValidPlayerPosition(newCoordXOnly))
            {
                entity.Coord = newCoordXOnly;
            }
            else if (_state.IsValidPlayerPosition(newCoordYOnly))
            {
                entity.Coord = newCoordYOnly;
            }
        }

        private string ReadInputs()
        {
            var ret = new StringBuilder();
            if (NativeKeyboard.IsKeyDown(KeyCode.Ctrl) && NativeKeyboard.IsKeyDown(KeyCode.Left)) ret.Append("L");
            else if (NativeKeyboard.IsKeyDown(KeyCode.Left)) ret.Append("l");
            if (NativeKeyboard.IsKeyDown(KeyCode.Ctrl) && NativeKeyboard.IsKeyDown(KeyCode.Right)) ret.Append("R");
            else if (NativeKeyboard.IsKeyDown(KeyCode.Right)) ret.Append("r");
            if (NativeKeyboard.IsKeyDown(KeyCode.Up)) ret.Append("u");
            if (NativeKeyboard.IsKeyDown(KeyCode.Down)) ret.Append("d");
            return ret.ToString();
        }

        public void TryToGoThisWay(Mob mob, double angleTowardsPlayer, double elapsed)
        {
            if (angleTowardsPlayer < mob.ViewAngle)
            {
                var inputs = "ur";
                HandleMovementFor(mob, inputs, elapsed);
            }
            else
            {
                var inputs = "lr";
                HandleMovementFor(mob, inputs, elapsed);
            }
        }
    }
}