using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.PhysicsTest
{
    class PhysicsTestGame : IGame
    {
        private MouseState _oldMouseState;
        private KeyboardState _oldKeyboardState;

        private Texture2D _sprCircle, _sprWhite;
        private Vector2 _spawnPosition;

        private List<Circle> _circleList = new List<Circle>();
        
        private Random RDom = new Random();

        private const int ScreenWidth = 1280;
        private const int ScreenHeight = 720;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            _sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _sprWhite.SetData(new[] { Color.White });

            _sprCircle = content.Load<Texture2D>("PhysicsTest/circle");
        }

        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                _spawnPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed)
            {
                var velocity = _spawnPosition - new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                velocity *= 0.3f;
                _circleList.Insert(0, new Circle(_spawnPosition, -velocity, RDom.Next(10, 50)));
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed && _oldMouseState.RightButton != ButtonState.Pressed)
                _circleList.Clear();

            if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                Keyboard.GetState().IsKeyDown(Keys.X) && !_oldKeyboardState.IsKeyDown(Keys.X))
            {
                foreach (var circle in _circleList)
                {
                    // gravity
                    circle.Velocity += new Vector2(0, 0.4f);
                    //circle.Velocity *= 0.99f;

                    float pushStrenght = 0f;
                    Circle collidingCircle = CollidingVelocity(circle);
                    if (collidingCircle != null)
                    {
                        var normal = collidingCircle.Position - circle.Position;
                        normal.Normalize();

                        float lenght = (collidingCircle.Velocity.Length() + circle.Velocity.Length()) / 2f;
                        collidingCircle.Velocity = normal * lenght * 0.8f + normal * pushStrenght;
                        circle.Velocity = -normal * lenght * 0.8f - normal * pushStrenght;
                    }
                    else if (circle.Position.X + circle.Velocity.X + circle.Radius >= ScreenWidth ||
                             circle.Position.X + circle.Velocity.X - circle.Radius <= 0)
                    {
                        circle.Velocity.X = -circle.Velocity.X * 0.8f;
                    }
                    else if (circle.Position.Y + circle.Velocity.Y + circle.Radius >= ScreenHeight ||
                             circle.Position.Y + circle.Velocity.Y - circle.Radius <= 0)
                    {
                        circle.Velocity.Y = -circle.Velocity.Y * 0.8f;
                    }
                    else
                    {
                        circle.Position += circle.Velocity;
                    }
                }
            }

            _oldMouseState = Mouse.GetState();
            _oldKeyboardState = Keyboard.GetState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var circle in _circleList)
            {
                spriteBatch.Draw(_sprCircle,
                    new Rectangle((int)(circle.Position.X - circle.Radius), (int)(circle.Position.Y - circle.Radius), (int)(circle.Radius * 2),
                        (int)circle.Radius * 2), Color.Green);

                float rotation = (float)Math.Atan2(circle.Velocity.Y, circle.Velocity.X);
                spriteBatch.Draw(_sprWhite, new Rectangle((int)circle.Position.X, (int)circle.Position.Y, (int)(circle.Velocity.Length() * 3), 2),
                    new Rectangle(0, 0, 1, 1), Color.Red, rotation, Vector2.Zero, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        public Circle Colliding(Circle circle)
        {
            return _circleList.Where(othersCircle => othersCircle != circle).FirstOrDefault(othersCircle => Colliding(othersCircle, circle));
        }

        public bool Colliding(Circle circleOne, Circle circleTwo)
        {
            return (circleOne.Position - circleTwo.Position).Length() < circleOne.Radius + circleTwo.Radius;
        }

        public Circle CollidingVelocity(Circle circle)
        {
            return _circleList.Where(othersCircle => othersCircle != circle).FirstOrDefault(othersCircle => CollidingVelocity(othersCircle, circle));
        }

        public bool CollidingVelocity(Circle circleOne, Circle circleTwo)
        {
            return ((circleOne.Position + circleOne.Velocity) - (circleTwo.Position + circleTwo.Velocity)).Length() < circleOne.Radius + circleTwo.Radius;
        }
    }
}
