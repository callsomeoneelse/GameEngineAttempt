using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frame
{
    public class Bullet : GameObject
    {
        const float SPEED = 12f;

        Character shooter;

        private int _destroyTimer;
        const int MAXTIMER = 180;

        SoundEffect staffshot;

        public Bullet()
        {
            _active = false;
        }

        public override void Load(ContentManager content)
        {
            _image = TexFac.Load("staffshot.png", content);
            staffshot = content.Load<SoundEffect>("Audio//flaunch.wav");
            base.Load(content);
        }

        public override void Update(List<GameObject> objs, Map map)
        {
            if (_active == false)
            {
                return;
            }

            _position += dir * SPEED; //bullet movement

            CheckColl(objs, map);


            
            _destroyTimer--;
            if(_destroyTimer <= 0 && _active == true) //remove bullet after period of time
            {
                Destroy();
            }    


            base.Update(objs, map);  
        }

        public void CheckColl(List<GameObject> objs, Map map)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i]._active == true && objs[i] != shooter && objs[i].CheckCollision(BoundingBox) == true)
                {
                    Destroy();
                    objs[i].BulletInteract();
                    return;
                }
            }

            if (map.CheckColl(BoundingBox) != Rectangle.Empty)
            {
                Destroy();
            }
        }

        public void Fire(Character inputShooter, Vector2 inputPos, Vector2 inputDir)
        {
            shooter = inputShooter;
            _position = inputPos;
            dir = inputDir;
            _active = true;
            _destroyTimer = MAXTIMER;
            staffshot.Play();

        }
        public void Destroy()
        {
            _active = false;
        }
    }
}
