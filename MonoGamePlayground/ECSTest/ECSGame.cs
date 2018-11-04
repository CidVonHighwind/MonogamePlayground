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

namespace MonoGamePlayground.ECSTest
{
    class TickCounter
    {
        public int AverageTime;

        private readonly int[] _timeCounts;
        private int _currentTimeState;

        public TickCounter(int average)
        {
            _timeCounts = new int[average];
        }

        public void AddTick(int tick)
        {
            _timeCounts[_currentTimeState] = tick;

            _currentTimeState++;
            if (_currentTimeState >= _timeCounts.Length)
            {
                _currentTimeState = 0;
            }

            AverageTime = (int)_timeCounts.Average();
        }
    }

    class Ball : IComparable
    {
        public Vector2 pos;
        public Vector2 vel;
        public Color color;
        public Rectangle rec;
        public bool moving;

        public int CompareTo(object obj)
        {
            if (obj is Ball ball)
            {
                return (int)(pos.Y - ball.pos.Y);
            }

            return 0;
        }
    }

    class ECSGame : IGame
    {
        public static Texture2D sprWhite;
        private SpriteFont font;

        public static Random rDom = new Random();

        private readonly List<GameObject> goPool = new List<GameObject>();
        private readonly List<IUpdatable> updatablePool = new List<IUpdatable>();
        private readonly List<IDrawable> drawablePool = new List<IDrawable>();

        private CollisionSystem collisionSystem;
        private MovementSystem movementSystem;
        
        private readonly TickCounter updateTicks = new TickCounter(60);
        private readonly TickCounter poolUpdateTicks = new TickCounter(60);

        //private List<Ball> balls = new List<Ball>();
        private Ball[] balls;

        private Ball[,][] pools;

        private int[,] poolIndex;

        private int poolWidth, poolHeight;
        private int poolX, poolY;

        private readonly Stopwatch stopwatch = new Stopwatch();

        Vector2 vec1 = new Vector2(0, 0);
        private int width, height;
        
        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            font = content.Load<SpriteFont>("ECSTest/font");

            collisionSystem = new CollisionSystem();
            movementSystem = new MovementSystem();

            for (int i = 0; i < 500; i++)
            {
                AddToPool(new Minion());
            }
            
            //AddToPool(new Minion(100,200, 3,0));
            //AddToPool(new Minion(300, 201, -3, 0));

            int count = 1000;
            balls = new Ball[count];

            for (int i = 0; i < count; i++)
            {
                var ball = new Ball
                {
                    pos = new Vector2(rDom.Next(0, 769), rDom.Next(0, 769)),
                    vel = new Vector2(rDom.Next(0, 500) / 100f - 2.5f, rDom.Next(0, 500) / 100f - 2.5f),
                    color = new Color(rDom.Next(0, 255), rDom.Next(0, 255), rDom.Next(0, 255)),
                    rec = new Rectangle(0, 0, 8, 8),
                    moving = i >= 500
                };

                //balls.Add(ball);
                balls[i] = ball;
            }

            //var ball = new Ball
            //{
            //    pos = new Vector2(0, 10),
            //    vel = new Vector2(5, 0),
            //    color = new Color(rDom.Next(0, 255), rDom.Next(0, 255), rDom.Next(0, 255)),
            //    rec = new Rectangle(0, 0, 30, 30)
            //};
            //balls[0] = ball;
            //ball = new Ball
            //{
            //    pos = new Vector2(769, 10),
            //    vel = new Vector2(-5, 0),
            //    color = new Color(rDom.Next(0, 255), rDom.Next(0, 255), rDom.Next(0, 255)),
            //    rec = new Rectangle(0, 0, 30, 30)
            //};
            //balls[1] = ball;

            poolX = 16;
            poolY = 16;

            poolWidth = 800 / poolX;
            poolHeight = 800 / poolY;

            poolIndex = new int[poolX, poolY];
            pools = new Ball[poolX, poolY][];
            for (int y = 0; y < pools.GetLength(1); y++)
            {
                for (int x = 0; x < pools.GetLength(0); x++)
                {
                    pools[x, y] = new Ball[count];
                }
            }
        }

        public void Update(GameTime gameTime)
        {            // Times:
            // first:   16500   ticks
            // second:   6000   ticks
            // third:    6100   ticks (added speed)

            // object for loops
            // list      5950   ticks
            // array     3900   ticks

            // list test:
            // for:      2200
            // foreach:  1970
            // array test
            // for:      1530
            // foreach:  1360

            stopwatch.Reset();
            stopwatch.Start();

            UpdatePools();

            stopwatch.Stop();
            poolUpdateTicks.AddTick((int)stopwatch.ElapsedTicks);

            stopwatch.Reset();
            stopwatch.Start();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                foreach (var go in updatablePool)
                {
                    go.Update();
                }

                collisionSystem.Update();

                movementSystem.Update();
            }
            else
            {
                UpdateBalls();
            }

            stopwatch.Stop();

            updateTicks.AddTick((int)stopwatch.ElapsedTicks);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (int x = 0; x < poolX; x++)
            {
                for (int y = 0; y < poolY; y++)
                {
                    spriteBatch.Draw(sprWhite, new Rectangle(
                            poolWidth * x, poolHeight * y, poolWidth, poolHeight), (x + y) % 2 == 0 ? Color.White : Color.LightGray);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                for (int i = 0; i < drawablePool.Count; i++)
                {
                    drawablePool[i].Draw(spriteBatch);
                }
            }
            else
            {
                for (int i = 0; i < balls.Length; i++)
                    spriteBatch.Draw(sprWhite, new Rectangle(
                        (int)balls[i].pos.X,
                        (int)balls[i].pos.Y, balls[i].rec.Width, balls[i].rec.Height), balls[i].color);
            }

            spriteBatch.DrawString(font, "time: " + updateTicks.AverageTime + "\npool: " + poolUpdateTicks.AverageTime, Vector2.One, Color.Green);

            spriteBatch.End();
        }

        public void UpdatePools()
        {
            width = 1280;
            height = 720;

            poolWidth = 1280 / poolX;
            poolHeight = 720 / poolY;

            /*
            width = GraphicsDevice.Viewport.Width - 1;
            height = GraphicsDevice.Viewport.Height - 1;

            poolWidth = GraphicsDevice.Viewport.Width / poolX;
            poolHeight = GraphicsDevice.Viewport.Height / poolY;
            */

            for (var y = 0; y < poolIndex.GetLength(1); y++)
                for (var x = 0; x < poolIndex.GetLength(0); x++)
                    poolIndex[x, y] = 0;

            Point[] offsets = { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) };

            int posX, posY, lastPosX, lastPosY, aCount;

            foreach (var ball in balls)
            {
                aCount = 0;
                lastPosX = -99;
                lastPosY = -99;

                for (var j = 0; j < offsets.Length; j++)
                {
                    posX = (int)((ball.pos.X + offsets[j].X * ball.rec.Width) / poolWidth);
                    posY = (int)((ball.pos.Y + offsets[j].Y * ball.rec.Height) / poolHeight);

                    if (posX < poolX && posY < poolY && (j == 0 ||
                        (j == 1 && posX != lastPosX) ||
                        (j == 2 && posY != lastPosY) ||
                        aCount == 3))
                    {
                        pools[posX, posY][poolIndex[posX, posY]] = ball;
                        poolIndex[posX, posY]++;
                        aCount++;
                    }

                    lastPosX = posX;
                    lastPosY = posY;
                }
            }
        }

        public void UpdateBalls()
        {
            foreach (var ball in balls)
            {
                if (ball.moving)
                {
                    ball.pos += ball.vel;

                    // Move?
                    if (ball.pos.X <= 0)
                    {
                        ball.pos.X = 0;
                        ball.vel.X = -ball.vel.X;
                    }
                    else if (ball.pos.X >= width - ball.rec.Width)
                    {
                        ball.pos.X = width - ball.rec.Width;
                        ball.vel.X = -ball.vel.X;
                    }

                    if (ball.pos.Y <= 0)
                    {
                        ball.pos.Y = 0;
                        ball.vel.Y = -ball.vel.Y;
                    }
                    else if (ball.pos.Y >= height - ball.rec.Height)
                    {
                        ball.pos.Y = height - ball.rec.Height;
                        ball.vel.Y = -ball.vel.Y;
                    }
                }

                ball.rec.X = (int)ball.pos.X;
                ball.rec.Y = (int)ball.pos.Y;

                // new PRectangle((int)balls[i].pos.X, (int)balls[i].pos.Y, 8, 8);
                //balls[i].rec.Update();
            }

            // Array.Sort(balls);


            int posX, posY, lastPosX, lastPosY, aCount;

            Point[] offsets = { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) };

            // check for collision
            for (var i = 0; i < balls.Length; i++)
            {
                if (!balls[i].moving)
                    continue;

                aCount = 0;
                lastPosX = -99;
                lastPosY = -99;

                for (int y = 0; y < offsets.Length; y++)
                {
                    posX = (int)(((int)balls[i].pos.X + offsets[y].X * balls[i].rec.Width) / poolWidth);
                    posY = (int)(((int)balls[i].pos.Y + offsets[y].Y * balls[i].rec.Height) / poolHeight);

                    if (posX < poolX && posY < poolY && (y == 0 || y == 1 && posX != lastPosX || y == 2 && posY != lastPosY || aCount == 3))
                    {
                        for (var j = 0; j < poolIndex[posX, posY]; j++)
                        {
                            if (!pools[posX, posY][j].rec.Intersects(balls[i].rec)) continue;

                            // found collision
                            var normal = (balls[i].rec.Center - pools[posX, posY][j].rec.Center).ToVector2();
                            //var normal = balls[i].rec.Center - balls[j].rec.Center;

                            if (normal == Vector2.Zero)
                            {
                                continue;
                            }

                            normal.Normalize();

                            var speed1 = pools[posX, posY][j].vel.Length();
                            var speed2 = balls[i].vel.Length();

                            balls[i].vel = normal * speed1;
                            pools[posX, posY][j].vel = -normal * speed2;

                            break;
                            //collPool[i].cFunction(collPool[j].gameObject);
                            //collPool[j].cFunction(collPool[i].gameObject);
                        }
                    }
                }
            }
        }

        public void AddToPool(GameObject go)
        {
            goPool.Add(go);

            if (go is IDrawable drawable)
            {
                drawablePool.Add(drawable);
            }

            if (go is IUpdatable updatable)
            {
                updatablePool.Add(updatable);
            }

            if (go is IInteractable)
            {

            }

            // check for components
            collisionSystem.NewEntity(go);

            movementSystem.NewEntity(go);
        }
    }

    public interface IInteractable
    {

    }

    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }

    public interface IUpdatable
    {
        void Update();
    }

    public abstract class Component
    {
        public GameObject gameObject;

        public Component(GameObject go)
        {
            gameObject = go;
        }
    }

    public class ComponentPosition : Component
    {
        public Vector2 pos;

        public ComponentPosition(GameObject go, float X, float Y) : base(go)
        {
            pos = new Vector2(X, Y);
        }
    }

    public class ComponentVelocity : Component
    {
        public Vector2 vec;

        public ComponentVelocity(GameObject go, float X, float Y) : base(go)
        {
            vec = new Vector2(X, Y);
        }
    }

    public class ComponentCollidable : Component
    {
        private readonly int width;
        private readonly int height;
        public Rectangle collisionBox;

        public delegate void CollisionFunction(GameObject go);
        public CollisionFunction cFunction;

        public ComponentCollidable(GameObject go, int width, int height, CollisionFunction cFunction) : base(go)
        {
            this.width = width;
            this.height = height;
            this.cFunction = cFunction;
        }

        public void UpdatePosition(ComponentPosition pos)
        {
            collisionBox = new Rectangle((int)pos.pos.X, (int)pos.pos.Y, width, height);
        }
    }

    public class GameObject
    {
        public List<Component> components = new List<Component>();

        // return the component
        // really bad...
        public Component GetComponent(Type componentType)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == componentType)
                {
                    return components[i];
                }
            }

            return null;
        }
    }

    public class Minion : GameObject, IInteractable, IUpdatable, IDrawable
    {
        private readonly ComponentCollidable cComponent;
        private readonly ComponentPosition positionC;
        private readonly ComponentVelocity velocityC;

        private readonly Color color;

        public Minion() : this(
            ECSGame.rDom.Next(10, 780),
            ECSGame.rDom.Next(10, 780),
            ECSGame.rDom.Next(0, 500) / 100f - 2.5f,
            ECSGame.rDom.Next(0, 500) / 100f - 2.5f)
        {
        }

        public Minion(float posX, float posY, float velX, float velY)
        {
            cComponent = new ComponentCollidable(this, 8, 8, Collision);
            positionC = new ComponentPosition(this, posX, posY);
            velocityC = new ComponentVelocity(this, velX, velY);

            components.Add(cComponent);
            components.Add(positionC);
            components.Add(velocityC);

            color = new Color(ECSGame.rDom.Next(0, 255), ECSGame.rDom.Next(100, 255), ECSGame.rDom.Next(100, 255));
        }

        public void Update()
        {

        }

        // Move!
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ECSGame.sprWhite, new Rectangle((int)positionC.pos.X, (int)positionC.pos.Y, 8, 8), color);
        }

        // Make general circle collision component

        public void Collision(GameObject go)
        {

        }
    }

    public class System
    {
        // always update
        // added entity
        // deleted entity


        //private T[] matches;

        public List<GameObject> entityPool = new List<GameObject>();

        public System(params Component[] matchComponents)
        {
        }

        public virtual void NewEntity(GameObject go) { }

        public virtual void Update() { }
    }

    public class CollisionSystem : System
    {
        //public List<ComponentPosition> posPool = new List<ComponentPosition>();
        //public List<ComponentCollidable> collPool = new List<ComponentCollidable>();
        // public List<ComponentVelocity> velPool = new List<ComponentVelocity>();

        public ComponentPosition[] posPool = new ComponentPosition[0];
        public ComponentCollidable[] collPool = new ComponentCollidable[0];
        public ComponentVelocity[] velPool = new ComponentVelocity[0];

        public override void NewEntity(GameObject go)
        {
            // no gud
            for (int i = 0; i < go.components.Count; i++)
            {
                if (go.components[i] is ComponentPosition position)
                {
                    Array.Resize(ref posPool, posPool.Length + 1);
                    posPool[posPool.Length - 1] = position;
                }
                else if (go.components[i] is ComponentCollidable collidable)
                {
                    Array.Resize(ref collPool, collPool.Length + 1);
                    collPool[collPool.Length - 1] = collidable;
                }
                else if (go.components[i] is ComponentVelocity vec)
                {
                    Array.Resize(ref velPool, velPool.Length + 1);
                    velPool[velPool.Length - 1] = vec;
                }
            }
        }

        public override void Update()
        {
            for (var i = 0; i < posPool.Length; i++)
            {
                // update the component
                collPool[i].collisionBox = new Rectangle((int)posPool[i].pos.X, (int)posPool[i].pos.Y, 8, 8);
            }

            // check for collision
            for (var i = 0; i < posPool.Length; i++)
                for (var j = i + 1; j < posPool.Length; j++)
                {
                    if (collPool[j].collisionBox.Intersects(collPool[i].collisionBox))
                    {
                        // found collision
                        var normal = (collPool[i].collisionBox.Center - collPool[j].collisionBox.Center).ToVector2();

                        if (normal == Vector2.Zero)
                        {
                            return;
                        }

                        normal.Normalize();

                        var speed1 = velPool[j].vec.Length();
                        var speed2 = velPool[i].vec.Length();

                        velPool[i].vec = normal * speed1;
                        velPool[j].vec = -normal * speed2;

                        //collPool[i].cFunction(collPool[j].gameObject);
                        //collPool[j].cFunction(collPool[i].gameObject);
                    }
                }
        }

        public void UpdatePosition(ComponentPosition pos, ComponentCollidable coll)
        {
            coll.UpdatePosition(pos);
        }
    }

    public class MovementSystem : System
    {
        public ComponentPosition[] posPool = new ComponentPosition[0];
        public ComponentVelocity[] velPool = new ComponentVelocity[0];

        public override void NewEntity(GameObject go)
        {
            // needs to check if both components exist in a enty
            for (int i = 0; i < go.components.Count; i++)
            {
                if (go.components[i] is ComponentPosition position)
                {
                    Array.Resize(ref posPool, posPool.Length + 1);
                    posPool[posPool.Length - 1] = position;
                }
                else if (go.components[i] is ComponentVelocity vec)
                {
                    Array.Resize(ref velPool, velPool.Length + 1);
                    velPool[velPool.Length - 1] = vec;
                }
            }
        }

        public override void Update()
        {
            // update the components
            for (var i = 0; i < posPool.Length; i++)
            {
                // ?
                posPool[i].pos += velPool[i].vec;

                if (posPool[i].pos.X <= 0)
                {
                    posPool[i].pos.X = 0;
                    velPool[i].vec.X = -velPool[i].vec.X;
                }
                else if (posPool[i].pos.X >= 792)
                {
                    posPool[i].pos.X = 792;
                    velPool[i].vec.X = -velPool[i].vec.X;
                }

                if (posPool[i].pos.Y <= 0)
                {
                    posPool[i].pos.Y = 0;
                    velPool[i].vec.Y = -velPool[i].vec.Y;
                }
                else if (posPool[i].pos.Y >= 792)
                {
                    posPool[i].pos.Y = 792;
                    velPool[i].vec.Y = -velPool[i].vec.Y;
                }
            }
        }
    }
}
