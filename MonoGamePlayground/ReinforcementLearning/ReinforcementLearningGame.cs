using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.ReinforcementLearning
{
    class Field
    {
        public int Reward;
        public int[] QFunction = { 0, 0, 0, 0 };
    }

    struct Player
    {
        public Point Position;
    }

    class ReinforcementLearningGame : IGame
    {
        private Texture2D sprWhite;

        Field[,] gameField = new Field[10, 10];
        Player player = new Player();

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            for (var y = 0; y < gameField.GetLength(1); y++)
                for (var x = 0; x < gameField.GetLength(0); x++)
                    gameField[x, y] = new Field();

            gameField[9, 0].Reward = 4096;
            gameField[9, 9].Reward = -4096;
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            var centerX = 1280 / 2 - gameField.GetLength(0) * 32 / 2;
            var centerY = 720 / 2 - gameField.GetLength(1) * 32 / 2;

            for (var y = 0; y < gameField.GetLength(1); y++)
            {
                for (var x = 0; x < gameField.GetLength(0); x++)
                {
                    spriteBatch.Draw(sprWhite, new Rectangle(centerX + x * 32, centerY + y * 32, 30, 30),
                        new Color(
                            1 - MathHelper.Clamp(gameField[x, y].Reward / 4096f, 0f, 1f),
                            1 - MathHelper.Clamp(-gameField[x, y].Reward / 4096f, 0f, 1f),
                            1 - MathHelper.Clamp(Math.Abs(gameField[x, y].Reward) / 4096f, 0f, 1f)));
                }
            }

            // draw the player
            spriteBatch.Draw(sprWhite, new Rectangle(
                centerX + player.Position.X * 32 + 2,
                centerY + player.Position.Y * 32 + 2, 26, 26), Color.Aqua);

            spriteBatch.End();
        }

        private Point[] directions = { new Point(1, 0), new Point(-1, 0), new Point(0, -1), new Point(0, 1) };

        public void QMove()
        {
            var bestMove = 0;
            var bestQ = int.MinValue;

            for (var i = 0; i < 4; i++)
            {
                var newPosition = player.Position + directions[i];

                // check if the move is valide
                if (newPosition.X >= 0 && newPosition.X < gameField.GetLength(0) &&
                    newPosition.Y >= 0 && newPosition.Y < gameField.GetLength(1))
                {
                    if (bestQ < gameField[newPosition.X, newPosition.Y].QFunction[i])
                    {
                        bestQ = gameField[newPosition.X, newPosition.Y].QFunction[i];
                        bestMove = i;
                    }
                }
            }
        }
    }
}
