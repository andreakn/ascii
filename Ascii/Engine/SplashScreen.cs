using System;
using System.Drawing;
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

        
        public void ShowFileInSplashScreen(string file, bool clear, int delayMillis, bool waitForUserInputAfterDelay = false)
        {
            if (!File.Exists(file))
                return;
            var text = File.ReadAllText(file);
            ShowSplashText(text, clear, delayMillis, waitForUserInputAfterDelay);

        }

        public void ShowSplashText(string text, bool clear, int delayMillis, bool waitForUserInputAfterDelay = false)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            State.AdjustToScreenToConsoleDimentions();
            
            if (clear)
            {
                for (int i = 0; i < State.ScreenHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(string.Join("", Enumerable.Repeat(" ", State.ScreenWidth)).Pastel(Color.White));
                }
            }
            
            var height = State.ScreenHeight * 2 / 3;
            var width = State.ScreenWidth * 2 / 3;
            var startY = (int)(State.ScreenHeight * 0.16);
            var startX = (int)(State.ScreenWidth * 0.16);
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                Console.Write(string.Join("", Enumerable.Repeat("*", width)).Pastel(Color.White));
            }

            width -= 6;
            height -= 4;
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(startX + 3, startY + 2 + y);
                Console.Write(string.Join("", Enumerable.Repeat(" ", width)).Pastel(Color.White));
            }

            var lines = text.Replace("\r", "").Split('\n');
            var initialX = State.ScreenWidth / 2 - (lines.Max(l => l.Length) / 2);
            var initialY = (State.ScreenHeight / 2 - lines.Length / 2);
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(initialX, initialY + i);
                Console.Write(lines[i].Pastel(Color.White));
            }

            if (delayMillis > 0)
            {
                Thread.Sleep(delayMillis);
            }

            if (waitForUserInputAfterDelay)
            {
                Console.ReadKey();
            }
        }
    }
}