using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace LydsTextAdventure
{

    public class Block
    {

        //add new blocks to the end of this enum else expect issues
        [Serializable]
        public enum Types : int
        {
            AIR,
            WOOD,
            STONE,
            WALL_HORIZONTAL,
            WALL_VERTICAL,
            WALL,
            FLOOR_BASIC,
            FLOOR_WOOD,
            GRASS,
            DIRT,
            SAND,
            WATER,
            SNOW,
            DEEP_WATER
        }

        public readonly Dictionary<Types, bool> Solids = new Dictionary<Types, bool>
        {
           {Types.WALL, true },
           {Types.WALL_HORIZONTAL, true },
           {Types.WALL_VERTICAL, true },
        };

        public readonly Dictionary<Types, int> Breakable = new Dictionary<Types, int>
        {
           {Types.WOOD, 100 },
           {Types.STONE, 200 },
        };

        public static Dictionary<Types, char> Textures;

        static Block()
        {

            string filename = String.Format("textures.pak");

            if (!File.Exists(filename))
                Debug.WriteLine("unable to find {0}", (object)filename);
            else
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);

                Textures = (Dictionary<Types, char>)formatter.Deserialize(stream);
                stream.Close();

                Debug.WriteLine("loaded {0}", (object)filename);
            }
        }

        public static void SaveTextures()
        {

            string filename = String.Format("textures.pak");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, Textures);
            stream.Close();
            Debug.WriteLine("saved {0}", (object)filename);
        }

        public static void AddTexture(Types type, char character)
        {

            if (Textures == null)
                Textures = new Dictionary<Types, char>();

            if (Textures.ContainsKey(type))
                return;

            Textures.Add(type, character);
            Debug.WriteLine("added texture {0} {1}", type, character);
        }
    }
}
