namespace Ascii
{
    public class Mob: Entity
    {
        public string PrevInput { get; set; }= "";
        public char MobType { get; set; }

        public Mob()
        {
            Speed = 1;
        }
        
        public bool IsNear(Coord coord)
        {
            return Coord.IsNear(coord);
        }
    }
}