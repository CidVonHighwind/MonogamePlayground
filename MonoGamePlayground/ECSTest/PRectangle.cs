using Microsoft.Xna.Framework;

namespace MonoGamePlayground.ECSTest
{
    public class PRectangle
    {
        public int X, Y, Width, Height;
        public int Right, Bottom;

        public PRectangle(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        public bool Intersects(PRectangle value)
        {
            return value.X < Right &&
                   X < value.Right &&
                   value.Y < Bottom &&
                   Y < value.Bottom;
        }

        public Vector2 Center => new Vector2(X + (Width / 2), Y + Height / 2);

        public void Update()
        {
            Right = X + Width;
            Bottom = Y + Height;
        }
    }
}
