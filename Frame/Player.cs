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
    public class Player : Attacker
    {
        public static int score;

        public Player()
        {

        }

        public Player(Vector2 inputPos)
        {
            _position = inputPos;
        }

        public override void Initialize()
        {
            score = 0;
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            _image = TexFac.Load("Sheet", content);

            LoadAnimation("Omori.anm", content);
            ChangeAnimation(Animations.Idle);

            base.Load(content);

            _boundingBoxOffset.X = 0;
            _boundingBoxOffset.Y = 0;
            _boundingBoxWidth = animationSet.width;
            _boundingBoxHeight = animationSet.height;
        }

        public override void Update(List<GameObject> objs, Map map)
        {
            CheckInput(objs, map);
            base.Update(objs, map);
        }

        private void CheckInput(List<GameObject> objs, Map map)
        {

            if (Character.applyGrav == false)
            {
                if (Input.IsKeyDown(Keys.W) == true)
                {
                    MoveUp();
                }
                else if (Input.IsKeyDown(Keys.S) == true)
                {
                    MoveDown();
                }
                if (Input.IsKeyDown(Keys.A) == true)
                {
                    MoveLeft();
                }
                else if (Input.IsKeyDown(Keys.D) == true)
                {
                    MoveRight();
                }
            }
            else
            {
                if (Input.IsKeyDown(Keys.A) == true)
                {
                    MoveLeft();
                }
                else if (Input.IsKeyDown(Keys.D) == true)
                {
                    MoveRight();
                }

                if (Input.IsKeyDown(Keys.W) == true)
                {
                    Jump(map);
                }
            }

            if (Input.KeyPressed(Keys.J))
            {
                Fire();
            }

        }

        public override void UpdateAnimation()
        {
            if (currentAnimation == null)
            {
                return;
            }

            base.UpdateAnimation();

            if (velocity != Vector2.Zero || jumping == true)
            {
                if (dir.X < 0 && AnimationIsNot(Animations.WalkLeft))
                {
                    ChangeAnimation(Animations.WalkLeft);
                }
                else if (dir.X > 0 && AnimationIsNot(Animations.WalkRight))
                {
                    ChangeAnimation(Animations.WalkRight);
                }
            }
            else if (velocity == Vector2.Zero && jumping == false)
            {
                if (dir.X < 0 && AnimationIsNot(Animations.Idle))
                {
                    ChangeAnimation(Animations.Idle);
                }
                else if (dir.X > 0 && AnimationIsNot(Animations.Idle))
                {
                    ChangeAnimation(Animations.Idle);
                }
            }
        }
    }
}
