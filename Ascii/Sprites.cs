using System;

namespace Ascii
{
    public class Sprites
    {
        public Sprites(GameState state)
        {
            this.State = state;
        }

        public GameState State { get; set; }



        public void RenderSpritesToScreenBuffer()
        {
            for (int x = 0; x < State.ScreenWidth; x++)
            {
                // For each column, calculate the projected ray angle into world space
                double rayAngle = (State.PlayerViewAngle - (State.FOV / 2.0)) + (x * State.FOV / (double)State.ScreenWidth);
                var rayResolution = 0.1;
                var distanceToSprite = 0.0;

                char? spriteHit = null;

                var edgeHit = false;
                var eyeX = Math.Sin(rayAngle);
                var eyeY = Math.Cos(rayAngle);

                var bullsEyeY = 0.0;
                var bullsEyeX = 0.0;
                var bullsEyeXY = 0.0;


                while (spriteHit == null && distanceToSprite < State.RenderDepth)
                {
                    distanceToSprite += rayResolution;
                    var rayTestX = (int)(State.PlayerCoord.X + (eyeX * distanceToSprite));
                    var rayTestY = (int)(State.PlayerCoord.Y + (eyeY * distanceToSprite));
                    var rayTestDX = (State.PlayerCoord.X + (eyeX * distanceToSprite));
                    var rayTestDY = (State.PlayerCoord.Y + (eyeY * distanceToSprite));

                    if (rayTestX < 0 || rayTestX >= State.MapWidth || rayTestY < 0 || rayTestY >= State.MapHeight)
                    {
                        spriteHit = ' '; //No need to see beyond world
                    }
                    else
                    {
                        if (State.Map[rayTestY][rayTestX] == '#')
                        {
                            spriteHit = ' '; //wall is not sprite
                        }
                        else if (State.Map[rayTestY][rayTestX] != '.')
                        {
                            bullsEyeY = (1 - 2* Math.Abs(rayTestDY - ((double) rayTestY + 0.5)));
                            bullsEyeX = (1 - 2* Math.Abs(rayTestDX - ((double) rayTestX + 0.5)));
                            bullsEyeXY = Math.Sqrt(bullsEyeX * bullsEyeY);
                            if (bullsEyeXY > 0.75 )
                            {
                                spriteHit = State.Map[rayTestY][rayTestX];
                            }
                        }
                    }
                }


                if (spriteHit.HasValue && spriteHit.Value != ' ')
                {
                    var middle = State.ScreenHeight / 2.0;

                    var normalCeiling = (middle - ((double)State.ScreenHeight / distanceToSprite));
                    var closerToCenter = (normalCeiling + middle)/2.0;
                    var evenCloserToCenter = (closerToCenter + middle)/2.0;
                    var heightFromMiddle =middle - evenCloserToCenter;

                    var from = (int) (middle - heightFromMiddle);
                    var to = (int)(middle + heightFromMiddle);

                    for (int y = 0; y < State.ScreenHeight; y++)
                    {
                        var centerness = 1 - Math.Abs(y - middle) / middle;

                        if (y > from && y <= to)
                        {
                            if (Math.Sqrt(centerness * bullsEyeXY) > 0.80)
                            {
                                var buffercoord = State.SBP(x, y);
                                State.ScreenBuffer[buffercoord] = spriteHit.Value;
                            }
                        }
                    }
                }
            }
        }
    }
}