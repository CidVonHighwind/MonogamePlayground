using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.BluredShadowShader
{
    class BluredShadowShaderGame : IGame
    {
        private Texture2D sprTexture;

        private Effect shadowEffect, blurEffect, depthOfField;

        private RenderTarget2D renderTarget, renderTargetBlured, renderTargetDepth, renderTargetOriginal;

        Matrix viewMatrix;
        Matrix projectionMatrix;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            shadowEffect = content.Load<Effect>("BluredShadowShader/shadow");
            blurEffect = content.Load<Effect>("BluredShadowShader/EffectBlur");
            depthOfField = content.Load<Effect>("BluredShadowShader/DepthOfField");

            sprTexture = content.Load<Texture2D>("BluredShadowShader/link0");

            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            renderTargetBlured = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            renderTargetOriginal = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            renderTargetDepth = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            blurEffect.Parameters["pixelX"].SetValue(1f / Game1.graphics.PreferredBackBufferWidth);
            blurEffect.Parameters["pixelY"].SetValue(1f / Game1.graphics.PreferredBackBufferHeight);

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTargetOriginal);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            spriteBatch.Draw(sprTexture, new Rectangle(100, 100, 14 * 3, 16 * 3), new Rectangle(0, 0, 14, 16), Color.White);
            spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTargetDepth);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, shadowEffect, null);
            spriteBatch.Draw(sprTexture, new Rectangle(100, 100, 14 * 3, 16 * 3), new Rectangle(0, 0, 14, 16), Color.White);
            spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
            blurEffect.Parameters["mode"].SetValue(0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, blurEffect, null);
            spriteBatch.Draw(renderTargetOriginal, Vector2.Zero, Color.White);
            spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTargetBlured);
            blurEffect.Parameters["mode"].SetValue(1);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, blurEffect, null);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            for (var i = 0; i < 1; i++)
            {
                Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
                blurEffect.Parameters["mode"].SetValue(0);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, blurEffect, null);
                spriteBatch.Draw(renderTargetBlured, Vector2.Zero, Color.White);
                spriteBatch.End();
                Game1.graphics.GraphicsDevice.SetRenderTarget(renderTargetBlured);
                blurEffect.Parameters["mode"].SetValue(1);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, blurEffect, null);
                spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            Game1.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            depthOfField.Parameters["SpirteDepth"].SetValue(renderTargetDepth);
            depthOfField.Parameters["SpriteOriginal"].SetValue(renderTargetOriginal);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, depthOfField, null);
            spriteBatch.Draw(renderTargetBlured, Vector2.Zero, Color.White);
            spriteBatch.End();

            Game1.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawSpriteWithShadow(spriteBatch, 380, 200, new Rectangle(0, 0, 14, 16), new Rectangle(0, 0, 14, 16));
            DrawSpriteWithShadow(spriteBatch, 180, 200, new Rectangle(16, 0, 32, 32), new Rectangle(16, 0, 32, 28));
        }


        public void DrawSpriteWithShadow(SpriteBatch spriteBatch, int posX, int posY, Rectangle rectangle, Rectangle shadowRectangle)
        {
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight, 0, 0, -1);
            shadowEffect.Parameters["xViewProjection"].SetValue(projectionMatrix);
            shadowEffect.Parameters["pixelWidth"].SetValue(1f / sprTexture.Width * 0.75f);
            shadowEffect.Parameters["posX"].SetValue(posX);
            shadowEffect.Parameters["posY"].SetValue(posY);
            shadowEffect.Parameters["width"].SetValue(shadowRectangle.Width * 3f);
            shadowEffect.Parameters["height"].SetValue(shadowRectangle.Height * 3f);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, shadowEffect, null);
            spriteBatch.Draw(sprTexture, new Rectangle(posX, posY, shadowRectangle.Width * 3, shadowRectangle.Height * 3), rectangle, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            spriteBatch.Draw(sprTexture, new Rectangle(posX, posY, rectangle.Width * 3, rectangle.Height * 3), rectangle, Color.White);
            spriteBatch.End();
        }
    }
}
