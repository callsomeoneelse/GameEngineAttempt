using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Frame
{
    public class AnimationObject : GameObject
    {
        protected int currentAnimationFrame;
        protected float animationTimer;
        protected int currentAnimationX = -1, currentAnimationY = -1;
        protected AnimationSet animationSet = new AnimationSet();
        protected Animation currentAnimation;
        protected bool flipRightFrame = true;
        protected bool flipLeftFrame = false;
        protected SpriteEffects spriteEffect = SpriteEffects.None;

        protected enum Animations
        {
            WalkLeft,
            WalkRight,
            Idle,
        }

        public override void Update(List<GameObject> objs, Map map)
        {
            base.Update(objs, map);

            if (currentAnimation != null)
            {
                UpdateAnimation();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_active == false)
            {
                return;
            }

            if (currentAnimationX == -1 || currentAnimationY == -1)
            {
                base.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(_image, _position, new Rectangle(currentAnimationX, currentAnimationY, animationSet.width, animationSet.height ), _drawColor, rotation, Vector2.Zero, _scale, spriteEffect, layerDepth);
            }
        }

        public bool AnimationComplete()
        {
            return currentAnimationFrame >= currentAnimation.animationOrder.Count - 1;
        }

        protected void CalculateFramePos()
        {
            int coordinate = currentAnimation.animationOrder[currentAnimationFrame];

            currentAnimationX = (coordinate % animationSet.gridX) * animationSet.width ;
            currentAnimationY = (coordinate / animationSet.gridX) * animationSet.height ;
        }

        public virtual void UpdateAnimation()
        {
            if (currentAnimation.animationOrder.Count < 1)
            {
                return;
            }

            animationTimer -= 1;

            if(animationTimer <= 0)
            {
                animationTimer = currentAnimation.speed;

                if (AnimationComplete() == true)
                {
                    currentAnimationFrame = 0;
                }
                else
                {
                    currentAnimationFrame++;
                }

                CalculateFramePos();
            }
        }

        protected Animation GetAnimation(Animations animation)
        {
            string name = GetAnimationName(animation);

            for (int i = 0; i < animationSet.animationList.Count; i++)
            {
                if (animationSet.animationList[i].name == name)
                {
                    return animationSet.animationList[i];
                }
            }
            return null;
        }

        protected virtual void ChangeAnimation(Animations newAnimation)
        {
            currentAnimation = GetAnimation(newAnimation);

                if (currentAnimation == null)
                {
                     return;
                }

            currentAnimationFrame = 0;   //start on first frame
            animationTimer = currentAnimation.speed; 

            CalculateFramePos();

            if (flipRightFrame == true && currentAnimation.name.Contains("Right") || flipLeftFrame == true && currentAnimation.name.Contains("Left"))
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                spriteEffect = SpriteEffects.None;
            }
        }

        protected string GetAnimationName(Animations animation)
        {
            return animation.ToString();
        }

        protected bool AnimationIsNot(Animations input)
        {
            return currentAnimation != null && GetAnimationName(input) != currentAnimation.name;
        }

        public void LoadAnimation(string path, ContentManager content)
        {
            AnimationData animationData = AnimationLoader.Load(path);
            animationSet = animationData.animation;

            //initializing
            _center.X = animationSet.width / 2;
            _center.Y = animationSet.height / 2;

            if (animationSet.animationList.Count > 0)
            {
                currentAnimation = animationSet.animationList[0];  //current animation is first item

                currentAnimationFrame = 0;
                animationTimer = 0f;
                CalculateFramePos();
            }
        }
    }
}
