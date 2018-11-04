using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.BeansJam
{
    public class BeansJamGame : IGame
    {
        public class Planet
        {
            public Vector2 position;
            public float radius, rotation;
        }

        public static KeyboardState keyboardState;
        public static Texture2D sprWhite, sprRocket, sprPlanet;
        public static List<Planet> planets = new List<Planet>();

        Player player = new Player();
        Vector2 cameraPosition;

        float cameraScale = 1;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            sprRocket = new Texture2D(graphics.GraphicsDevice, 2, 1);
            sprRocket.SetData(new[] { Color.Red, Color.White });

            sprPlanet = content.Load<Texture2D>("BeansJam/planet");

            planets.Add(new Planet() { position = new Vector2(0, 0), radius = 75 });
            planets.Add(new Planet() { position = new Vector2(600, -150), radius = 75 });
        }

        void IGame.Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            player.Update(gameTime);

            if (keyboardState.IsKeyDown(Keys.X))
                cameraScale -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.C))
                cameraScale += 0.01f;

            cameraPosition.X = 500 / cameraScale;
            cameraPosition.Y = 400 / cameraScale;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (var i = 0; i < planets.Count; i++)
            {
                spriteBatch.Draw(sprPlanet, new Rectangle(
                        (int)((planets[i].position.X + cameraPosition.X) * cameraScale),
                        (int)((planets[i].position.Y + cameraPosition.Y) * cameraScale),
                        (int)(planets[i].radius * 2 * cameraScale),
                        (int)(planets[i].radius * 2 * cameraScale)),
                    new Rectangle(0, 0, 1010, 1010), Color.White, 0, new Vector2(505, 505), SpriteEffects.None, 0);
            }

            player.Draw(spriteBatch, cameraScale, cameraPosition);

            spriteBatch.End();
        }
    }
}
