using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.MatrixCam
{
    public class Player
    {
        public Vector2 position = new Vector2(0, -8);
    }

    class MatrixCamGame : IGame
    {
        private Texture2D sprImage, sprLink, sprTile, sprWhite;
        private SpriteFont font;
        Matrix cameraMatrix;
        private CameraNew camera;
        private Player player = new Player();

        public static string strDebug;
        private int counter;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            font = content.Load<SpriteFont>("MatrixCam/font");

            sprImage = content.Load<Texture2D>("MatrixCam/pic2");
            sprLink = content.Load<Texture2D>("MatrixCam/link");
            sprTile = content.Load<Texture2D>("MatrixCam/tile");

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            camera = new CameraNew(graphics.GraphicsDevice.Viewport);
        }

        public void Update(GameTime gameTime)
        {            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            counter += gameTime.ElapsedGameTime.Milliseconds;

            var vecMovement = Vector2.Zero;
            var gamepadMovement = Vector2.Zero;

            gamepadMovement.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            gamepadMovement.Y -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;

            //gamepadMovement.X = MathHelper.Clamp(gamepadMovement.X, -1, 1);
            //gamepadMovement.Y = MathHelper.Clamp(gamepadMovement.Y, -1, 1);

            var speed = 0.0625f;
            speed = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                vecMovement.X += speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                vecMovement.X -= speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                vecMovement.Y -= speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                vecMovement.Y += speed;

            if (vecMovement.Length() < 0.1f)
                vecMovement = Vector2.Zero;
            else
                vecMovement.Normalize();

            vecMovement += gamepadMovement;

            player.position += vecMovement;

            //camera.Zoom = (float)Math.Sin(counter / 250f) * 2 + 2;
            //camera.Location = new Vector2(sprImage.Width / 2, sprImage.Height / 2);

            camera.SetBounds(Game1.graphics.GraphicsDevice.Viewport);
            camera.Center(new Vector2(
                              player.position.X,
                              player.position.Y) * 4 + new Vector2(6, 8) * 4, vecMovement * 4);
        }

        public void Draw(SpriteBatch spriteBatch)
        {            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.TransformMatrix);


            for (var y = 0; y < 100; y++)
            {
                for (var x = 0; x < 100; x++)
                {
                    spriteBatch.Draw(sprTile, new Vector2(x * 16, y * 16), new Rectangle(8, 8, 16, 16), Color.White);
                }
            }

            spriteBatch.Draw(sprImage,
                Vector2.Zero,
                Color.White);


            //spriteBatch.Draw(sprLink, new Vector2(
            //    camera.ToWorldCoords(player.position.X),
            //    camera.ToWorldCoords(player.position.Y)), Color.White);


            var px = (int)Math.Round(player.position.X * 4, 0, MidpointRounding.AwayFromZero);
            var py = (int)Math.Round(player.position.Y * 4, 0, MidpointRounding.AwayFromZero);

            spriteBatch.Draw(sprLink, new Vector2(
                px / 4f,
                py / 4f), Color.White);

            //spriteBatch.Draw(sprLink, new Vector2(
            //    player.position.X,
            //    player.position.Y), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearClamp);

            // draw center
            // spriteBatch.Draw(sprWhite, new Rectangle(496, 498, 8, 8), Color.Green);

            // (int)(player.position.X * 4) + (((int)player.position.X * 4) % 1 >= 0.5f ? 1 : 0);
            spriteBatch.DrawString(font, strDebug + "\n" + px, Vector2.Zero, Color.White);

            spriteBatch.Draw(sprWhite, new Vector2(10 + (float)Math.Sin(counter / 250f), 100), Color.White);

            spriteBatch.End();
        }
    }
}
