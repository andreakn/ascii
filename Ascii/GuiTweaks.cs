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
                State.FOV += Math.PI / 1024.0;
                State.FOV = Math.Min(State.FOV, Math.PI *2.0);
            }
            if (NativeKeyboard.IsKeyDown(KeyCode.Minus))
            {
                State.FOV -= Math.PI / 1024.0;
                State.FOV = Math.Max(State.FOV, Math.PI / 16.0);
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