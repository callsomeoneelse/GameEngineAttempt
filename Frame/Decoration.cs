using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frame
{
    public class Decoration : GameObject
    {
        public string imgPath;

        public Rectangle sourceRec;

        public Decoration()
        {
            _collidable = false;
        }


        public Decoration(Vector2 inputPos, string inputImgPath, float inputDepth)
        {
            _position = inputPos;
            imgPath = inputImgPath;
            layerDepth = inputDepth;
            _active = true;
            _collidable = false;
        }

        public virtual void Load(ContentManager content, string asset)
        {
            _image = TexFac.Load(asset, content);
            _image.Name = asset;

            _boundingBoxWidth = _image.Width;
            _boundingBoxHeight = _image.Height;

            if (sourceRec == Rectangle.Empty)
            {
                sourceRec = new Rectangle(0, 0, _image.Width, _image.Height);
            }
        }

        public void SetImage(Texture2D inp, string newPath)
        {
            _image = inp;
            imgPath = newPath;

            _boundingBoxWidth = sourceRec.Width = _image.Width;
            _boundingBoxHeight = sourceRec.Height = _image.Height;

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_image != null && _active == true)
            {
                spriteBatch.Draw(_image, _position, sourceRec, _drawColor, rotation, Vector2.Zero, _scale, SpriteEffects.None, layerDepth);
            }

            base.Draw(spriteBatch);
        }


        public string Name
        {
            get { return imgPath; }
        }
    }
}
