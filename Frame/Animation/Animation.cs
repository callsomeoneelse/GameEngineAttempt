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
    public class Animation  //single animations
    {
        public string name;
        public List<int> animationOrder = new List<int>();
        public int speed;

        public Animation()
        {

        }

        public Animation(string inpName, int inpSpeed, List<int> inputAnimationOrder)
        {
            name = inpName;
            speed = inpSpeed;
            animationOrder = inputAnimationOrder;          
        }

    }

    public class AnimationSet
    {
        public int width;
        public int height;
        public int gridX;
        public int cellY;
        public List<Animation> animationList = new List<Animation>();

        public AnimationSet()
        {

        }

        public AnimationSet(int inpWidth, int inpHeight, int inpCellX, int inpCellY)
        {
            width = inpWidth;
            height = inpHeight;
            gridX = inpCellX;
            cellY = inpCellY;
        }
    }

    public class AnimationData
    {
        public AnimationSet animation { get; set; }
     
        public string texPath { get; set; }
  
    }




}
