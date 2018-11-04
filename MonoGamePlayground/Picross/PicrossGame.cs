using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.Picross
{
    class PicrossGame : IGame
    {
        private MouseState mouseState;

        private Texture2D sprWhite;
        private SpriteFont font1;

        int[,] field = new int[5, 5];
        int[,] solution = new int[5, 5];

        private int[][] leftNumbers = new int[5][];
        private int[][] topNumbers = new int[5][];

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new RenderTarget2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[] { Color.White });

            font1 = content.Load<SpriteFont>("Picross/font1");
        }

        public void Update(GameTime gameTime)
        {
            int posX = (Mouse.GetState().X - 100) / 40;
            int posY = (Mouse.GetState().Y - 100) / 40;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                if (posX >= 0 && posX < field.GetLength(0) &&
                    posY >= 0 && posY < field.GetLength(1))
                {
                    if (field[posX, posY] == 0)
                        field[posX, posY] = 1;
                    else
                        field[posX, posY] = 0;
                }
            }

            mouseState = Mouse.GetState();

            UpdateNumbers();

            Solve();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // draw the field
            for (var y = 0; y < field.GetLength(1); y++)
            {
                for (var x = 0; x < field.GetLength(0); x++)
                {
                    spriteBatch.Draw(sprWhite, new Rectangle(100 + x * 40, 100 + y * 40, 40, 40),
                        (x + y * field.GetLength(0)) % 2 == 0 ? Color.White : Color.LightBlue);

                    if (field[x, y] == 1)
                        spriteBatch.Draw(sprWhite, new Rectangle(100 + 5 + x * 40, 100 + 5 + y * 40, 30, 30),
                            Color.DarkGray);
                }
            }

            for (var y = 0; y < field.GetLength(1); y++)
            {
                for (var x = 0; x < field.GetLength(0); x++)
                {
                    if (solution[x, y] == 1)
                        spriteBatch.Draw(sprWhite, new Rectangle(100 + x * 40, 100 + y * 40, 40, 40),
                        Color.Red * 0.1f);
                    // save
                    if (solution[x, y] == 2)
                        spriteBatch.Draw(sprWhite, new Rectangle(100 + x * 40, 100 + y * 40, 40, 40),
                            Color.Green * 0.3f);
                }
            }

            // draw the top numbers
            for (int x = 0; x < topNumbers.Length; x++)
            {
                for (int y = 0; y < topNumbers[x].Length; y++)
                {
                    spriteBatch.DrawString(font1, topNumbers[x][y].ToString(),
                        new Vector2(100 + 10 + x * 40, 100 - topNumbers[x].Length * 15 + y * 15), Color.Black);
                }
            }
            // draw the left numbers
            for (int x = 0; x < leftNumbers.Length; x++)
            {
                for (int y = 0; y < leftNumbers[x].Length; y++)
                {
                    spriteBatch.DrawString(font1, leftNumbers[x][y].ToString(),
                        new Vector2(100 - leftNumbers[x].Length * 15 + y * 15, 100 + 10 + x * 40), Color.Black);
                }
            }

            spriteBatch.End();
        }

        public void UpdateNumbers()
        {
            bool isField;
            List<int> numbers = new List<int>();
            int count;

            for (var x = 0; x < field.GetLength(0); x++)
            {
                numbers.Clear();
                isField = false;
                count = 0;

                for (var y = 0; y < field.GetLength(1); y++)
                {
                    if (isField && field[x, y] == 0)
                    {
                        numbers.Add(count);
                        count = 0;
                    }

                    isField = field[x, y] == 1;
                    if (isField)
                        count++;
                }

                if (isField)
                    numbers.Add(count);

                topNumbers[x] = numbers.ToArray();
            }

            for (var y = 0; y < field.GetLength(1); y++)
            {
                numbers.Clear();
                isField = false;
                count = 0;

                for (var x = 0; x < field.GetLength(0); x++)
                {
                    if (isField && field[x, y] == 0)
                    {
                        numbers.Add(count);
                        count = 0;
                    }

                    isField = field[x, y] == 1;
                    if (isField)
                        count++;
                }

                if (isField)
                    numbers.Add(count);

                leftNumbers[y] = numbers.ToArray();
            }
        }

        public void Solve()
        {
            for (var y = 0; y < field.GetLength(1); y++)
                for (var x = 0; x < field.GetLength(0); x++)
                    solution[x, y] = 1;

            for (var y = 0; y < field.GetLength(1); y++)
            {
                if (leftNumbers[y].Length == 0)
                    for (var x = 0; x < field.GetLength(0); x++)
                        solution[x, y] = 0;
            }

            for (var x = 0; x < field.GetLength(0); x++)
            {
                if (topNumbers[x].Length == 0)
                    for (var y = 0; y < field.GetLength(0); y++)
                        solution[x, y] = 0;
            }

            bool changed;

            do
            {
                changed = false;

                for (var y = 0; y < field.GetLength(1); y++)
                {
                    if (getCount(y, true).SequenceEqual(leftNumbers[y]))
                    {
                        if (markLine(y, 1, 2, true))
                            changed = true;
                    }
                }
                for (var x = 0; x < field.GetLength(0); x++)
                {
                    if (getCount(x, false).SequenceEqual(topNumbers[x]))
                    {
                        if (markLine(x, 1, 2, false))
                            changed = true;
                    }
                }

                // delete already solved fields
                for (var y = 0; y < field.GetLength(1); y++)
                {
                    if (getSaveSolutions(y, true).SequenceEqual(leftNumbers[y]))
                    {
                        markLine(y, 1, 0, true);
                    }
                }
                for (var x = 0; x < field.GetLength(0); x++)
                {
                    if (getSaveSolutions(x, false).SequenceEqual(topNumbers[x]))
                    {
                        markLine(x, 1, 0, false);
                    }
                }
            } while (changed);
        }

        public bool markLine(int line, int from, int to, bool horizontal)
        {
            bool changed = false;

            if (!horizontal)
                for (var y = 0; y < field.GetLength(1); y++)
                {
                    if (solution[line, y] == from)
                    {
                        solution[line, y] = to;
                        changed = true;
                    }
                }
            else
                for (var x = 0; x < field.GetLength(1); x++)
                {
                    if (solution[x, line] == from)
                    {
                        solution[x, line] = to;
                        changed = true;
                    }
                }

            return changed;
        }

        public int[] getCount(int line, bool horizontal)
        {
            List<int> output = new List<int>();
            int count = 0;
            bool marked = false;

            if (horizontal)
            {
                for (var x = 0; x < field.GetLength(0); x++)
                {
                    if (marked && solution[x, line] == 0)
                    {
                        output.Add(count);
                        count = 0;
                    }

                    marked = solution[x, line] > 0;

                    if (solution[x, line] > 0)
                        count++;
                }

                if (marked)
                    output.Add(count);
            }
            else
            {
                for (var y = 0; y < field.GetLength(1); y++)
                {
                    if (marked && solution[line, y] == 0)
                    {
                        output.Add(count);
                        count = 0;
                    }

                    marked = solution[line, y] > 0;

                    if (solution[line, y] > 0)
                        count++;
                }

                if (marked)
                    output.Add(count);
            }

            return output.ToArray();
        }

        public int[] getSaveSolutions(int line, bool horizontal)
        {
            List<int> output = new List<int>();
            int count = 0;
            bool marked = false;

            if (horizontal)
            {
                for (var x = 0; x < field.GetLength(0); x++)
                {
                    if (marked && solution[x, line] != 2)
                    {
                        output.Add(count);
                        count = 0;
                    }

                    marked = solution[x, line] == 2;

                    if (solution[x, line] == 2)
                        count++;
                }

                if (marked)
                    output.Add(count);
            }
            else
            {
                for (var y = 0; y < field.GetLength(1); y++)
                {
                    if (marked && solution[line, y] != 2)
                    {
                        output.Add(count);
                        count = 0;
                    }

                    marked = solution[line, y] == 2;

                    if (solution[line, y] == 2)
                        count++;
                }

                if (marked)
                    output.Add(count);
            }

            return output.ToArray();
        }
    }
}
