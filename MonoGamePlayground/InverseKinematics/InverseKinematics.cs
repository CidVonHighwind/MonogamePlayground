using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.InverseKinematics
{
    class IKElement
    {

    }

    class Joint : IKElement
    {
        public Vector2 Position;
        public float Angle;
        public float MaxAngle = 160;
    }

    class Bone : IKElement
    {
        public int Length;

        public Bone(int length)
        {
            Length = length;
        }
    }

    class InverseKinematicsGame : IGame
    {
        private MouseState currentMouseState, lastMouseState;

        private Texture2D sprWhite;
        private SpriteFont font;

        List<IKElement> IKChain = new List<IKElement>();

        Vector2 basePosition = new Vector2(640, 360);
        private Vector2 startPosition;
        private Vector2 goalPosition;

        private int selectedBone = 0;
        private int timer;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            font = content.Load<SpriteFont>("InverseKinematics/debugFont");

            var boneCount = 10;
            var currentPosition = basePosition;
            for (var i = 0; i < boneCount; i++)
            {
                IKChain.Add(new Joint() { Position = currentPosition });
                IKChain.Add(new Bone(30));
            }
            IKChain.Add(new Joint() { Position = currentPosition });
        }

        private float Angle;
        public void Update(GameTime gameTime)
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (lastMouseState.ScrollWheelValue > currentMouseState.ScrollWheelValue)
                Angle-= 10;
            else if (lastMouseState.ScrollWheelValue < currentMouseState.ScrollWheelValue)
                Angle+= 10;

            if (Angle > 360)
                Angle -= 360;
            if (Angle < 0)
                Angle += 360;

            // selectedBone = MathHelper.Clamp(selectedBone, 0, bones.Length - 1);


            var goalPoint = Mouse.GetState().Position;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                goalPosition = new Vector2(goalPoint.X, goalPoint.Y);

            SolveIK();

            // BackwardPass(selectedBone);
            // ForwardPass(selectedBone);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // draw the bones
            for (var i = 0; i < IKChain.Count; i++)
            {
                if (IKChain[i] is Bone)
                {
                    var boneRectangle = new Rectangle(
                        (int)((Joint)IKChain[i - 1]).Position.X,
                        (int)((Joint)IKChain[i - 1]).Position.Y, ((Bone)IKChain[i]).Length, 4);

                    var newPosition = ((Joint)IKChain[i + 1]).Position - ((Joint)IKChain[i - 1]).Position;
                    var newRadius = (float)Math.Atan2(newPosition.Y, newPosition.X);
                    var rotation = MathHelper.ToDegrees(newRadius);

                    // draw the bone
                    spriteBatch.Draw(sprWhite, boneRectangle, new Rectangle(0, 0, 1, 1), Color.LightGray,
                        MathHelper.ToRadians(rotation), new Vector2(0, 0.5f), SpriteEffects.None, 0);

                    // spriteBatch.DrawString(font, "" + bones[i].Rotation, currentPosition, Color.Green);
                }
            }

            for (var i = 0; i < IKChain.Count; i++)
                if (IKChain[i] is Joint)
                {
                    // draw the joint
                    spriteBatch.Draw(sprWhite, new Rectangle(
                        (int)((Joint)IKChain[i]).Position.X - 3,
                        (int)((Joint)IKChain[i]).Position.Y - 3, 6, 6), (i - 1 == selectedBone) ? Color.Blue : Color.Red);

                    if (false)
                    {
                        var backgroundSize = font.MeasureString(((Joint)IKChain[i]).Angle.ToString());
                        spriteBatch.Draw(sprWhite,
                            new Rectangle(
                                (int)((Joint)IKChain[i]).Position.X + 5,
                                (int)((Joint)IKChain[i]).Position.Y - 4,
                                (int)backgroundSize.X, (int)backgroundSize.Y), Color.White * 0.75f);

                        spriteBatch.DrawString(font, ((Joint)IKChain[i]).Angle.ToString(), ((Joint)IKChain[i]).Position + new Vector2(5, -4),
                            Color.Black);
                    }
                }

            spriteBatch.End();
        }

        void SolveIK()
        {
            //// get the current positions of the joints
            //var currentPosition = basePosition;
            //for (var i = 0; i < bones.Length; i++)
            //{
            //    bones[i].startPosition = currentPosition;

            //    currentPosition += new Vector2(
            //                           (float)Math.Cos(MathHelper.ToRadians(bones[i].Rotation)),
            //                           (float)Math.Sin(MathHelper.ToRadians(bones[i].Rotation))) * bones[i].Length;

            //    bones[i].endPosition = currentPosition;
            //}

            timer++;

            //if (timer == 29)
            //    BackwardPassNew();
            //if (timer == 59)
            //{
            //    ForwardPassNew();
            //    timer = 0;
            //}

            ((Joint)IKChain[IKChain.Count - 1]).Position = goalPosition;
            ((Joint) IKChain[IKChain.Count - 1 - 2]).Angle = Angle;
            ((Joint)IKChain[IKChain.Count - 1 - 2]).Position =
                ((Joint)IKChain[IKChain.Count - 1]).Position - new Vector2(
                (float)Math.Cos(MathHelper.ToRadians(((Joint)IKChain[IKChain.Count - 1 - 2]).Angle)),
                (float)Math.Sin(MathHelper.ToRadians(((Joint)IKChain[IKChain.Count - 1 - 2]).Angle))) * ((Bone)IKChain[IKChain.Count - 1 - 1]).Length;

            // ((Joint)IKChain[IKChain.Count - 1]).Position = goalPosition;

            for (int i = 0; i < 1; i++)
            {
                BackwardPassNew(IKChain.Count - 1 - 2 * 2);
                ForwardPassNew(IKChain.Count - 1 - 2);
            }
        }

        void BackwardPassNew(int startIndex)
        {
            startPosition = ((Joint)IKChain[0]).Position;

            for (var i = startIndex; i >= 0; i -= 2)
            {
                var newPosition = ((Joint)IKChain[i + 2]).Position - ((Joint)IKChain[i]).Position;
                var newRadius = (float)Math.Atan2(newPosition.Y, newPosition.X);
                var rotation = MathHelper.ToDegrees(newRadius);

                // check for rotation constraints
                if (i < IKChain.Count - 3)
                {
                    var prePosition = ((Joint)IKChain[i + 4]).Position - ((Joint)IKChain[i + 2]).Position;
                    var preRadius = (float)Math.Atan2(prePosition.Y, prePosition.X);
                    var preRotation = MathHelper.ToDegrees(preRadius);

                    var rotationDiff = preRotation - rotation;
                    if (rotationDiff < -180)
                        rotationDiff += 360;
                    else if (rotationDiff > 180)
                        rotationDiff -= 360;

                    ((Joint)IKChain[i]).Angle = rotationDiff;

                    // check if the joint constraint is meet
                    if (-((Joint)IKChain[i + 2]).MaxAngle > rotationDiff || ((Joint)IKChain[i + 2]).MaxAngle < rotationDiff)
                        rotation = preRotation - Math.Sign(rotationDiff) * ((Joint)IKChain[i + 2]).MaxAngle;
                }

                ((Joint)IKChain[i]).Position = ((Joint)IKChain[i + 2]).Position - new Vector2(
                                   (float)Math.Cos(MathHelper.ToRadians(rotation)),
                                   (float)Math.Sin(MathHelper.ToRadians(rotation))) * ((Bone)IKChain[i + 1]).Length;
            }
        }

        void ForwardPassNew(int endIndex)
        {
            ((Joint)IKChain[0]).Position = startPosition;

            for (var i = 0; i <= IKChain.Count - 1 - 2; i += 2)
            {
                var newPosition = ((Joint)IKChain[i + 2]).Position - ((Joint)IKChain[i]).Position;
                var newRadius = (float)Math.Atan2(newPosition.Y, newPosition.X);
                var rotation = MathHelper.ToDegrees(newRadius);

                if (i >= endIndex)
                    rotation = ((Joint) IKChain[i]).Angle;

                // check for rotation constraints
                if (i > 0)
                {
                    var prePosition = ((Joint)IKChain[i]).Position - ((Joint)IKChain[i - 2]).Position;
                    var preRadius = (float)Math.Atan2(prePosition.Y, prePosition.X);
                    var preRotation = MathHelper.ToDegrees(preRadius);

                    var rotationDiff = preRotation - rotation;
                    if (rotationDiff < -180)
                        rotationDiff += 360;
                    else if (rotationDiff > 180)
                        rotationDiff -= 360;

                    ((Joint)IKChain[i]).Angle = rotationDiff;

                    // check if the joint constraint is meet
                    if (-((Joint)IKChain[i]).MaxAngle > rotationDiff || ((Joint)IKChain[i]).MaxAngle < rotationDiff)
                        rotation = preRotation - Math.Sign(rotationDiff) * ((Joint)IKChain[i]).MaxAngle;
                }

                ((Joint)IKChain[i + 2]).Position = ((Joint)IKChain[i]).Position + new Vector2(
                                                   (float)Math.Cos(MathHelper.ToRadians(rotation)),
                                                   (float)Math.Sin(MathHelper.ToRadians(rotation))) * ((Bone)IKChain[i + 1]).Length;
            }
        }

        //var angleRadiant = Scalar(newPosition, prePosition) / (Math.Sqrt(Scalar(newPosition, newPosition)) * Math.Sqrt(Scalar(prePosition, prePosition)));
        //var angleDegree = MathHelper.ToDegrees((float)Math.Acos(angleRadiant));

        //float Scalar(Vector2 a, Vector2 b)
        //{
        //    return (a.X * b.X + a.Y * b.Y);
        //}

        int FastestDirection(float from, float to)
        {

            return 0;
        }

        float Normalize(float rotation)
        {
            while (rotation < 0)
                rotation += 360;
            while (rotation > 360)
                rotation -= 360;

            return rotation;
        }

        /*
        void BackwardPass(int startBone)
        {
            var currentPosition = basePosition;
            for (var i = 0; i < bones.Length; i++)
            {
                bones[i].startPosition = currentPosition;

                currentPosition += new Vector2(
                                        (float)Math.Cos(MathHelper.ToRadians(bones[i].Rotation)),
                                        (float)Math.Sin(MathHelper.ToRadians(bones[i].Rotation))) * bones[i].Length;

                bones[i].endPosition = currentPosition;
            }

            var goalPoint = Mouse.GetState().Position;
            var goalPosition = new Vector2(goalPoint.X, goalPoint.Y);
            for (var i = startBone; i >= 0; i--)
            {
                bones[i].endPosition = goalPosition;

                var newPosition = goalPosition - bones[i].startPosition;
                var newRadius = (float)Math.Atan2(newPosition.Y, newPosition.X);
                var rotation = MathHelper.ToDegrees(newRadius);

                // check for rotation constraints
                if (i < startBone)
                {
                    var abs = Math.Abs(rotation - bones[i + 1].Rotation);
                    while (abs > 360)
                        abs -= 360;
                    if (abs > 90 && abs < 270)
                        rotation = bones[i + 1].Rotation + Math.Sign(rotation - bones[i + 1].Rotation) * 90;
                }

                while (rotation < 0)
                    rotation += 360;

                bones[i].Rotation = rotation;
                goalPosition = bones[i].endPosition - new Vector2(
                                   (float)Math.Cos(MathHelper.ToRadians(rotation)),
                                   (float)Math.Sin(MathHelper.ToRadians(rotation))) * bones[i].Length;
            }

            
            //goalPoint = Mouse.GetState().Position;
            //goalPosition = new Vector2(goalPoint.X, goalPoint.Y);
            //for (var i = startBone + 1; i < bones.Length; i++)
            //{
            //    bones[i].startPosition = goalPosition;

            //    var newPosition = bones[i].endPosition - goalPosition;
            //    var newRadius = Math.Atan2(newPosition.Y, newPosition.X);
            //    bones[i].Rotation = MathHelper.ToDegrees((float)newRadius);

            //    goalPosition = bones[i].startPosition + new Vector2(
            //                       (float)Math.Cos(MathHelper.ToRadians(bones[i].Rotation)),
            //                       (float)Math.Sin(MathHelper.ToRadians(bones[i].Rotation))) * bones[i].Length;
            //}
        }

        void ForwardPass(int startBone)
        {
            var goalPosition = basePosition;

            for (var i = 0; i < startBone; i++)
            {
                bones[i].startPosition = goalPosition;

                var newPosition = bones[i].endPosition - goalPosition;
                var newRadius = (float)Math.Atan2(newPosition.Y, newPosition.X);
                var rotation = MathHelper.ToDegrees(newRadius);

                // check for rotation constraints
                if (i > 0)
                {
                    var abs = Math.Abs(rotation - bones[i - 1].Rotation);
                    while (abs > 360)
                        abs -= 360;
                    if (abs > 90 && abs < 270)
                        rotation = bones[i - 1].Rotation - Math.Sign(rotation - bones[i - 1].Rotation) * 90;
                }

                while (rotation < 0)
                    rotation += 360;

                bones[i].Rotation = rotation;
                goalPosition = bones[i].startPosition + new Vector2(
                                   (float)Math.Cos(MathHelper.ToRadians(rotation)),
                                   (float)Math.Sin(MathHelper.ToRadians(rotation))) * bones[i].Length;
            }
        }
        */
    }
}
