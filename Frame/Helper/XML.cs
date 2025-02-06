using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Frame
{
    public static class XML
    {
        public static LevelData Load(string fileName) //loads file into leveldata object
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
            TextReader reader = new StreamReader(fileName);
            LevelData obj = (LevelData)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static void Save<T>(T obj, string fileName)
        {
            TextWriter writer = new StreamWriter(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, obj);
            writer.Close();
        }
    }
}
