using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground
{
    interface IGame
    {
        void Load(GraphicsDeviceManager graphics, ContentManager content);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
