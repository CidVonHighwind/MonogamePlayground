using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.MatrixCam
{
    class CameraNew
    {
        public float Zoom = 4;
        public Vector2 Location, currentCamDist, aimPosition;
        public float Rotation { get; set; }

        private Rectangle Bounds { get; set; }

        public Matrix TransformMatrix =>
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(Zoom) *
                                         Matrix.CreateTranslation(new Vector3(-roundX, -roundY, 0)) *
                                         Matrix.CreateTranslation(new Vector3((int)(Bounds.Width * 0.5f), (int)(Bounds.Height * 0.5f), 0));
        
        public int roundX => (int)Math.Round(Location.X, 0, MidpointRounding.AwayFromZero);
        public int roundY => (int)Math.Round(Location.Y, 0, MidpointRounding.AwayFromZero);
        
        public float cameraDeadzone = 15, distX, lastDistX, velocityX;
        private int currentDistX;
        private int deadzone = 25;
        private Vector2 loc;

        public CameraNew(Viewport viewport)
        {
            Bounds = viewport.Bounds;

            var rotMatrix = Matrix.CreateRotationZ(Rotation);
            var zoomMatrix = Matrix.CreateScale(Zoom);
            var transMatrix = Matrix.CreateTranslation(new Vector3(-ToWorldCoords(Location.X), -ToWorldCoords(Location.Y), 0));
        }

        public float ToWorldCoords(float input)
        {
            return (int)(input / (1f / Zoom)) * (1f / Zoom);
        }

        public void SetBounds(Viewport viewport)
        {
            Bounds = viewport.Bounds;
        }

        public void Center(Vector2 position, Vector2 maxSpeed)
        {
            var dist = position - loc;
            var speed = new Vector2((Math.Abs(dist.X) - deadzone) / 15, (Math.Abs(dist.Y) - deadzone) / 15) * 2;

            if (loc.X < position.X - deadzone)
            {
                loc.X += speed.X;
                if (loc.X > position.X)
                    loc.X = position.X;
            }
            else if (loc.X > position.X + deadzone)
            {
                loc.X -= speed.X;
                if (loc.X < position.X)
                    loc.X = position.X;
            }

            if (loc.Y < position.Y - deadzone)
            {
                loc.Y += speed.Y;
                if (loc.Y > position.Y)
                    loc.Y = position.Y;
            }
            else if (loc.Y > position.Y + deadzone)
            {
                loc.Y -= speed.Y;
                if (loc.Y < position.Y)
                    loc.Y = position.Y;
            }

            Location.X = position.X + (int)(loc.X - position.X);
            Location.Y = position.Y + (int)(loc.Y - position.Y);

            //Game1.strDebug = "\ndistX: " + distX + "\nDiff" + (locX - position.X);
            //Game1.strDebug += "\nRealDiff: " + (roundX - ToWorldCoords(position.X));
            //Game1.strDebug += "\nWorldCoords(locX): " + ToWorldCoords(locX);
            //Game1.strDebug += "\nToWorldCoords(position.X): " + ToWorldCoords(position.X);
            //Game1.strDebug += "\nWorldCoords(locX - position.X): " + ToWorldCoords(locX - position.X);

            //Location = position;

            Location.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 2;
            Location.Y -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 2;
        }
        
        // 0,1  1,0
        double functF(float x)
        {
            return 1 / (x + (1 / 2f * (Math.Sqrt(5) - 1))) + (-1 / (1 / 2f * (Math.Sqrt(5) - 1)) + 1);
        }
    }
}