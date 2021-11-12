﻿namespace Ascii
{
    public enum KeyCode : int
    {
        /// <summary>
        /// The left arrow key.
        /// </summary>
        Left = 37,

        Up = 38,

        Right = 39,

        Down = 40,
        
        M = 77,
        Shift = 16,
        Ctrl = 17,
        Plus = 187,
        Minus = 189,
        O = 79,
        P = 80,
        Spacebar = 32,
        R = 82,
        E = 69

    }

   
    /// <summary>
    /// Provides keyboard access.
    /// </summary>
    public static class NativeKeyboard
    {
        /// <summary>
        /// A positional bit flag indicating the part of a key state denoting
        /// key pressed.
        /// </summary>
        private const int KeyPressed = 0x8000;

        /// <summary>
        /// Returns a value indicating if a given key is pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// <c>true</c> if the key is pressed, otherwise <c>false</c>.
        /// </returns>
        public static bool IsKeyDown(KeyCode key)
        {
            return (GetKeyState((int)key) & KeyPressed) != 0;
        }

        /// <summary>
        /// Gets the key state of a key.
        /// </summary>
        /// <param name="key">Virtuak-key code for key.</param>
        /// <returns>The state of the key.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetKeyState(int key);
    }
}