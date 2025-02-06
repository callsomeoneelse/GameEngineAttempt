using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frame
{
    public class LevelData
    {
        [XmlElement("Player", Type = typeof(Player))]
        [XmlElement("Enemy", Type = typeof(Enemy))]
        public List<GameObject> objects { get; set; }

        public List<Wall> walls { get; set; }

        public List<Decoration> decoration { get; set; }

        public int mapW { get; set; }
        public int mapH { get; set; }
    }
}
