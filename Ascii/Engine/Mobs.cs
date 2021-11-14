using System;
using System.Linq;

namespace Ascii
{
    public class Mobs
    {
        private readonly GameState _state;
        private readonly Movement _movement;
        private readonly SoundManager _soundManager;
        private readonly AudioPlaybackEngine _audioPlaybackEngine;

        public Mobs(GameState state, Movement movement, SoundManager soundManager, AudioPlaybackEngine audioPlaybackEngine)
        {
            _state = state;
            _movement = movement;
            _soundManager = soundManager;
            _audioPlaybackEngine = audioPlaybackEngine;
        }

        public void HandleEnemyMovements(double elapsed)
        {
            foreach (var mob in _state.Mobs)
            {
                _movement.DoSemiRandomMove(mob, elapsed);
            }
        }

        public void HandleEnemyCollisions()
        {
            foreach (var mob in _state.Mobs.ToList())
            {
                if (mob.Coord.IsNear(_state.Player.Coord, 1.3))
                {
                    var s = _soundManager.CreateSoundInstance($"{mob.MobType}_collision");
                    _audioPlaybackEngine.PlaySoundInstance(s);
                    _state.Mobs.Remove(mob);
                }
            }
        }
    }
}