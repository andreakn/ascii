using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Ascii
{
    public class SplashScreen
    {
        public SplashScreen(GameState s)
        {
            State = s;
        }

        public GameState State { get; set; }


        public void PrintSplashScreen(string splash, bool clear = false, int delayMillis = 2000)
        {
            if (clear)
            {
                for(int i = 0; i<State.ScreenHeight; i++)
                {
                    Console.SetCursorPosition(0,i);
                    Console.Write(string.Join("", Enumerable.Repeat(" ",State.ScreenWidth)));
                }
            }

            var message = File.ReadAllText($"splash/{splash}.txt");

            var height = State.ScreenHeight * 2 / 3;
            var width = State.ScreenWidth *2 / 3;
            var startY = (int)(State.ScreenHeight * 0.16) ;
            var startX = (int)(State.ScreenWidth * 0.16);
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                Console.Write(string.Join("", Enumerable.Repeat("*", width)));
            }

            width -= 6;
            height -= 4;
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(startX + 3, startY + 2 + y);
                Console.Write(string.Join("", Enumerable.Repeat(" ", width)));
            }

            var lines = message.Replace("\r", "").Split('\n');
            var initialX = State.ScreenWidth / 2 - (lines.Max(l=>l.Length)/ 2);
            var initialY = (State.ScreenHeight / 2 - lines.Length / 2);
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(initialX, initialY+i );
                Console.Write(lines[i]);
            }

            if (delayMillis > 0)
            {
                Thread.Sleep(delayMillis);
            }

        }


    }
}