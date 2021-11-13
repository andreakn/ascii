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

        public Engine()
        {

            state = new GameState
            {
            };
            theMap = new Map(state);
           
            _inventory = new Inventory(state);
            _movement = new Movement(state, _inventory);
            _playerView = new PlayerView(state);
            _sprites = new Sprites(state);
            _tweak = new GuiTweaks(state);
            _scoring = new Scoring(state);
            _lazer = new Lazer(state);
            _audio = new Audio();
            _splash = new SplashScreen(state);
            _sunset = new Sunset(state);
            _enemyMovement = new EnemyMovement(state, _movement);
        }

        private readonly Movement _movement;
        private readonly Sprites _sprites;
        private readonly PlayerView _playerView;
        private readonly GuiTweaks _tweak;
        private readonly Scoring _scoring;
        private readonly Lazer _lazer;
        private readonly Inventory _inventory;
        private readonly EnemyMovement _enemyMovement;
        private readonly Audio _audio;
        private readonly Sunset _sunset;
        private readonly SplashScreen _splash;

        public async Task Run()
        {
            var t1 = DateTime.Now;
            var t2 = DateTime.Now;
            var counter = 0;
            var win = false;
            var level = 1;
            var levelFinished = 1;
            _audio.PlayAudio();



            for (int i = 1; i < 8; i++)
            {
                state.ScreenHeight = Console.WindowHeight - 2;
                state.ScreenWidth = Console.WindowWidth - 2;
                _splash.PrintSplashScreen("story_"+i, true, 1000);
            }
            state.ScreenHeight = Console.WindowHeight - 2;
            state.ScreenWidth = Console.WindowWidth - 2;
            _splash.PrintSplashScreen("logo", true, 0);
            Console.ReadKey();



            while (true)
            {
                state.ScreenHeight = Console.WindowHeight - 2;
                state.ScreenWidth = Console.WindowWidth - 2;

                if (levelFinished == level)
                {
                    RenderLevel(level);
                    level++;
                    InitializeLevel(level);
                }


                if (HasWon())
                {
                    levelFinished++;
                    if (levelFinished == 3)
                    {
                        _splash.PrintSplashScreen("win",true, 30000);
                        break;
                    }
                }
                else if (HasLost())
                {
                    win = false;
                    break;
                }
                else
                {
                    counter++;
                    if (counter % 10 == 0)
                    {
                        //state.Redness += 1;
                        counter = 0;
                    }
                }

                
               state.RecalculateScreenBuffer();

                t2 = DateTime.Now;
                var elapsed = (t2 - t1).TotalSeconds;
                t1 = t2;
                _movement.HandleMovementForPlayer(elapsed);
                _enemyMovement.HandleEnemyMovements(elapsed);
                _enemyMovement.HandleEnemyCollisions();

                _scoring.CalculateScore();
                _tweak.TweakGuiBasedOnUserInput();

                _playerView.RenderViewToScreenBuffer();
                _sprites.RenderSpritesToScreenBuffer();
                _sunset.CalculateTimeOfDay();

                theMap.ShowMapIfAppropriate();
                RenderScreenBufferToConsole();
            }

            _splash.PrintSplashScreen("win");
        }

        private void RenderLevel(int level)
        {
            _splash.PrintSplashScreen("level_"+level);
        }

      

        private void InitializeLevel(int level)
        {
            var mapString = File.ReadAllText($"map{level}.txt");
            state.Initialize(mapString);
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