using System;
using System.Drawing;
using System.Globalization;
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

        public Engine(string mapString, int playerNumber)
        {

            state = new GameState
            {
                PlayerNumber = playerNumber,
                Map = mapString.Replace("\r", "").Split("\n").Select(line => line.ToCharArray()).ToArray(),
                PlayerViewAngle = 0,
                ScreenBuffer = new char[0],
                //Redness = 0
            };
            state.ReadStartingPositionFromMap(playerNumber);
            theMap = new Map(state);
            _inventory = new Inventory(state);
            _movement = new PlayerMovement(state, _inventory);
            _playerView = new PlayerView(state);
            _sprites = new Sprites(state);
            _tweak = new GuiTweaks(state);
            _scoring = new Scoring(state);
            _lazer = new Lazer(state);
        }

        private readonly PlayerMovement _movement;
        private readonly Sprites _sprites;
        private readonly PlayerView _playerView;
        private readonly GuiTweaks _tweak;
        private readonly Scoring _scoring;
        private readonly Lazer _lazer;
        private readonly Inventory _inventory;

        public async Task Run()
        {
            var t1 = DateTime.Now;
            var t2 = DateTime.Now;
            var counter = 0;
            var win = false;
            while (true)
            {
                state.ScreenHeight = Console.WindowHeight-2;
                state.ScreenWidth = Console.WindowWidth-2;

                if (HasWon())
                {
                    win = true;
                    break;
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
                _scoring.CalculateScore();
                _tweak.TweakGuiBasedOnUserInput();
                _playerView.RenderViewToScreenBuffer();
                _sprites.RenderSpritesToScreenBuffer();

                theMap.ShowMapIfAppropriate();
                RenderScreenBufferToConsole();
            }

            PrintSplashScreen(win ? "You WIN!" : "You LOSE!");
        }

        private void PrintSplashScreen(string message)
        {
            var height = state.ScreenHeight / 2;
            var width = state.ScreenWidth / 2;
            var startY = state.ScreenHeight / 4;
            var startX = state.ScreenWidth / 4;
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                Console.Write(string.Join("",Enumerable.Repeat("*",width)));
            }

            width -= 6;
            height -= 4;
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(startX+3, startY +2 + y);
                Console.Write(string.Join("", Enumerable.Repeat(" ", width)));
            }
            Console.SetCursorPosition(state.ScreenWidth/2 - (message.Length/2), state.ScreenHeight / 2);
            Console.Write(message);
            Console.SetCursorPosition(0, state.ScreenHeight-3);
            Console.ReadKey();
        }

        private bool HasLost()
        {
            return false; //state.Redness >= 0xE0;
        }

        private bool HasWon()
        {
            return !state.Map.Any(y=>y.Any(x=>x == 'o'));
        }


        private void RenderScreenBufferToConsole()
        {
            Console.SetCursorPosition(0,0);
            //var hexValue = (0xFF - state.Redness).ToString("X");
            //var color = $"#FF{hexValue}{hexValue}";
            //if (state.LazerMapCoords.Any(lc => lc.Schmequals(state.PlayerCoord)))
            //{
            //    color = "#FF0000";
            //}
            Console.Write(new string(state.ScreenBuffer));//.Pastel(color));
            Console.WriteLine($"p:{state.PlayerCoord.X}/{state.PlayerCoord.Y} ({state.PlayerViewAngle})                                                            ".Pastel(Color.White));
        }

       


     

       

        



        

       
    }
}