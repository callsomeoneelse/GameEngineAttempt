using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frame
{
    static class ResX
    {
        static private GraphicsDeviceManager _Device = null;

        static private int _width = 800;
        static private int _height = 600;
        static private int _vWidth = 1024;
        static private int _vHeight = 768;
        static private Matrix _scaleMatrix;
        static private bool _FullScreen = false;
        static private bool _dirtyMatrix = true;
        static private int _virtualViewportX;
        static private int _virtualViewportY;

        public static int VirtualViewportX
        {
            get { return _virtualViewportX; }
        }

        public static int VirtualViewportY
        {
            get { return _virtualViewportY; }
        }

        public static int VirtualWidth
        {
            get { return _vWidth; }
        }

        public static int VirtualHeight
        {
            get { return _vHeight; }
        }

        static public void Init(ref GraphicsDeviceManager device)
        {
            _width = device.PreferredBackBufferWidth;
            _height = device.PreferredBackBufferHeight;
            _Device = device;
            _dirtyMatrix = true;
            ApplyResolutionSettings();
        }

        static public Matrix getTransformationMatrix()
        {
            if (_dirtyMatrix) RecreateScaleMatrix();

            return _scaleMatrix;
        }

        static public void SetResolution(int Width, int Height, bool FullScreen)
        {
            _width = Width;
            _height = Height;

            _FullScreen = FullScreen;

            ApplyResolutionSettings();
        }

        static public void SetVirtualResolution(int Width, int Height)
        {
            _vWidth = Width;
            _vHeight = Height;

            _dirtyMatrix = true;
        }

        static private void ApplyResolutionSettings()
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size
            if (_FullScreen == false)
            {
                if ((_width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (_height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    _Device.PreferredBackBufferWidth = _width;
                    _Device.PreferredBackBufferHeight = _height;
                    _Device.IsFullScreen = _FullScreen;
                    _Device.PreferMultiSampling = true;
                    _Device.ApplyChanges();
                }
            }
            else
            {
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == _width) && (dm.Height == _height))
                    {
                        _Device.PreferredBackBufferWidth = _width;
                        _Device.PreferredBackBufferHeight = _height;
                        _Device.IsFullScreen = _FullScreen;
                        _Device.PreferMultiSampling = true;
                        _Device.ApplyChanges();
                    }
                }
            }

            _dirtyMatrix = true;

            _width = _Device.PreferredBackBufferWidth;
            _height = _Device.PreferredBackBufferHeight;
        }

        /// <summary>
        /// /// Sets correct aspect ratio
        /// </summary>
        static public void BeginDraw()
        {
            FullViewport();
            _Device.GraphicsDevice.Clear(Color.Black);
            ResetViewport();
            _Device.GraphicsDevice.Clear(Color.Black);
        }

        static private void RecreateScaleMatrix()
        {
            _dirtyMatrix = false;
            _scaleMatrix = Matrix.CreateScale(
                           (float)_Device.GraphicsDevice.Viewport.Width / _vWidth,
                           (float)_Device.GraphicsDevice.Viewport.Width / _vWidth,
                           1f);
        }


        static public void FullViewport()
        {
            Viewport vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = _width;
            vp.Height = _height;
            _Device.GraphicsDevice.Viewport = vp;
        }

        /// <summary>
        /// Get VMA Ratio
        /// </summary>
        /// <returns>aspect ratio</returns>
        static public float getVirtualAspectRatio()
        {
            return (float)_vWidth / (float)_vHeight;
        }

        static public void ResetViewport()
        {
            float targetAspectRatio = getVirtualAspectRatio();
            int width = _Device.PreferredBackBufferWidth;
            int height = (int)(width / targetAspectRatio + .5f);
            bool changed = false;

            if (height > _Device.PreferredBackBufferHeight)
            {
                height = _Device.PreferredBackBufferHeight;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
                changed = true;
            }

            // set up the new viewport centered in backbuffer
            Viewport viewport = new Viewport();

            viewport.X = (_Device.PreferredBackBufferWidth / 2) - (width / 2);
            viewport.Y = (_Device.PreferredBackBufferHeight / 2) - (height / 2);
            _virtualViewportX = viewport.X;
            _virtualViewportY = viewport.Y;
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (changed)
            {
                _dirtyMatrix = true;
            }

            _Device.GraphicsDevice.Viewport = viewport;
        }
    }
}
