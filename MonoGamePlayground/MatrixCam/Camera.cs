using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.MatrixCam
{
    class Camera
    {
        public float Zoom = 4;
        public Vector2 Location, currentCamDist, aimPosition;
        public float Rotation { get; set; }

        private Rectangle Bounds { get; set; }

        public Matrix TransformMatrix =>
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(Zoom) *
                                         Matrix.CreateTranslation(new Vector3(-roundX, -roundY, 0)) *
                                         //Matrix.CreateTranslation(new Vector3(-Location.X, -Location.Y, 0)) *
                                         Matrix.CreateTranslation(new Vector3((int)(Bounds.Width * 0.5f), (int)(Bounds.Height * 0.5f), 0));

        //public float roundX => Location.X;
        //public float roundY => Location.Y;

        //public int roundX => (int)Math.Round(Location.X, 0, MidpointRounding.AwayFromZero);
        //public int roundY => (int)Math.Round(Location.Y, 0, MidpointRounding.AwayFromZero);

        public int roundX => (int)Math.Round(aimPosition.X) - (int)Math.Round(currentCamDist.X);
        public int roundY => (int)Math.Round(aimPosition.Y) - (int)Math.Round(currentCamDist.Y);

        public float cameraDeadzone = 15, distX, lastDistX, velocityX;
        private int currentDistX;
        private int deadzone = 25;

        public Camera(Viewport viewport)
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
            // position = new Vector2(ToWorldCoords(position.X), ToWorldCoords(position.Y));

            lastDistX = distX;

            aimPosition = position;
            var dist = position - Location;
            var speedX = MathHelper.Clamp(15f, 0, Math.Max(Math.Abs(maxSpeed.X), Math.Abs(distX)));
            var aimDistX = maxSpeed.X * 10;

            if (distX < aimDistX)
            {
                distX += speedX;
                if (distX > aimDistX)
                    distX = aimDistX;
            }
            else if (distX > aimDistX)
            {
                distX -= speedX;
                if (distX < aimDistX)
                    distX = aimDistX;
            }

            currentCamDist.X = distX;

            MatrixCamGame.strDebug = "speed: " + speedX + "\naimDistX: " + aimDistX + "\ndistX: " + distX + "\nDiff" + (Location.X - position.X);
            MatrixCamGame.strDebug += "\nRealDiff: " + (roundX - ToWorldCoords(position.X));


            Location.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 2;
            Location.Y -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 2;

            // Location = position;
        }

        //public void UpdateMovement()
        //{

        //    if (Location.X < position.X)
        //    {
        //        Location.X += speedX;
        //        if (Location.X > position.X)
        //            Location.X = position.X;
        //    }
        //    else if (Location.X > position.X)
        //    {
        //        Location.X -= speedX;
        //        if (Location.X < position.X)
        //            Location.X = position.X;
        //    }

        //}

        // 0,1  1,0
        double functF(float x)
        {
            return 1 / (x + (1 / 2f * (Math.Sqrt(5) - 1))) + (-1 / (1 / 2f * (Math.Sqrt(5) - 1)) + 1);
        }
    }
}