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
    public class GameObject
    {
        protected Texture2D _image;
        public Vector2 _position;
        protected Color _drawColor = Color.White;
        protected float _scale = 1.0f, rotation = 0f;
        public float layerDepth = 1.0f;
        public bool _active = true;
        protected Vector2 _center;
        protected Vector2 dir = new Vector2(1, 0);

        public bool _collidable = true;
        protected int _boundingBoxWidth, _boundingBoxHeight;
        protected Vector2 _boundingBoxOffset;
        Texture2D _boundingBoxImage;

        public Vector2 startPosition = new Vector2(-1, -1);
        

        const bool DRAWBOUNDBOX = true;


        public GameObject()
        {

        }

        public virtual void Initialize()
        {
            if (startPosition == new Vector2(-1, -1))
            {
                startPosition = _position;
            }
        }

        public virtual void SetDefaultPosition()
        {
            _position = startPosition;
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)(_position.X + _boundingBoxOffset.X), (int)(_position.Y + _boundingBoxOffset.Y), _boundingBoxWidth, _boundingBoxHeight);
            }
        }

        public virtual void Load(ContentManager content)
        {
            _boundingBoxImage = TexFac.Load("pixel", content);

            CalcCenter();

            if (_image != null)
            {
                _boundingBoxWidth = _image.Width;
                _boundingBoxHeight = _image.Height;
            }
        }

        public virtual void Update(List<GameObject> objs, Map map)
        {

        }

        public virtual bool CheckCollision(Rectangle input)
        {
            return BoundingBox.Intersects(input);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_boundingBoxImage != null && DRAWBOUNDBOX == true && _active == true)
            {
                spriteBatch.Draw(_boundingBoxImage, new Vector2(BoundingBox.X, BoundingBox.Y), BoundingBox, new Color(120,120,120,120), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            }

                if (_image != null && _active == true)
            {
                spriteBatch.Draw(_image, _position, null, _drawColor, rotation, Vector2.Zero, _scale, SpriteEffects.None, layerDepth);
            }
        }

        public virtual void BulletInteract()
        {

        }

        private void CalcCenter()
        {
            if (_image == null)
            {
                return;
            }

            _center.X = _image.Width / 2;
            _center.Y = _image.Height / 2;
        }
    }
}
