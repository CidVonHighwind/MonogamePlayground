using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.GraphParty
{
    class GraphPartyGame : IGame
    {
        private Texture2D _graphTexture;

        public const int Width = 1280;
        public const int Height = 720;
        private readonly Color[] _colorData = new Color[Width * Height];

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            _graphTexture = new Texture2D(graphics.GraphicsDevice, Width, Height);
        }

        public void Update(GameTime gameTime)
        {
            var mixState = (float)Math.Sin((gameTime.TotalGameTime.Milliseconds + gameTime.TotalGameTime.Seconds * 1000) / 250f) / 2f + 0.5f;

            _graphTexture.SetData(getGraphData(_colorData, Width, Height, Width * 2, 10, -10, 10, -150, 150, (x) => x * x, (x) => x * x * x, mixState));

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null);

            spriteBatch.Draw(_graphTexture, new Rectangle(0, 0, _graphTexture.Width, _graphTexture.Height), Color.White);

            spriteBatch.End();
        }

        private delegate double GraphFunction(double positionX);

        private Color[] getGraphData(Color[] colorData, int width, int height, int samples, double tickness,
            double left, double right, double top, double bottom,
            GraphFunction firstFunction, GraphFunction secondFunction, float mixState)
        {
            // clear the background
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                colorData[x + y * width] = Color.Transparent;


            for (var x = 0; x < samples; x++)
            {
                var posX = (x / (double)samples) * width;

                var firstValue = firstFunction((posX / width) * (right - left) + left);
                var secondValue = secondFunction((posX / width) * (right - left) + left);
                var valueMix = firstValue * mixState + secondValue * (1 - mixState);

                var value = (valueMix / (top - bottom) - (bottom / (top - bottom))) * height;

                for (var i = 0; i < tickness; i++)
                {
                    var graphValue = value + (-(tickness - 1) / 2 + i);
                    if (Math.Round(graphValue) >= 0 && Math.Round(graphValue) < height)
                    {
                        colorData[(int)Math.Round(posX) + ((int)Math.Round(graphValue) * width)] = Color.Red *
                                                                                                   (float)(1 - Math.Abs(value - (int)Math.Round(graphValue)) / tickness);// * Math.Abs(posX - (int)Math.Round(posX)));
                    }
                }
            }

            return colorData;
        }
    }
}
