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
using static Frame.Animation;

namespace Frame
{
    public static class AnimationLoader
    {
        /// <summary>
        /// fills object with animation content
        /// </summary>
        public static AnimationData Load(string name)
        {
            //Load XML data
            XmlSerializer serializer = new XmlSerializer(typeof(AnimationData));
            TextReader reader = new StreamReader("Resources//Animations//" + name);
            AnimationData obj = (AnimationData)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }
    }
}
