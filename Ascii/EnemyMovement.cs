using System;
using System.Linq;

namespace Ascii
{
    public class EnemyMovement
    {
        private GameState _state;
        private Movement _movement;
        private SoundManager _soundManager;
        private AudioPlaybackEngine _audioPlaybackEngine;

        public EnemyMovement(GameState state, Movement movement, SoundManager soundManager, AudioPlaybackEngine audioPlaybackEngine)
        {
            _state = state;
            _movement = movement;
            _soundManager = soundManager;
            _audioPlaybackEngine = audioPlaybackEngine;
        }

        public void HandleEnemyMovements(double elapsed)
        {
            //oppdag om vi ser spiller, i så fall gå mot spiller
            //ellers gå random

            foreach (var mob in _state.Mobs)
            {
                _movement.HandleRandomMovementForMob(mob, elapsed);
            }


        }

        public void HandleEnemyCollisions()
        {
            foreach (var mob in _state.Mobs.ToList())
            {
                if (mob.Coord.IsNear(_state.Player.Coord, 2))
                {
                    var sound = "chicken-1";
                    if (mob.MobType == 'b')
                    {
                        sound = "babe";
                    }

                    var s = _soundManager.createSoundInstance(sound);
                    s.LoopingEnabled = false;
                    _audioPlaybackEngine.PlaySoundInstance(s);
                    //_audioPlaybackEngine.StopSoundInstance(s);
                    _state.Mobs.Remove(mob);
                }


            }
        }
    }
}