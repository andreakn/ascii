using System;
using System.Linq;

namespace Ascii
{
    public class EnemyMovement
    {
        private GameState _state;
        private Movement _movement;

        public EnemyMovement(GameState state, Movement movement)
        {
            _state = state;
            _movement = movement;
        }

        public void HandleEnemyMovements(double elapsed)
        {
            //oppdag om vi ser spiller, i så fall gå mot spiller
            //ellers gå random

            foreach (var mob in _state.Mobs)
            {
                _movement.HandleRandomMovementForMob(mob, elapsed);
                continue;
                var relativeAngleTowardsPlayer = mob.CanSeePlayerAtAngle(_state.Player,_state);
                if (relativeAngleTowardsPlayer.HasValue)
                {
                    _movement.TryToGoThisWay(mob, relativeAngleTowardsPlayer.Value, elapsed);
                }
                else
                {
                    _movement.HandleRandomMovementForMob(mob, elapsed);
                }

            }


        }

        public void HandleEnemyCollisions()
        {
            foreach (var mob in _state.Mobs.ToList())
            {
                if (mob.Coord.IsNear(_state.Player.Coord, 1))
                {
                    try
                    {
                        Console.Beep(1000 -(50* _state.Mobs.Count), 200);
                    }
                    catch
                    {
                        Console.Beep();
                    }
                    _state.Mobs.Remove(mob);
                }


            }
        }
    }
}