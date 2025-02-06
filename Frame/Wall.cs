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
    public class Wall
    {
        public Rectangle wall;
        public bool active = true;

        public Wall()
        {

        }

        public Wall(Rectangle inputRect)
        {
            wall = inputRect;
        }
    }
}
