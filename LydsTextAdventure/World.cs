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
    public class World : Command
    {

        private int worldWidth;
        private int worldHeight;
        private string worldName;

        protected float noiseFrequency;
        protected int structureCount;
        protected int foliageCount;

        private Block.Types[,] worldData;

        protected Block.Types[,] WorldData { get => worldData; set => worldData = value; }

        //constructor
        public World(int width, int height, string worldname="default")
        {

            worldWidth = width;
            worldHeight = height;
            worldName = worldname;
        }

        //adds our abbreviations for shorthand mode
        static World()
        {

            Game.abbreviations.Add("save", new string[]
            {
                "s"
            });
        }

        //adds our game commands
        public void AddCommands(Game game)
        {

            game.AddCommand("regenerate", (command) =>
            {
                GenerateWorld();
            });

            game.AddCommand("world_parameters", (command) =>
            {
                Console.WriteLine("{0} {1} {2} {3} {4} {5}", GetWorldParameters());
            });

            game.AddCommand("save", (command) =>
            {
                Save();
            });
        }

        //Generates the world
        public Block.Types[,] GenerateWorld()
        {

            WorldData = new Block.Types[worldWidth, worldHeight];

            Debug.WriteLine("generated new world");
            return WorldData;
        }

        //Saves the world
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            string filename = String.Format("saves/{0}.world", worldName);

            if (!Directory.Exists("saves/"))
                Directory.CreateDirectory("saves/");

            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, WorldData);
            stream.Close();

            Debug.WriteLine("saved world {0}", worldName);
        }

        //Gets world parameters in the form of an object
        public object[] GetWorldParameters()
        {
            object[] parameters = {
                worldWidth,
                worldHeight,
                worldName,
                noiseFrequency,
                structureCount,
                foliageCount
            };

            return parameters;
        }

        //Sets the world parameters in the form of an object
        public void SetWorldParameters(object[] parameters)
        {

           for(int i = 0; i < parameters.Length; i ++)
                switch(i)
                {
                    case 0:
                        noiseFrequency = (float)parameters[i];
                        break;
                    case 1:
                        structureCount = (int)parameters[i];
                        break;
                    case 2:
                        foliageCount = (int)parameters[i];
                        break;
                }
        }

        //Loads a world from the worldname
        public static World LoadWorld(string worldname="default")
        {
            IFormatter formatter = new BinaryFormatter();
            string filename = String.Format("saves/{0}.world", worldname);

            if (!Directory.Exists("saves/"))
                Directory.CreateDirectory("saves/");

            if (!File.Exists(filename))
                return null;

            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

            World world = NewWorld();
            world.WorldData = (Block.Types[,])formatter.Deserialize(stream);

            return world;
        }
    
        //Creatates a new world
        public static World NewWorld(string worldname="default")
        {

            return new World(Program.Settings.GetIntOrZero("world_width"), Program.Settings.GetIntOrZero("world_height"), worldname);
        }
    }
}
