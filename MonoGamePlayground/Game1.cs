using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePlayground.BatteryColor;
using MonoGamePlayground.BeansJam;
using MonoGamePlayground.BluredShadowShader;
using MonoGamePlayground.BlurShader;
using MonoGamePlayground.CircleShader;
using MonoGamePlayground.ECSTest;
using MonoGamePlayground.FOVShader;
using MonoGamePlayground.GameOfLifeShader;
using MonoGamePlayground.GBShader;
using MonoGamePlayground.GraphParty;
using MonoGamePlayground.MatrixCam;
using MonoGamePlayground.PaletteConverter;
using MonoGamePlayground.PhysicsTest;
using MonoGamePlayground.Picross;
using MonoGamePlayground.ReinforcementLearning;
using MonoGamePlayground.SpriteShadow;
using MonoGamePlayground.InverseKinematics;
using MonoGamePlayground.MandelbrotShader;
using MonoGamePlayground.SpriteShadowShader;
using MonoGamePlayground.UIShadowShader;

namespace MonoGamePlayground
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<IGame> gameList = new List<IGame>();

        private Vector2 WindowSize;

        private int selectedGame = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameList.Add(new PathfindingGame());

            gameList.Add(new BlurShaderGame());
            gameList.Add(new InverseKinematicsGame());

            gameList.Add(new GameOfLifeShaderGame());
            gameList.Add(new MandelbrotShaderGame());
            gameList.Add(new UIShadowShaderGame());
            gameList.Add(new SpriteShadowShaderGame());
            gameList.Add(new SpriteShadowGame());
            gameList.Add(new FlashlightShaderGame());
            gameList.Add(new FOVShaderGame());
            gameList.Add(new GBShaderGame());
            gameList.Add(new BluredShadowShaderGame());
            gameList.Add(new CircleShaderGame());

            gameList.Add(new PaletteConverterGame());
            gameList.Add(new ReinforcementLearningGame());
            gameList.Add(new MatrixCamGame());
            gameList.Add(new ECSGame());
            gameList.Add(new PicrossGame());
            gameList.Add(new PhysicsTestGame());
            gameList.Add(new BatteryColorGame());
            gameList.Add(new GraphPartyGame());
            gameList.Add(new BeansJamGame());

            // load the games data
            foreach (var game in gameList)
            {
                game.Load(graphics, Content);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gameList[selectedGame].Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            gameList[selectedGame].Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
