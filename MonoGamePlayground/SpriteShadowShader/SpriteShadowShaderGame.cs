using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.SpriteShadowShader
{
    class SpriteShadowShaderGame : IGame
    {
        private Texture2D sprTexture2D;
        Color grassColor = new Color(98, 189, 32);
        Rectangle recPlayer = new Rectangle(0, 16, 16, 16);
        private Effect shadowShader;

        private float dir = 1, height = 0.5f;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprTexture2D = content.Load<Texture2D>("2DShadow/test");
            shadowShader = content.Load<Effect>("2DShadow/shadow");
        }

        public void Update(GameTime gameTime)
        {
            if (height < 0)
                dir = -dir;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                dir -= 0.05f;
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
                dir += 0.05f;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                height -= 0.05f;
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
                height += 0.05f;

            //height = 0.5f - Math.Abs(dir) / 3.5f * 0.5f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Point position = new Point(Mouse.GetState().X, Mouse.GetState().Y);

            Game1.graphics.GraphicsDevice.Clear(grassColor);

            shadowShader.Parameters["direction"].SetValue(dir);
            shadowShader.Parameters["size"].SetValue(height);
            shadowShader.Parameters["mirror"].SetValue(true);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, shadowShader);

            shadowShader.Parameters["posX"].SetValue(recPlayer.X / (float)sprTexture2D.Width);
            shadowShader.Parameters["posY"].SetValue(recPlayer.Y / (float)sprTexture2D.Height);
            shadowShader.Parameters["width"].SetValue(recPlayer.Width / (float)sprTexture2D.Width);
            shadowShader.Parameters["height"].SetValue(recPlayer.Height / (float)sprTexture2D.Height);

            // draw the player
            spriteBatch.Draw(sprTexture2D, new Rectangle(position.X - recPlayer.Width * 4, position.Y, recPlayer.Width * 12, recPlayer.Height * 8),
                new Rectangle(-sprTexture2D.Width, 0, sprTexture2D.Width * 3, sprTexture2D.Height * 2), Color.Black * 0.75f);


            //spriteBatch.Draw(sprTexture2D, new Rectangle(position.X, position.Y, recPlayer.Width * 4, recPlayer.Height * 4), recPlayer, Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

            // draw the player
            if (Mouse.GetState().LeftButton != ButtonState.Pressed)
                spriteBatch.Draw(sprTexture2D, new Rectangle(position.X, position.Y, recPlayer.Width * 4, recPlayer.Height * 4), recPlayer, Color.White,
                    0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

            spriteBatch.End();
        }
    }
}
