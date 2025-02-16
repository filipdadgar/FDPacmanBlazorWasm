namespace FDPacmanBlazorWasm.Game
{
    public abstract class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char UnderChar { get; set; } = ' ';
        protected Entity(int y, int x)
        {
            Y = y;
            X = x;
        }
    }
}