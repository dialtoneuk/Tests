using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using static LydsTextAdventure.Game;

namespace LydsTextAdventure
{

    public class Block : Command
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
            DEEP_WATER,
            BED_ROCK,
            MOUNTAIN_GRASS,
            TRENCH
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

        public void AddCommands(Game game)
        {

            Dictionary<string, Action<object[]>> commands = new Dictionary<string, Action<object[]>>()
            {
                { "add_texture",
                    command =>
                    {

                        if(command.Length<3)
                            return;

                        Block.AddTexture((Block.Types)command[1], command[2].ToString()[0]);
                    }
                },
                { "modify_texture",
                    command =>
                    {

                        if(command.Length<3)
                            return;

                        if(!Game.CommandIsInt(command[1]))
                            return;

                        Block.Types type = (Block.Types)command[1];
                        char character = command[2].ToString()[0];

                        if(Block.Textures.ContainsKey((Block.Types)command[1]))
                        {
                            Block.Textures[type] = character;
                            Debug.WriteLine("modified texture {0} {1}", type, character);
                        }
                        else
                           Console.WriteLine("texture not already present, add_texture instead");

                    }
                },
                { "textures",
                    command =>
                    {

                        var elements = Enum.GetNames(typeof(Block.Types));


                        int key=0;
                        foreach(string name in elements)
                        {

                            char texture;
                            if(Block.Textures.ContainsKey((Block.Types)key))
                                texture =  Block.Textures[(Block.Types)key];
                            else
                                texture = 'N';

                            Console.WriteLine("{0} {1} {2}", name, key, texture);
                            key++;
                        }

                    }
                },
                { "save_textures",
                    command =>
                    {

                       Block.SaveTextures();
                    }
                },
            };

            foreach (KeyValuePair<string, Action<object[]>> action in commands)
                game.AddCommand(action.Key, action.Value);
        }

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

        public static void AddDeveloperCommands(Game game)
        {

            Block block = new Block();
            block.AddCommands(game);
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
