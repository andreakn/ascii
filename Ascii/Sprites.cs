using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Ascii
{
    public class Sprite
    {
        public char[][] Chars { get; set; }
        public char Name { get; set; }
        public int Distance { get; set; }
    }

    public class Sprites
    {
        private List<Sprite> _sprites;
        public GameState State { get; set; }
        public Sprites(GameState state)
        {
            State = state;
            _sprites = ReadSpriteFiles();
        }

        private List<Sprite> ReadSpriteFiles()
        {
            var sprites = new List<Sprite>();
            foreach (var entityDir in Directory.EnumerateDirectories($"{State.GameRootFolder}/entities"))
            {
                var entityName = new DirectoryInfo(Path.GetFileName(entityDir)).Name;

                foreach (var file in Directory.EnumerateFiles($"{entityDir}/sprites"))
                {
                    if (file.EndsWith(".txt"))
                    {
                        var fileName = new FileInfo(file).Name;
                        var lines = File.ReadAllLines(file);
                        var size = int.Parse(fileName.Split('.')[0]);
                        var sprite = new Sprite
                        {
                            Name = entityName[0],
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


                
            }

            return sprites;
        }





        public void RenderSpritesToScreenBuffer()
        {
            List<SpriteInfo> sprites = new();

            for (int x = 0; x < State.ScreenWidth; x++)
            {
                double rayAngle = (State.Player.ViewAngle - (State.Player.FOV / 2.0)) + (x * State.Player.FOV / (double)State.ScreenWidth);
                var rayResolution = 0.1;
                var distanceToSprite = 0.0;

                char? spriteHit = null;

                var eyeX = Math.Sin(rayAngle);
                var eyeY = Math.Cos(rayAngle);

                if (Math.Abs((rayAngle%(Math.PI*2))) < 0.1)
                {
                    RegisterSprite(sprites, 0, 0, '*',x,100); //register the sun (special handling)
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
                    }

                    foreach (var mob in State.Mobs)
                    {
                        if (mob.IsNear(new Coord(rayTestDX, rayTestDY)))
                        {
                            RegisterSprite(sprites, mob, x, distanceToSprite);
                        }
                    }
                }
               
            }

            foreach (var spriteInfo in sprites.Where(s=>s.SpriteName!='*').OrderByDescending(s=>s.Distance))
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

            var sun = sprites.FirstOrDefault(x => x.SpriteName == '_');
            if (sun != null)
            {
                var spriteWidth = (sun.SpriteEndX - sun.SpriteStartX);
                var middleX = sun.SpriteStartX + spriteWidth / 2;
                var sunSprite = _sprites.First(x => x.Name == sun.SpriteName);

                var middleY = State.ScreenHeight / 2;
                var startY =  middleY - (int)(State.SunHeight*middleY);

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
            var sprite = (sprites.FirstOrDefault(s => s.SpriteName==name && s.SpritePositionX == rayTestX && s.SpritePositionY == rayTestY));

            if (sprite == null)
            {
                sprite = new SpriteInfo
                {
                    SpritePositionX = rayTestX,
                    SpritePositionY = rayTestY,
                    SpriteName = name,
                    Distance = distanceToSprite,
                    SpriteStartX = viewX,
                };
                sprites.Add(sprite);
            }
            sprite.SpriteEndX = viewX;

        }
        private void RegisterSprite(List<SpriteInfo> sprites, Mob mob, int viewX, double distanceToSprite)
        {
            var mobSprite = (sprites.FirstOrDefault(s => s.Mob == mob));

            if (mobSprite == null)
            {
                mobSprite = new SpriteInfo
                {
                    Mob = mob,
                    SpritePositionX = mob.Coord.X,
                    SpritePositionY = mob.Coord.Y,
                    SpriteName =mob.MobType,
                    Distance = distanceToSprite,
                    SpriteStartX = viewX,
                };
                sprites.Add(mobSprite);
            }
            mobSprite.SpriteEndX = viewX;
        }
    }


    public class SpriteInfo
    {
        public double SpritePositionX { get; set; }
        public double SpritePositionY { get; set; }
        public char SpriteName { get; set; }
        public int SpriteStartX { get; set; }
        public int SpriteEndX { get; set; }
        public double Distance { get; set; }
        public Mob Mob { get; set; }
    }
}