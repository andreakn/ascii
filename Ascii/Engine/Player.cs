namespace Ascii
{
    public class Player : Entity
    {
        public Player()
        {
            Coord = new Coord
            {
                X = 1,
                Y = 1
            };
            FOV = 3.14159f / 4.0f;
        }
    }
}