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
    public class Attacker : Character
    {
        List<Bullet> bullets = new List<Bullet>();

        const int NUMOFBULLETS = 20;

        public Attacker()
        {

        }

        public override void Initialize()
        {
            if (bullets.Count == 0)
            {
                for (int i = 0; i < NUMOFBULLETS; i++)
                {
                    bullets.Add(new Bullet());
                }
            }

            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            for (int i = 0; i < NUMOFBULLETS; i++)
            {
                bullets[i].Load(content);
            }

            base.Load(content);
        }

        public override void Update(List<GameObject> objs, Map map)
        {
            for (int i = 0; i < NUMOFBULLETS; i++)
            {
                bullets[i].Update(objs, map);
            }

            base.Update(objs, map);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < NUMOFBULLETS; i++)
            {
                bullets[i].Draw(spriteBatch);
            }

            base.Draw(spriteBatch);
        }

        public void Fire()
        {
            for (int i = 0; i < NUMOFBULLETS; i++)
            {
                if (bullets[i]._active == false)
                {
                    bullets[i].Fire(this, _position, dir);
                    break;
                }
                
            }
        }
    }
}
