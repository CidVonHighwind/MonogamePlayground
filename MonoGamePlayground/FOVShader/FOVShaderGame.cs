using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.FOVShader
{
    class FOVShaderGame : IGame
    {
        KeyboardState keyboard = Keyboard.GetState();

        Texture2D sprWindow;
        Effect fovShader;

        bool enableShader = true;
        
        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWindow = content.Load<Texture2D>("FOVShader/gimpWindow");

            fovShader = content.Load<Effect>("FOVShader/fovShader");
            fovShader.Parameters["size"].SetValue(250);

            fovShader.Parameters["imageWidth"].SetValue(sprWindow.Width);
            fovShader.Parameters["imageHeight"].SetValue(sprWindow.Height);
        }

        public void Update(GameTime gameTime)
        {
            fovShader.Parameters["posX"].SetValue(Mouse.GetState().X);
            fovShader.Parameters["posY"].SetValue(Mouse.GetState().Y);

            if (keyboard.IsKeyDown(Keys.A) && !Keyboard.GetState().IsKeyDown(Keys.A))
                enableShader = !enableShader;

            fovShader.Parameters["enabled"].SetValue(enableShader);

            keyboard = Keyboard.GetState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            fovShader.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(sprWindow, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
