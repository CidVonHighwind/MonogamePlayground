using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePlayground.SpriteShadow
{
    class SpriteShadowGame : IGame
    {
        private Texture2D sprTexture, sprLink;
        private Effect shadowEffect;

        private VertexPositionTexture[] _vertexArray;

        private int playerPositionX;
        private int playerPositionY;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            shadowEffect = content.Load<Effect>("SpriteShadow/Shadow");

            sprTexture = content.Load<Texture2D>("SpriteShadow/test");
            sprLink = content.Load<Texture2D>("SpriteShadow/link0");

            playerPositionX = 1280 / 2 - 140 / 2;
            playerPositionY = 720 / 2 - 150 / 2;
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawShadow();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            
            spriteBatch.Draw(sprLink, new Rectangle(
                playerPositionX, playerPositionY, 140, 150), new Rectangle(20, 46, 14, 15), Color.White);

            spriteBatch.End();
        }

        public void DrawShadow()
        {
            Game1.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game1.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Game1.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Transform your model to place it somewhere in the world
            var basicEffect = new BasicEffect(Game1.graphics.GraphicsDevice);

            basicEffect.World = Matrix.CreateScale(1) *
                                Matrix.CreateTranslation(new Vector3(-1280 / 2, -720 / 2, 0));

            basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, -1), Vector3.Zero, Vector3.Down);
            basicEffect.Projection = Matrix.CreateOrthographic(1280, 720, -1, 1000);

            Game1.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            basicEffect.TextureEnabled = true;

            _vertexArray = GetVertexPt(sprLink.Width, sprLink.Height, 0.5f, 0.5f, 
                new Rectangle(playerPositionX, playerPositionY, 140, 150), new Rectangle(20, 46, 14, 15));

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                shadowEffect.CurrentTechnique.Passes[0].Apply();

                Game1.graphics.GraphicsDevice.Textures[0] = sprLink;
                Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertexArray, 0, 2);
            }
        }

        public VertexPositionTexture[] GetVertexPt(int textureWidth, int textureHeight, float rotX, float height, Rectangle destination, Rectangle source)
        {
            _vertexArray = new VertexPositionTexture[6];

            var posLeft = destination.Left;
            var posRight = destination.Right;
            var posTop = destination.Top;
            var posBottom = destination.Bottom;

            var offsetX = destination.Width * rotX;
            var offsetY = destination.Height * height;

            var vecPos = new[]
            {
                new Vector3(posLeft + offsetX, posTop + offsetY, 0),
                new Vector3(posRight + offsetX, posTop + offsetY, 0),
                new Vector3(posLeft, posBottom, 0),
                new Vector3(posLeft, posBottom, 0),
                new Vector3(posRight + offsetX, posTop + offsetY, 0),
                new Vector3(posRight, posBottom, 0)
            };

            var left = source.X / (float)textureWidth;
            var top = source.Y / (float)textureHeight;
            var right = source.Right / (float)textureWidth;
            var bottom = source.Bottom / (float)textureHeight;

            var texCoord = new[]
            {
                new Vector2(left, top), new Vector2(right, top), new Vector2(left, bottom),
                new Vector2(left, bottom), new Vector2(right, top), new Vector2(right, bottom)
            };

            for (var i = 0; i < _vertexArray.Length; i++)
            {
                _vertexArray[i] = new VertexPositionTexture(vecPos[i], texCoord[i]);
            }

            return _vertexArray;
        }
    }
}
