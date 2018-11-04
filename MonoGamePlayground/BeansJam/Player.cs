using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.BeansJam
{
    class Player
    {
        private BeansJamGame.Planet _mainPlanet;
        private Rectangle _playerRectangle = new Rectangle(20, 0, 20, 10);
        private Vector2 _position;
        private Vector2 _velocity;

        private float _minDistance;
        private float _acceleration = -5;
        private float _rotation;
        private float _speed = 1f;
        private float _rVelocity;

        private bool _isJumping;

        public Player()
        {
            _position = new Vector2(0, -100);
            _rotation = (float)Math.PI / 2;
        }

        public void Update(GameTime gameTime)
        {
            bool collision = false;
            _minDistance = float.MaxValue;

            for (int i = 0; i < BeansJamGame.planets.Count; i++)
            {
                Vector2 vec = (BeansJamGame.planets[i].position - _position);

                if (vec.Length() - BeansJamGame.planets[i].radius < _minDistance)
                {
                    _minDistance = vec.Length() - BeansJamGame.planets[i].radius;
                    _mainPlanet = BeansJamGame.planets[i];
                }

                float planetDistance = vec.Length();
                vec.Normalize();

                Vector2 planetAcceleration = vec * (BeansJamGame.planets[i].radius * 5 / planetDistance) * 0.05f;
                _velocity += planetAcceleration;
            }

            for (int i = 0; i < BeansJamGame.planets.Count; i++)
                if (Math.Abs(((_position + _velocity) - BeansJamGame.planets[i].position).Length()) <= BeansJamGame.planets[i].radius)
                    collision = true;

            if (collision)
            {
                _velocity = Vector2.Zero;
                _isJumping = false;
            }
            else
            {
                _position += _velocity;
            }

            Vector2 mainVec = (_mainPlanet.position - _position);
            mainVec.Normalize();

            //rotation = (float)Math.Atan2(mainVec.Y, mainVec.X);


            if (BeansJamGame.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                _rVelocity -= 0.0001f;
                //velocity += new Vector2(-mainVec.Y, mainVec.X) * speed;
            }
            if (BeansJamGame.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                _rVelocity += 0.0001f;
                //velocity -= new Vector2(-mainVec.Y, mainVec.X) * speed;
            }

            _rotation += _rVelocity;

            //if (!isJumping && BeansJamGame.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            //{
            //    isJumping = true;
            //    velocity = mainVec * acceleration;
            //}

            if (BeansJamGame.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                Vector2 engineVector = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
                _velocity += engineVector * -0.351f;
            }

            _playerRectangle.X = (int)(_position.X);
            _playerRectangle.Y = (int)(_position.Y);
        }

        public void Draw(SpriteBatch spriteBatch, float scale, Vector2 cameraPosition)
        {
            spriteBatch.Draw(BeansJamGame.sprRocket, new Rectangle(
                (int)((_position.X + cameraPosition.X) * scale),
                (int)((_position.Y + cameraPosition.Y) * scale),
                (int)(_playerRectangle.Width * scale),
                (int)(_playerRectangle.Height * scale)),
                new Rectangle(0, 0, 2, 1), Color.White, _rotation,
                new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
        }
    }
}
