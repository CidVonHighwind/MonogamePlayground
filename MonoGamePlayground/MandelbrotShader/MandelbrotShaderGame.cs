using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.MandelbrotShader
{
    class MandelbrotShaderGame : IGame
    {
        private Effect mShader;
        private Vector4 position = new Vector4(-2, -2, 4, 4);
        private Texture2D sprWhite;

        private MouseState currentMouseState, lastMouseState;

        private int windowWidth = 1280;
        private int windowHeight = 720;
        
        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            mShader = content.Load<Effect>("Mandelbrot/MandelbrotShader");

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            var zoomSpeed = 0.01f;

            // move the image with the mouse
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                position.X -= (currentMouseState.X - lastMouseState.X) / (float)windowWidth * position.Z;
                position.Y -= (currentMouseState.Y - lastMouseState.Y) / (float)windowHeight * position.W;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                var zoomAdd = position.Z * 0.01f;

                position.X -= zoomAdd / 2;

                position.Z += zoomAdd;
                position.W += zoomAdd;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                var zoomSub = position.Z * 0.01f;

                position.X += zoomSub / 2;

                position.Z -= zoomSub;
                position.W -= zoomSub;
            }
            
            position.W = position.Z * windowHeight / windowWidth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mShader.Parameters["posX"].SetValue(position.X);
            mShader.Parameters["posY"].SetValue(position.Y);
            mShader.Parameters["width"].SetValue(position.Z);
            mShader.Parameters["height"].SetValue(position.W);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, mShader);

            spriteBatch.Draw(sprWhite, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

            spriteBatch.End();
        }
    }
}
