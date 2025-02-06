using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frame
{
    static class Camera  //only one camera 
    {
        /// <summary>
        /// Initialize camera with position, rotation, zoom and axis 
        /// </summary>
        private static Matrix _transformMatrix; 
        private static Vector2 _position;
        public static float rotation;
        private static float _zoom;
        private static Rectangle _screenRect;

        public static bool updateYAxis = false; 
        public static bool updateXAxis = false; 

        public static void Initialize()
        {
            _zoom = 1.0f;
            rotation = 0.0f;

            //Center camera
            _position = new Vector2(ResX.VirtualWidth / 2, ResX.VirtualHeight / 2);
        }
        public static Rectangle ScreenRect
        {
            get { return _screenRect; }
        }

        public static float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; 
            }
        }

        public static void Update(Vector2 follow)
        {
            UpdateMovement(follow);
            CalculateMatrixAndRectangle();
        }

        private static void UpdateMovement(Vector2 follow)
        {
            //Make the camera center on and follow the position and camera will follow the position passed in
            if (updateXAxis == true)
                _position.X += ((follow.X - _position.X)); 

            if (updateYAxis == true)
                _position.Y += ((follow.Y - _position.Y)); 
        }

        /// <summary>
        /// Sets the camera to look at the position passed in
        /// </summary>
        public static void LookAt(Vector2 lookAt)
        {
            if (updateXAxis == true)
                _position.X = lookAt.X;
            if (updateYAxis == true)
                _position.Y = lookAt.Y;
        }

        private static void CalculateMatrixAndRectangle()
        {           
            _transformMatrix = Matrix.CreateTranslation(new Vector3(-_position, 0)) * Matrix.CreateRotationZ(rotation) *
                        Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) * Matrix.CreateTranslation(new Vector3(ResX.VirtualWidth
                            * 0.5f, ResX.VirtualHeight * 0.5f, 0));

            _transformMatrix = _transformMatrix * ResX.getTransformationMatrix();

            //Round up the X and Y translation 
            _transformMatrix.M41 = (float)Math.Round(_transformMatrix.M41, 0);
            _transformMatrix.M42 = (float)Math.Round(_transformMatrix.M42, 0);

            _screenRect = VisibleArea();
        }

        /// <summary>
        /// Calculates screenRect based on where the camera is
        private static Rectangle VisibleArea()
        {
            Matrix inverseViewMatrix = Matrix.Invert(_transformMatrix);
            Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            Vector2 tr = Vector2.Transform(new Vector2(ResX.VirtualWidth, 0), inverseViewMatrix);
            Vector2 bl = Vector2.Transform(new Vector2(0, ResX.VirtualHeight), inverseViewMatrix);
            Vector2 br = Vector2.Transform(new Vector2(ResX.VirtualWidth, ResX.VirtualHeight), inverseViewMatrix);
            Vector2 min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            Vector2 max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            return new Rectangle((int)min.X, (int)min.Y, (int)(ResX.VirtualWidth / _zoom), (int)(ResX.VirtualHeight / _zoom));
        }

        public static Matrix GetTransformMatrix()
        {
            return _transformMatrix;
        }
    }
}
