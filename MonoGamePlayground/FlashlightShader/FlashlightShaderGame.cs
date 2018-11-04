using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.SpriteShadowShader
{
    class FlashlightShaderGame : IGame
    {
        Texture2D sprImage, sprLight;
        RenderTarget2D rdTargetOne;

        Vector2[] dirs = { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1) };
        Vector2 position = new Vector2(3 * 10 * 16, 7 * 8 * 16);
        Effect lightShader;

        int dir;
        private bool init;
        bool moving;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprImage = content.Load<Texture2D>("LightOne/shaderTest");
            sprLight = content.Load<Texture2D>("LightOne/light");

            lightShader = content.Load<Effect>("LightOne/lightShader");

            rdTargetOne = new RenderTarget2D(Game1.graphics.GraphicsDevice, sprImage.Width, sprImage.Height,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                init = false;

            if (!moving)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    moving = true;
                    dir = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    moving = true;
                    dir = 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    moving = true;
                    dir = 2;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    moving = true;
                    dir = 3;
                }
            }
            else
            {
                position += dirs[dir] * 4;

                if ((position.X % (10 * 16)) == 0 && (position.Y % (8 * 16)) == 0)
                    moving = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.SetRenderTarget(rdTargetOne);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            // draw the light
            spriteBatch.Begin();
            spriteBatch.Draw(sprLight, new Vector2(Mouse.GetState().X - sprLight.Width / 2, Mouse.GetState().Y - sprLight.Height / 2), Color.White);
            spriteBatch.End();
            
            lightShader.Parameters["sprImage"].SetValue(rdTargetOne);
            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            Game1.graphics.GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, lightShader);
            spriteBatch.Draw(sprImage, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
