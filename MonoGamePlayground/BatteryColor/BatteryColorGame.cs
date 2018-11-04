using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.BatteryColor
{
    class BatteryColorGame : IGame
    {
        private Texture2D sprWhite;
        private float stateMoveDir = 1;
        private float currentBatteryState;

        private Vector3 colorFull = new Vector3(0.0f, 0.78f, 0.078f);
        private Vector3 colorEmpty = new Vector3(0.749f, 0.11f, 0.176f);

        private float colorCount = 5;
        private Vector3[] colors = {
            new Vector3(0.745f, 0.114f, 0.176f),
            //new Vector3(0.922f, 0.11f, 0.141f),
            new Vector3(0.92f, 0.361f, 0.176f),
            new Vector3(0.976f, 0.69f, 0.255f),
            new Vector3(0.545f, 0.769f, 0.247f),
            new Vector3(0.545f, 0.769f, 0.247f),
            new Vector3(0.0f, 0.78f, 0.078f),
            new Vector3(0.0f, 0.78f, 0.078f)
        };

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            currentBatteryState += stateMoveDir * 0.001f;
            if (currentBatteryState < 0)
            {
                currentBatteryState = 0;
                stateMoveDir = -stateMoveDir;
            }
            else if (currentBatteryState > 1)
            {
                currentBatteryState = 1;
                stateMoveDir = -stateMoveDir;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(sprWhite, new Rectangle(50, 50, 250, 100), Color.White);

            var colorState = (currentBatteryState % (1 / (colorCount))) / (1 / colorCount);
            var currentColor = (int)(currentBatteryState / (1 / colorCount));
            var batteryColor = new Color(colors[currentColor] * (1 - colorState) + colors[currentColor + 1] * colorState);
            var width = (int)(currentBatteryState * 240);
            spriteBatch.Draw(sprWhite, new Rectangle(55 + 240 - width, 55, width, 90), batteryColor);

            spriteBatch.End();
        }
    }
}
