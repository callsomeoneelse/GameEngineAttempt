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
    public class Map
    {
        public List<Wall> walls = new List<Wall>();
        public List<Decoration> decorations = new List<Decoration>();

        Texture2D wallImg;

        public int mapW = 25;
        public int mapH = 23;
        public int tileSize = 32;

        public void LoadMap(ContentManager content)
        {
            for (int i = 0; i < decorations.Count; i++)
            {
                decorations[i].Load(content, decorations[i].imgPath);
            }
        }

        public void Update(List<GameObject> obj)
        {
            for (int i = 0; i < decorations.Count; i++)
            {
                decorations[i].Update(obj, this);
            }
        }

        public void Load(ContentManager content)
        {
            wallImg = TexFac.Load("pixel", content);
        }

        public Rectangle CheckColl(Rectangle input)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].wall.Intersects(input) == true)
                {
                    return walls[i].wall;
                }          
            }
            return Rectangle.Empty;
        }

        public void DrawWalls(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].active == true)
                {
                    spriteBatch.Draw(wallImg, new Vector2(walls[i].wall.X, walls[i].wall.Y), walls[i].wall, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.7f);
                }
            }
        }

        public Point GetTileIDX(Vector2 inputPos)
        {
            if (inputPos == new Vector2(-1, -1))
            {
                return new Point(-1, -1);
            }

            return new Point((int)inputPos.X / tileSize, (int)inputPos.Y / tileSize);
        }
    }

    

}
