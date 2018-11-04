using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.UIShadowShader
{
    class UIShadowShaderGame : IGame
    {
        private Texture2D sprWhite;
        private SpriteFont font1;

        Effect shadowShader;
        RenderTarget2D buttonRenderTarget, buttonRenderTarget2, shadowRenderTarget;

        float sinShift;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            font1 = content.Load<SpriteFont>("InverseKinematics/debugFont");

            shadowShader = content.Load<Effect>("UIShadow/shadowEffect");

            shadowShader.Parameters["pixelSizeX"].SetValue(1.0f / Game1.graphics.PreferredBackBufferWidth);
            shadowShader.Parameters["pixelSizeY"].SetValue(1.0f / Game1.graphics.PreferredBackBufferHeight);

            buttonRenderTarget = new RenderTarget2D(
                Game1.graphics.GraphicsDevice,
                Game1.graphics.PreferredBackBufferWidth,
                Game1.graphics.PreferredBackBufferHeight);

            buttonRenderTarget2 = new RenderTarget2D(
                Game1.graphics.GraphicsDevice,
                Game1.graphics.PreferredBackBufferWidth,
                Game1.graphics.PreferredBackBufferHeight);

            shadowRenderTarget = new RenderTarget2D(
                Game1.graphics.GraphicsDevice,
                Game1.graphics.PreferredBackBufferWidth,
                Game1.graphics.PreferredBackBufferHeight,
                false, SurfaceFormat.Color, DepthFormat.Depth16, 1, RenderTargetUsage.PreserveContents);
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            BlendState ownState = new BlendState
            {
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.InverseSourceColor,

                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.InverseSourceColor
            };

            //clear rendertarget
            Game1.graphics.GraphicsDevice.SetRenderTarget(shadowRenderTarget);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            //draw the button
            Game1.graphics.GraphicsDevice.SetRenderTarget(buttonRenderTarget);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            //shadowShader.Parameters["mode"].SetValue(2);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, DepthStencilState.None, null);

            spriteBatch.Draw(sprWhite, new Rectangle(0, 0, 1200, 150), new Color(0, 90, 160));

            int posY = 250;
            Rectangle rec1 = new Rectangle(0, posY, 800, 130);
            Rectangle rec2 = new Rectangle(0, posY += 150, 800, 100);
            Rectangle rec3 = new Rectangle(0, posY += 120, 800, 100);

            spriteBatch.Draw(sprWhite, rec1, new Color(1, 111, 193));
            spriteBatch.Draw(sprWhite, rec2, new Color(1, 111, 193));
            spriteBatch.Draw(sprWhite, rec3, new Color(1, 111, 193));

            //spriteBatch.Draw(sprWhite, new Rectangle(20, 240, 1, 1), Color.Black);

            //delete stuff
            Game1.graphics.GraphicsDevice.BlendState = ownState;

            spriteBatch.DrawString(font1, "Test",
                new Vector2(600 - font1.MeasureString("Patetris").X * 10 / 2, 5),
                Color.White, 0, Vector2.Zero, 10, SpriteEffects.None, 0);

            spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);


            //draw the button
            Game1.graphics.GraphicsDevice.SetRenderTarget(buttonRenderTarget2);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, DepthStencilState.None, null);

            spriteBatch.DrawString(font1, "Start", new Vector2(rec1.Width / 2 - font1.MeasureString("Start").X * 8 / 2,
                rec1.Y + rec1.Height / 2 - font1.MeasureString("Start").Y * 8 / 2), Color.White, 0, Vector2.Zero, 8, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Highscore", new Vector2(rec2.Width / 2 - font1.MeasureString("Highscore").X * 6 / 2,
                rec2.Y + rec2.Height / 2 - font1.MeasureString("Highscore").Y * 6 / 2), Color.White, 0, Vector2.Zero, 6, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "HowTo", new Vector2(rec3.Width / 2 - font1.MeasureString("HowTo").X * 6 / 2,
                rec3.Y + rec3.Height / 2 - font1.MeasureString("HowTo").Y * 6 / 2), Color.White, 0, Vector2.Zero, 6, SpriteEffects.None, 0);

            spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            //draw shadows
            drawShadow(spriteBatch, buttonRenderTarget, 5);
            drawShadow(spriteBatch, buttonRenderTarget2, 3);

            //draw to the screen
            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            Game1.graphics.GraphicsDevice.Clear(new Color(247, 247, 247));
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            /*
            spriteBatch.Draw(buttonRenderTarget, new Vector2(5,5), Color.Black * 0.5f);
            spriteBatch.Draw(buttonRenderTarget, Vector2.Zero, Color.White);

            spriteBatch.Draw(buttonRenderTarget2, new Vector2(3, 3), Color.Black * 0.5f);
            spriteBatch.Draw(buttonRenderTarget2, Vector2.Zero, Color.White);
            */

            spriteBatch.Draw(buttonRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.Draw(buttonRenderTarget2, Vector2.Zero, Color.White);
            spriteBatch.Draw(shadowRenderTarget, Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        public void drawShadow(SpriteBatch spriteBatch, Texture2D sprMask, int depth)
        {
            //draw the shadow
            Game1.graphics.GraphicsDevice.SetRenderTarget(shadowRenderTarget);
            shadowShader.Parameters["depth"].SetValue(depth);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, shadowShader);
            spriteBatch.Draw(sprMask, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
