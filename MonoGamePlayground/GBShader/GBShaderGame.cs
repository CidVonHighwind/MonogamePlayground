using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.GBShader
{
    class GBShaderGame : IGame
    {
        RenderTarget2D renderTargetShader;
        Effect shaderGameBoy;
        Texture2D sprGameBoy;

        float scale;
        int windowWidth, windowHeight;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprGameBoy = content.Load<Texture2D>("GBShader/kidDracula");
            shaderGameBoy = content.Load<Effect>("GBShader/gbShader1");

            scale = 5;

            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;

            renderTargetShader = new RenderTarget2D(
                graphics.GraphicsDevice, 160 * (int)scale, 144 * (int)scale);
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTargetShader);

            shaderGameBoy.Parameters["spriteWidth"].SetValue(160 * (int)scale);
            shaderGameBoy.Parameters["spriteHeight"].SetValue(144 * (int)scale);
            shaderGameBoy.Parameters["scale"].SetValue((int)scale);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            else
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, shaderGameBoy);

            spriteBatch.Draw(sprGameBoy, new Rectangle(0, 0,
                (int)(sprGameBoy.Width * (int)scale),
                (int)(sprGameBoy.Height * (int)scale)), new Color(142, 150, 114));

            spriteBatch.End();

            // draw to the screen
            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(renderTargetShader, new Rectangle(
                windowWidth / 2 - (int)(sprGameBoy.Width * scale) / 2,
                windowHeight / 2 - (int)(sprGameBoy.Height * scale) / 2,
                (int)(sprGameBoy.Width * scale), (int)(sprGameBoy.Height * scale)), Color.White);

            spriteBatch.Draw(renderTargetShader, new Vector2(
                windowWidth / 2 - (int) (sprGameBoy.Width * scale) / 2,
                windowHeight / 2 - (int) (sprGameBoy.Height * scale) / 2), Color.White);

            spriteBatch.End();
        }
    }
}
