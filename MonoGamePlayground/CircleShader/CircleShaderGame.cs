using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.CircleShader
{
    class CircleShaderGame : IGame
    {
        private Texture2D sprTest1, sprTest2, sprLink;
        private Effect circleShader;

        Vector2 Middle = new Vector2(1497, 997);

        private float circleSize;

        private int timer;
        private int dir = 1, cImage = 1;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprTest1 = content.Load<Texture2D>("CircleShader/test1");
            sprTest2 = content.Load<Texture2D>("CircleShader/test2");
            sprLink = content.Load<Texture2D>("CircleShader/test3");

            circleShader = content.Load<Effect>("CircleShader/CircleShader");
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                timer = 0;

            timer += dir * gameTime.ElapsedGameTime.Milliseconds;

            float speed = 300;
            if (timer / speed > Math.PI)
            {
                timer = (int)(Math.PI * speed);
                dir = -dir;
                cImage = -cImage;
            }
            if (timer < 0)
            {
                timer = 0;
                dir = 1;
            }

            circleSize = (float)(Math.Cos(timer / speed) + 1) * Middle.Length() / 4f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            circleShader.Parameters["size"].SetValue(circleSize);//circleSize);
            circleShader.Parameters["centerX"].SetValue(Mouse.GetState().X);
            circleShader.Parameters["centerY"].SetValue(Mouse.GetState().Y);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, circleShader);

            if (cImage > 0)
                spriteBatch.Draw(sprTest1, Vector2.Zero, Color.White);
            else
                spriteBatch.Draw(sprTest2, Vector2.Zero, Color.White);

            spriteBatch.End();

            // draw link
            spriteBatch.Begin();

            spriteBatch.Draw(sprLink, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
