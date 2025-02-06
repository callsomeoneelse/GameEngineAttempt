using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace Frame
{
    public class Character : AnimationObject
    {
        public Vector2 velocity;

        protected float decel = 1.2f;  //deceleration
        protected float accel = 0.78f; //acceleration
        protected float maxVelocity = 5f;

        const float GRAVITY = 1f;
        const float JUMPVELO = 16f;
        const float MAXFALLVELO = 24;

        protected bool jumping;


        public static bool applyGrav = true;

        public override void Initialize()
        {
            velocity = Vector2.Zero;
            jumping = false;
            base.Initialize();
        }

        public override void Update(List<GameObject> objs, Map map)
        {
            UpdateMove(objs, map);

            base.Update(objs, map);
        }

        private void UpdateMove(List<GameObject> objs, Map map)
        {
            if (velocity.X != 0 && CheckCollide(map, objs, true) == true)
            {
                velocity.X = 0;
            }

            _position.X += velocity.X;

            if (velocity.Y != 0 && CheckCollide(map, objs, false) == true)
            {
                velocity.Y = 0;
            }

            _position.Y += velocity.Y;

            if (applyGrav == true)
            {
                ApplyGrav(map);
            }

            velocity.X = TendToZero(velocity.X, decel);

            if (applyGrav == false)
            {
                velocity.Y = TendToZero(velocity.Y, decel);
            }
        }

        private void ApplyGrav(Map map)
        {
            if (jumping == true || OnGround(map) == Rectangle.Empty)
            {
                velocity.Y += GRAVITY;
            }

            if (velocity.Y > MAXFALLVELO)
            {
                velocity.Y = MAXFALLVELO;
            }
        }

        protected void MoveRight()
        {
            velocity.X += accel + decel;

            if (velocity.X > maxVelocity)
            {
                velocity.X = maxVelocity;
            }

            dir.X = 1;
        }

        protected void MoveLeft()
        {
            velocity.X -= accel + decel;

            if (velocity.X < -maxVelocity)
            {
                velocity.X = -maxVelocity;
            }

            dir.X = -1;
        }

        protected void MoveDown()
        {
            velocity.Y += accel + decel;

            if (velocity.Y > maxVelocity)
            {
                velocity.Y = maxVelocity;
            }

            dir.X = 1;
        }

        protected void MoveUp()
        {
            velocity.Y -= accel + decel;

            if (velocity.Y < -maxVelocity)
            {
                velocity.Y = -maxVelocity;
            }

            dir.Y = -1;
        }

        protected bool Jump(Map map)
        {
            if (jumping == true)
            {
                return false;
            }

            if (velocity.Y == 0 && OnGround(map) != Rectangle.Empty)
            {
                velocity.Y -= JUMPVELO;
                jumping = true;
                return true;
            }

            return false;
        }

        protected virtual bool CheckCollide(Map map, List<GameObject> objects, bool xAxis)
        {
            Rectangle futureBoundingBox = BoundingBox;

            int maxX = (int)maxVelocity;
            int maxY = (int)maxVelocity;

            if (applyGrav == true)
            {
                maxY = (int)JUMPVELO;
            }

            if (xAxis == true && velocity.X != 0)
            {
                if (velocity.X > 0)
                {
                    futureBoundingBox.X += maxX;
                }
                else
                {
                    futureBoundingBox.X -= maxX;
                }
            }
            else if(applyGrav == false && xAxis == false && velocity.Y != 0)
            {
                if (velocity.Y > 0)
                {
                    futureBoundingBox.Y += maxY;
                }
                else
                {
                    futureBoundingBox.Y -= maxY;
                }
            }
            else if (applyGrav == true && xAxis == false && velocity.Y != GRAVITY)
            {
                if (velocity.Y > 0)
                {
                    futureBoundingBox.Y += maxY;
                }
                else
                {
                    futureBoundingBox.Y -= maxY;
                }
            }

            Rectangle wallCollision = map.CheckColl(futureBoundingBox);

            if (wallCollision != Rectangle.Empty)
            {
                if (applyGrav == true && velocity.Y >= GRAVITY && (futureBoundingBox.Bottom > wallCollision.Top - maxVelocity) && (futureBoundingBox.Bottom <= wallCollision.Top + velocity.Y))
                {
                    LandResponse(wallCollision);
                    return true;  //landing
                }
                else
                {
                    return true;
                }
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != this && objects[i]._active == true && objects[i]._collidable == true && objects[i].CheckCollision(futureBoundingBox) == true)
                {
                    return true;
                }
            }
            
            return false;
            
        }

        public void LandResponse(Rectangle wallCollision)
        {
            _position.Y = wallCollision.Top - (_boundingBoxHeight + _boundingBoxOffset.Y);
            velocity.Y = 0;
            jumping = false;
        }

        protected float TendToZero(float val, float amount)   //moves values towards 0
        {
            if (val > 0f && (val -= amount) < 0f) return 0f;
            if (val < 0f && (val += amount) > 0f) return 0f;
            return val;
        }

        protected Rectangle OnGround(Map map)
        {
            Rectangle futureBoundingBox = new Rectangle((int)(_position.X + _boundingBoxOffset.X), (int)(_position.Y + _boundingBoxOffset.Y + (velocity.Y + GRAVITY)), _boundingBoxWidth, _boundingBoxHeight);

            return map.CheckColl(futureBoundingBox);
        }

    }
}
