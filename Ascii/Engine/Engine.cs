using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Ascii
{
    public class Engine
    {
        public GameState state;
        private Map theMap;

        public Engine(string gameRootFolder)
        {

            state = new GameState
            {
                GameRootFolder = gameRootFolder
            };
            state.ReadLevelData();
            theMap = new Map(state);
           
            _inventory = new Inventory(state);
            _movement = new Movement(state);
            _playerView = new PlayerView(state);
            _sprites = new Sprites(state);
            _tweak = new GuiTweaks(state);
            _scoring = new Scoring(state);
            _audioPlaybackEngine = new AudioPlaybackEngine(44100, 2);
            _soundManager = new SoundManager(state.GameRootFolder);
            _splash = new SplashScreen(state);
            _sunset = new Sunset(state);
            _enemyMovement = new Mobs(state, _movement, _soundManager, _audioPlaybackEngine);
        }

        private readonly Movement _movement;
        private readonly Sprites _sprites;
        private readonly PlayerView _playerView;
        private readonly GuiTweaks _tweak;
        private readonly Scoring _scoring;
        private readonly Inventory _inventory;
        private readonly Mobs _enemyMovement;
        private readonly SoundManager _soundManager;
        private static AudioPlaybackEngine _audioPlaybackEngine;
        private readonly Sunset _sunset;
        private readonly SplashScreen _splash;

        public void Run()
        {
            var prevTickStart = DateTime.Now;
            var thisTickStart = DateTime.Now;
            var level = 0;
            var levelFinished = 0;

            _audioPlaybackEngine.PlaySoundInstance(_soundManager.CreateSoundInstance("background",1,true));

            var showIntro = false;
            if (showIntro)
            {
                foreach (var introFile in Directory.GetFiles($"{state.GameRootFolder}/intro").OrderBy(x=>x))
                {
                    _splash.ShowFileInSplashScreen(introFile, true, 1500);
                }
            }
            _splash.ShowFileInSplashScreen($"{state.GameRootFolder}/logo.txt",true, 500, true);

            while (true)
            {
                state.AdjustToScreenToConsoleDimentions(); 

                if (levelFinished == level)
                {
                    var newLevel = state.InitializeNextLevel(level);
                    if (newLevel!=null)
                    {
                        level = newLevel.Number;
                        _splash.ShowSplashText(newLevel.Intro, true, 2000, false);
                    }
                    else
                    {
                        _splash.ShowFileInSplashScreen($"{state.GameRootFolder}/win.txt", true, 3000);
                        Console.ReadKey();
                        break;
                    }
                }
                
                if (HasWon())
                {
                    levelFinished++;
                    _splash.ShowSplashText(state.CurrentLevel.Outro, false, 2000, true);
                }
                else if (HasLost())
                {
                    _splash.ShowFileInSplashScreen($"{state.GameRootFolder}/lose.txt", true, 30000);
                    break;
                }
                
                state.RecalculateScreenBuffer();

                thisTickStart = DateTime.Now;
                var elapsed = (thisTickStart - prevTickStart).TotalSeconds;
                _movement.HandleMovementForPlayer(elapsed);
                _enemyMovement.HandleEnemyMovements(elapsed);
                _enemyMovement.HandleEnemyCollisions();

                _tweak.TweakGuiBasedOnUserInput();

                _playerView.RenderViewToScreenBuffer();
                _sprites.RenderSpritesToScreenBuffer();
                _sunset.CalculateTimeOfDay();

                theMap.ShowMapIfAppropriate();

                RenderScreenBufferToConsole();
                _scoring.DisplayScore();

                prevTickStart = thisTickStart; //prep for next tick
            }
        }

        private void RenderLevel(int level)
        {
            _splash.ShowFileInSplashScreen($"{state.GameRootFolder}/levels/{level}/intro.txt",true, 2000);
            Console.ReadKey();
        }

      

        
        

        private bool HasLost()
        {
            return state.NightHasFallen;
        }

        private bool HasWon()
        {
            return !state.Mobs.Any();
        }


        private void RenderScreenBufferToConsole()
        {
            Console.SetCursorPosition(0,0);
           var color = state.SunlightColor;
            
            Console.Write(new string(state.ScreenBuffer).Pastel(color));
            Console.WriteLine($"p:{state.Player.Coord.X}/{state.Player.Coord.Y} ({state.Player.ViewAngle})                                                            ".Pastel(Color.White));
        }

       


     

       

        



        

       
    }
}