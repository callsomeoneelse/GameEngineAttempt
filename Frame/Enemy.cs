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
    public class Enemy : Character
    {
        private int respawnTime;
        const int maxRespawnTime = 60;

        Random random = new Random();

        SoundEffect destroy;

        public Enemy()
        {

        }

        public Enemy(Vector2 inputPos)
        {
            _position = inputPos;
        }

        public override void Initialize()
        {
            _active = true;
            _collidable = false;
            _position.X = random.Next(0, 780);

            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            _image = TexFac.Load("goblin", content);
            destroy = content.Load<SoundEffect>("Audio//rock_breaking.wav");

            base.Load(content);
        }

        public override void Update(List<GameObject> objs, Map map)
        {
            if (respawnTime > 0)
            {
                respawnTime--;
                if (respawnTime <= 0)
                {
                    Initialize();
                }
            }

            base.Update(objs, map);
        }
        public override void BulletInteract()
        {
            _active = false;
            respawnTime = maxRespawnTime;

            Player.score++;
            destroy.Play();



            base.BulletInteract();
        }
    }
}
