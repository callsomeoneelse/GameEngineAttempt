using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame
{
    static class Input
    {
        private static KeyboardState _keyboardState = Keyboard.GetState();
        private static KeyboardState _lastKeyboardState;
        private static MouseState _mouseState;
        private static MouseState _lastMouseState;

        public static void Update()
        {
            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        /// <summary>
        /// Checks if key is currently being pressed
        /// </summary>
        public static bool IsKeyDown(Keys input)
        {
            return _keyboardState.IsKeyDown(input);
        }

        /// <summary>
        /// Checks if key is released
        /// </summary>
        public static bool IsKeyUp(Keys input)
        {
            return _keyboardState.IsKeyUp(input);
        }

        /// <summary>
        /// Checks if key was just pressed
        /// </summary>
        public static bool KeyPressed(Keys input)
        {
            if (_keyboardState.IsKeyDown(input) == true && _lastKeyboardState.IsKeyDown(input) == false)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether left mouse button is being pressed
        /// </summary>
        public static bool MouseLeftDown()
        {
            if (_mouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether right mouse button is being pressed
        /// </summary>
        public static bool MouseRightDown()
        {
            if (_mouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the left mouse button was clicked
        /// </summary>
        public static bool MouseLeftClicked()
        {
            if (_mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the right mouse button was clicked
        /// </summary>
        public static bool MouseRightClicked()
        {
            if (_mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets mouse coordinates 
        /// </summary>
        public static Vector2 MousePositionCamera()
        {
            Vector2 mousePosition = Vector2.Zero;
            mousePosition.X = _mouseState.X;
            mousePosition.Y = _mouseState.Y;

            return ScreenToWorld(mousePosition);
        }

        /// <summary>
        /// Gets the last mouse coordinates 
        /// </summary>
        public static Vector2 LastMousePositionCamera()
        {
            Vector2 mousePosition = Vector2.Zero;
            mousePosition.X = _lastMouseState.X;
            mousePosition.Y = _lastMouseState.Y;

            return ScreenToWorld(mousePosition);
        }

        /// <summary>
        /// Takes screen coordinates
        /// </summary>
        private static Vector2 ScreenToWorld(Vector2 input)
        {
            input.X -= ResX.VirtualViewportX;
            input.Y -= ResX.VirtualViewportY;

            return Vector2.Transform(input, Matrix.Invert(Camera.GetTransformMatrix()));
        }

    }
}
