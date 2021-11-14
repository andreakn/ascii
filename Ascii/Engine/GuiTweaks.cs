using System;

namespace Ascii
{
    public class GuiTweaks
    {
        public GuiTweaks(GameState state)
        {
            State = state;
        }

        public GameState State{ get; set; }

        public void TweakGuiBasedOnUserInput()
        {
            if (NativeKeyboard.IsKeyDown(KeyCode.Plus))
            {
                State.Player.FOV += Math.PI / 256.0;
                State.Player.FOV = Math.Min(State.Player.FOV, Math.PI *2.0);
            }
            if (NativeKeyboard.IsKeyDown(KeyCode.Minus))
            {
                State.Player.FOV -= Math.PI / 256.0;
                State.Player.FOV = Math.Max(State.Player.FOV, Math.PI / 16.0);
            }
            if (NativeKeyboard.IsKeyDown(KeyCode.O))
            {
                State.RenderDepth += 0.1;
                State.RenderDepth = Math.Min(State.RenderDepth, 128);
            }
            if (NativeKeyboard.IsKeyDown(KeyCode.P))
            {
                State.RenderDepth -= 0.1;
                State.RenderDepth = Math.Max(State.RenderDepth, 4);
            }

            
        }

    }
}