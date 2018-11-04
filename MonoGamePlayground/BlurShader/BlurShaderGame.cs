using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.BlurShader
{
    class BlurShaderGame : IGame
    {
        MouseState lastMouseState = Mouse.GetState();

        Texture2D sprImage, sprWhite;
        SpriteFont font0;
        Effect blurEffectH;
        Effect blurEffectV;

        private Stopwatch stopwatch = new Stopwatch();

        RenderTarget2D renderTargetSmall, renderTarget0, renderTarget1;
        List<double> timeSmother = new List<double>();

        private int size = 3, mode;
        private int blurCount = 50, currentBCount;

        private int[] _blurTime = new int[30];
        private int _timeIndex = 0;

        private bool init = false;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            font0 = content.Load<SpriteFont>("BlurShader/font1");
            sprImage = content.Load<Texture2D>("BlurShader/game");

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[1] { Color.White });

            blurEffectH = content.Load<Effect>("BlurShader/BlurH");
            blurEffectV = content.Load<Effect>("BlurShader/BlurV");

            renderTargetSmall = new RenderTarget2D(graphics.GraphicsDevice,
                graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            renderTarget0 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            renderTarget1 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            blurEffectH.Parameters["pixelX"].SetValue(1.0f / renderTarget0.Width);
            blurEffectH.Parameters["pixelY"].SetValue(1.0f / renderTarget0.Height);
            blurEffectH.Parameters["width"].SetValue(graphics.PreferredBackBufferWidth);
            blurEffectH.Parameters["height"].SetValue(graphics.PreferredBackBufferHeight);

            blurEffectV.Parameters["pixelX"].SetValue(1.0f / renderTarget0.Width);
            blurEffectV.Parameters["pixelY"].SetValue(1.0f / renderTarget0.Height);
            blurEffectV.Parameters["width"].SetValue(graphics.PreferredBackBufferWidth);
            blurEffectV.Parameters["height"].SetValue(graphics.PreferredBackBufferHeight);
        }

        public void Update(GameTime gameTime)
        {
            if (lastMouseState.ScrollWheelValue > Mouse.GetState().ScrollWheelValue)
                blurCount++;
            if (lastMouseState.ScrollWheelValue < Mouse.GetState().ScrollWheelValue)
                blurCount--;

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                mode = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
                mode = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3))
                mode = 2;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                currentBCount = 0;
                init = false;
            }

            lastMouseState = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            stopwatch.Reset();
            stopwatch.Start();

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTargetSmall);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp);
            spriteBatch.Draw(sprImage, new Rectangle(0, 0, sprImage.Width / 2, sprImage.Height / 2), Color.White);
            spriteBatch.End();

            //v blur
            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget0);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, blurEffectH, null);
            spriteBatch.Draw(renderTargetSmall, new Rectangle(0, 0, renderTarget0.Width, renderTarget0.Height), Color.White);
            spriteBatch.End();

            //h blur
            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget1);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, blurEffectV, null);
            spriteBatch.Draw(renderTarget0, new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height), Color.White);
            spriteBatch.End();

            stopwatch.Stop();

            _blurTime[_timeIndex] = (int)stopwatch.ElapsedTicks;
            _timeIndex = (_timeIndex + 1) % 30;
            var averageTime = 0;
            foreach (var time in _blurTime)
                averageTime += time;
            averageTime /= 30;

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp);

            spriteBatch.Draw(renderTarget1, Vector2.Zero, Color.White);

            spriteBatch.DrawString(font0, "time: " + averageTime, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
