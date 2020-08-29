using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LydsTextAdventure
{

    public class Block
    {

        [Serializable]
        public enum Types : byte
        {
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

        public static readonly Dictionary<Types, char> Textures;

        static Block()
        {

            if (!File.Exists("textures.json"))
                throw new FileNotFoundException("textures file missing");
            else
                Textures = JsonSerializer.Deserialize<Dictionary<Types, char>>(File.ReadAllText("textures.json"));

            Debug.WriteLine("loaded textures");
        }
    }
}
