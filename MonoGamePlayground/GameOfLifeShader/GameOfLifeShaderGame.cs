using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.GameOfLifeShader
{
    class GameOfLifeShaderGame : IGame
    {
        private MouseState currentMouseState, lastMouseState;
        private Effect shader;

        private RenderTarget2D renderTarget, renderTarget2;

        private Texture2D sprWhite, sprLastStep, sprGun;
        private Vector2 CameraPosition;
        private Vector2 canvasPosition;
        private Point imageSize;

        private float scale = 1;
        private int counter;
        private int pixelSize = 3;

        public bool update;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            shader = content.Load<Effect>("GameOfLife/GameOfLife");

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            sprGun = content.Load<Texture2D>("GameOfLife/breeder");

            ScreenSizeChanged(1280, 720);
        }

        public void Update(GameTime gameTime)
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            counter += gameTime.ElapsedGameTime.Milliseconds;

            update = false;

            var speed = 20;
            if (counter > speed)
            {
                update = true;
                counter -= speed;
            }

            if (currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                CameraPosition += new Vector2(
                    currentMouseState.X - lastMouseState.X,
                    currentMouseState.Y - lastMouseState.Y);
            }

            var mouseVector = new Vector2(currentMouseState.X, currentMouseState.Y);
            canvasPosition = (mouseVector - CameraPosition) / scale;

            // zoom in
            if (currentMouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue)
            {
                CameraPosition -= (canvasPosition * 1.05f - canvasPosition) * scale;
                scale *= 1.05f;
            }

            // zoom out
            if (currentMouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue)
            {
                CameraPosition -= (canvasPosition * 0.95f - canvasPosition) * scale;
                scale *= 0.95f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (imageSize.X == 0)
                return;

            //shader.Parameters["width"].SetValue(Game1.WindowSize.X);
            //shader.Parameters["height"].SetValue(Game1.WindowSize.Y);
            shader.Parameters["pixelX"].SetValue(1 / (float)imageSize.X);
            shader.Parameters["pixelY"].SetValue(1 / (float)imageSize.Y);
            shader.Parameters["isRunning"].SetValue(!Keyboard.GetState().IsKeyDown(Keys.Space) && update);

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            spriteBatch.Begin();

            spriteBatch.Draw(sprLastStep ?? sprWhite, Vector2.Zero, Color.White);

            // draw a cell
            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed)
                spriteBatch.Draw(sprGun, new Vector2(
                    (int)canvasPosition.X - sprGun.Width / 2, 
                    (int)canvasPosition.Y - sprGun.Height / 2), Color.White);

            spriteBatch.End();

            if (currentMouseState.RightButton == ButtonState.Pressed)
                Game1.graphics.GraphicsDevice.Clear(Color.White);

            // create the next step
            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget2);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, shader);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            sprLastStep = renderTarget2;

            // draw the image to the screen
            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp);
            spriteBatch.Draw(renderTarget2, new Rectangle(
                (int)CameraPosition.X, (int)CameraPosition.Y,
                (int)(renderTarget2.Width * scale),
                (int)(renderTarget2.Height * scale)), Color.White);

            spriteBatch.Draw(sprGun, new Rectangle(
                (int)((canvasPosition.X - sprGun.Width / 2) * scale) + (int)CameraPosition.X,
                (int)((canvasPosition.Y - sprGun.Height / 2) * scale) + (int)CameraPosition.Y,
                (int)(sprGun.Width * scale), 
                (int)(sprGun.Height * scale)), Color.White * 0.25f);

            spriteBatch.Draw(sprWhite, Vector2.Zero, Color.Green * 0.2f);

            spriteBatch.End();
        }

        public void ScreenSizeChanged(int width, int height)
        {
            imageSize = new Point(width / pixelSize, height / pixelSize);
            imageSize = new Point(10000, 10000);

            // resize the rendertarget
            renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, imageSize.X, imageSize.Y,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            renderTarget2 = new RenderTarget2D(Game1.graphics.GraphicsDevice, imageSize.X, imageSize.Y,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }
    }
}
