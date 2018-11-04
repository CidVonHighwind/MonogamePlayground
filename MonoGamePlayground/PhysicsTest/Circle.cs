using Microsoft.Xna.Framework;

namespace MonoGamePlayground.PhysicsTest
{
    public class Circle
    {
        public Vector2 Position, Velocity;

        public float Radius, Mass;

        public Circle(Vector2 position, Vector2 velocity, float radius)
        {
            Position = position;
            Velocity = velocity;
            Radius = radius;
        }
    }
}