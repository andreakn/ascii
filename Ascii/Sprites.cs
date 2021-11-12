using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ascii
{
    public class Sprite
    {
        public char[][] Chars { get; set; }
        public string Name { get; set; }
        public int Distance { get; set; }
    }

    public class Sprites
    {
        private List<Sprite> _sprites = ReadSpriteFiles();

        private static List<Sprite> ReadSpriteFiles()
        {
            var sprites = new List<Sprite>();
            foreach (var file in Directory.EnumerateFiles("sprites"))
            {
                if (file.EndsWith(".txt") && file.Contains("_"))
                {
                    var lines = File.ReadAllLines(file);
                    var size = int.Parse(file.Split('.')[0].Split('_')[1]);
                    var fileName = file.Replace("sprites\\", "");
                    var sprite = new Sprite
                    {
                        Name = fileName.Split('_')[0],
                        Distance = size,
                        Chars = new char[lines.Length][]
                    };
                    for (var index = 0; index < sprite.Chars.Length; index++)
                    {
                        var line = lines[index];
                        sprite.Chars[index] = line.ToCharArray();
                    }
                    sprites.Add(sprite);
                }
            }

            return sprites;
        }

        public Sprites(GameState state)
        {
            this.State = state;


          
        }

        public GameState State { get; set; }



        public void RenderSpritesToScreenBuffer()
        {
            var babeDistance = -1D;
            var babeStartX = -1;

            List<SpriteInfo> sprites = new();

            for (int x = 0; x < State.ScreenWidth; x++)
            {
                // For each column, calculate the projected ray angle into world space
                double rayAngle = (State.Player.ViewAngle - (State.Player.FOV / 2.0)) + (x * State.Player.FOV / (double)State.ScreenWidth);
                var rayResolution = 0.1;
                var distanceToSprite = 0.0;

                char? spriteHit = null;

                var edgeHit = false;
                var eyeX = Math.Sin(rayAngle);
                var eyeY = Math.Cos(rayAngle);

                var bullsEyeY = 0.0;
                var bullsEyeX = 0.0;
                var bullsEyeXY = 0.0;


                if (Math.Abs(rayAngle) < 0.1)
                {
                    RegisterSprite(sprites, 0, 0, 's',x,100);
                }

                while (spriteHit == null && distanceToSprite < State.RenderDepth)
                {
                    distanceToSprite += rayResolution;
                    var rayTestX = (int)(State.Player.Coord.X + (eyeX * distanceToSprite));
                    var rayTestY = (int)(State.Player.Coord.Y + (eyeY * distanceToSprite));
                    var rayTestDX = (State.Player.Coord.X + (eyeX * distanceToSprite));
                    var rayTestDY = (State.Player.Coord.Y + (eyeY * distanceToSprite));

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
                        else if (State.Map[rayTestY][rayTestX] == 'b')
                        {
                            RegisterSprite(sprites, rayTestY, rayTestX, 'b', x, distanceToSprite);
                        }
                        else if (State.Map[rayTestY][rayTestX] == 'c')
                        {
                            RegisterSprite(sprites, rayTestY, rayTestX, 'c', x, distanceToSprite);
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

                    foreach (var mob in State.Mobs)
                    {
                        if (mob.IsNear(rayTestDX, rayTestDY))
                        {
                            RegisterSprite(sprites, mob.Coord.Y, mob.Coord.X, 'c', x, distanceToSprite);

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

            foreach (var spriteInfo in sprites.Where(s=>s.SpriteName!="sun").OrderByDescending(s=>s.Distance))
            {
                var spriteWidth = (spriteInfo.SpriteEndX - spriteInfo.SpriteStartX);
                var middleX =  spriteInfo.SpriteStartX + spriteWidth/ 2;
                var relevantSprites = _sprites.Where(x => x.Name == spriteInfo.SpriteName).ToList();
                var bestMatch = relevantSprites.OrderBy(x => x.Distance).FirstOrDefault(x => x.Distance > spriteInfo.Distance)
                                ;
                if(bestMatch == null){continue;}
                var spriteChars = bestMatch.Chars;
                var startX = middleX - (spriteChars[0].Length/2);

                var ceiling = (int)(((double)State.ScreenHeight / 2.0) - ((double)State.ScreenHeight / spriteInfo.Distance));
                var endY = State.ScreenHeight - ceiling;
                var startY = endY - spriteChars.Length;

                for (int yy = 0; yy < spriteChars.Length; yy++)
                {
                    for (int xx = 0; xx < spriteChars[yy].Length; xx++)
                    {
                        var yyy = yy + startY;
                        var xxx = xx + startX;
                        var bufferCoord = State.SBP(xxx, yyy);
                        if (bufferCoord >= 0 && bufferCoord < State.ScreenBuffer.Length
                                             && spriteChars[yy][xx] != ' ')
                        {
                            if (State.ScreenBuffer[bufferCoord] == '\n')
                            {
                                continue;
                            }
                            State.ScreenBuffer[bufferCoord] = spriteChars[yy][xx];
                        }
                    }
                }
            }

            var sun = sprites.FirstOrDefault(x => x.SpriteName == "sun");
            if (sun != null)
            {
                var spriteWidth = (sun.SpriteEndX - sun.SpriteStartX);
                var middleX = sun.SpriteStartX + spriteWidth / 2;
                var sunSprite = _sprites.First(x => x.Name == sun.SpriteName);

                var startY = 3;
                var spriteChars = sunSprite.Chars;
                var startX = middleX - (spriteChars[0].Length / 2);
                for (int yy = 0; yy < spriteChars.Length; yy++)
                {
                    for (int xx = 0; xx < spriteChars[yy].Length; xx++)
                    {
                        var yyy = yy + startY;
                        var xxx = xx + startX;
                        var bufferCoord = State.SBP(xxx, yyy);
                        if (bufferCoord >= 0 && bufferCoord < State.ScreenBuffer.Length
                                             && spriteChars[yy][xx] != ' ')
                        {
                            if (State.ScreenBuffer[bufferCoord] != ' ' )
                            {
                                continue;
                            }
                            State.ScreenBuffer[bufferCoord] = spriteChars[yy][xx];
                        }
                    }
                }

            }
        }

        private void RegisterSprite(List<SpriteInfo> sprites, int rayTestY, int rayTestX, char name, int viewX, double distanceToSprite)
        {
            var sprite = (sprites.FirstOrDefault(s => s.SpritePositionX == rayTestX && s.SpritePositionY == rayTestY));

            if (sprite == null)
            {
                sprite = new SpriteInfo
                {
                    SpritePositionX = rayTestX,
                    SpritePositionY = rayTestY,
                    SpriteName = GetSpriteName(name),
                    Distance = distanceToSprite,
                    SpriteStartX = viewX,
                };
                sprites.Add(sprite);
            }
            sprite.SpriteEndX = viewX;

        }
        private void RegisterSprite(List<SpriteInfo> sprites, double rayTestY, double rayTestX, char name, int viewX, double distanceToSprite)
        {
            var sprite = (sprites.FirstOrDefault(s => s.SpritePositionX == rayTestX && s.SpritePositionY == rayTestY));

            if (sprite == null)
            {
                sprite = new SpriteInfo
                {
                    SpritePositionX = rayTestX,
                    SpritePositionY = rayTestY,
                    SpriteName = GetSpriteName(name),
                    Distance = distanceToSprite,
                    SpriteStartX = viewX,
                };
                sprites.Add(sprite);
            }
            sprite.SpriteEndX = viewX;

        }

        private string GetSpriteName(char name)
        {
            if (name == 'b') return "girl";
            if (name == 'c') return "chick";
            if (name == 's') return "sun";
            return "chick";
        }


        private void RenderBabe(int babeStartX, double babeDistance)
        {
            //for(int i = 0; i<_babe.Length; i++)
            //for (int j = 0; j < _babe[i].Length; j++)
            //{
            //    var middle = State.ScreenHeight / 2.0;

            //    var normalCeiling = (middle - ((double)State.ScreenHeight / distanceToSprite));
            //    var closerToCenter = (normalCeiling + middle) / 2.0;
            //    var evenCloserToCenter = (closerToCenter + middle) / 2.0;
            //    var heightFromMiddle = middle - evenCloserToCenter;

            //    var from = (int)(middle - heightFromMiddle);
            //    var to = (int)(middle + heightFromMiddle);

            //    for (int y = 0; y < State.ScreenHeight; y++)
            //    {
            //        var centerness = 1 - Math.Abs(y - middle) / middle;

            //        if (y > from && y <= to)
            //        {
            //            if (Math.Sqrt(centerness * bullsEyeXY) > 0.80)
            //            {
            //                var buffercoord = State.SBP(x, y);
            //                State.ScreenBuffer[buffercoord] = spriteHit.Value;
            //            }
            //        }
            //    }
            //    }
        }
    }


    public class SpriteInfo
    {
        public double SpritePositionX { get; set; }
        public double SpritePositionY { get; set; }
        public string SpriteName { get; set; }
        public int SpriteStartX { get; set; }
        public int SpriteEndX { get; set; }
        public double Distance { get; set; }
    }
}