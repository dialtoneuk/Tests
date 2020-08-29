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
        private int worldDepth;
        private string worldName;
        private int worldSeed;

        protected float noiseFrequency;
        protected int structureCount;
        protected int foliageCount;

        private int[,,] worldData;

        protected int[,,] WorldData { get => worldData; set => worldData = value; }

        //constructor
        public World(int width, int height, int depth, string worldname="default")
        {

            worldWidth = width;
            worldHeight = height;
            worldDepth = depth;
            worldName = worldname;
            worldSeed = GenerateSeed();
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

        public void DrawWorld(int startx=0, int starty=0, int drawx=0, int drawy=0, int z=1)
        {

            if (z >= worldDepth)
                z = worldDepth - 1;

            int max_width = Console.WindowWidth - 12;
            int max_height = Console.WindowHeight - 2;

            for (int y = 0; y < worldHeight - drawy; y++)
            {
                for (int x = 0; x < worldWidth - drawx; x++)
                {

                    if ((drawy + y) > worldHeight || (drawx + x) > worldWidth)
                        if(x%2==0)
                            Console.Write("\\");
                          else
                            Console.Write("/");


                    if ((drawy + y) < 0 || (drawx + x) < 0)
                        if (x % 2 == 0)
                            Console.Write("\\");
                        else
                            Console.Write("/");


                    Block.Types block = GetBlock(x, y, z);

                    int _z = z;
                    while(block == Block.Types.AIR)
                    {
                        if (_z + 1 < worldDepth)
                            block = GetBlock(x, y, ++_z);
                        else
                            block = Block.Types.BED_ROCK;
                    }
                   
                    if (Block.Textures != null && Block.Textures.ContainsKey(block))
                        Console.Write(Block.Textures[block]);
                    else if(block == Block.Types.AIR)
                        Console.Write(" ");
                    else
                    {

                        Debug.WriteLine("no texture for {0}", block);
                        Console.Write("X");
                    }

                    if (x >= max_width)
                    {
                        Console.SetCursorPosition(startx, starty + (y + 1));
                        break;
                    }                  
                }

                if (y >= max_height)
                    break;
            }                  
        }

        public int GenerateSeed()
        {

            Random r = new Random();
            return (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 2 * r.Next(10, 100);
        }

        //Generates the world
        public int[,,] GenerateWorld()
        {

            WorldData = new int[worldWidth,worldHeight,worldDepth];
            FastNoise noise = new FastNoise();
            Random r = new Random();

            noise.SetSeed(worldSeed);
            noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            noise.SetInterp(FastNoise.Interp.Hermite);
            noise.SetFractalOctaves(6);
            noise.SetFractalGain(0.350f);
            noise.SetFrequency(Program.Settings.GetFloatOrZero("noise_frequency"));

            for (int x = 0; x < worldWidth; x++)
                for (int y = 0; y < worldHeight; y++)
                    for (int z = 0; z < worldDepth; z++)
                    {

                        float noiseValue = noise.GetPerlinFractal((float)x, (float)y, (float)z) * (Enum.GetValues(typeof(Block.Types)).Length + worldDepth);
                        noiseValue = noiseValue * 1.5f;
                        
                        if(z==0)
                            SetBlock(x, y, z, Block.Types.AIR);

                        if (r.Next(1, 100) < 50)
                            noiseValue = (float)Math.Floor(noiseValue);
                        else
                            noiseValue = (float)Math.Round(noiseValue);

                        if (z == 1)
                        {
                            switch (Math.Abs(noiseValue))
                            {
                                case 0.0f:
                                    SetBlock(x, y, z, Block.Types.SNOW);
                                    break;
                                case 1.0f:
                                    SetBlock(x, y, z, Block.Types.STONE);
                                    break;
                                case 2.0f:
                                    SetBlock(x, y, z, Block.Types.MOUNTAIN_GRASS);
                                    break;
                                case 3.0f:
                                    SetBlock(x, y, z, Block.Types.GRASS);
                                    break;
                                case 4.0f:
                                    SetBlock(x, y, z, Block.Types.DIRT);
                                    break;
                                case 5.0f:
                                    SetBlock(x, y, z, Block.Types.SAND);
                                    break;
                                case 7.0f:
                                case 6.0f:
                                    SetBlock(x, y, z, Block.Types.WATER);
                                    break;
                                case 8.0f:
                                case 9.0f:
                                    SetBlock(x, y, z, Block.Types.DEEP_WATER);
                                    break;
                                case 10.0f:
                                    SetBlock(x, y, z, Block.Types.TRENCH);
                                    break;
                            }
                        }
                        else if (z == worldDepth - 1)
                            SetBlock(x, y, z, Block.Types.BED_ROCK);
                        else
                        if (z > 1)
                        {
                            if (noiseValue < 0.75f)
                                SetBlock(x, y, z, Block.Types.STONE);
                            else if (noiseValue < 0.35f)
                                SetBlock(x, y, z, Block.Types.DIRT);
                            else
                                SetBlock(x, y, z, Block.Types.STONE);
                        }            
                    }
                           
            Debug.WriteLine("generated new world");
            return WorldData;
        }

        public Block.Types GetBlock(int x, int y, int z)
        {

            return (Block.Types)WorldData[x,y,z];
        }

        public void SetBlock(int x, int y, int z, Block.Types block )
        {

            WorldData[x,y,z] = (int)block;
        }

        //Saves the world
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            string filename = String.Format("saves/{0}.world", worldName);

            if (!Directory.Exists("saves/"))
                Directory.CreateDirectory("saves/");

            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, worldData);
            stream.Close();

            Debug.WriteLine("saved world {0}", (object)worldName);
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

            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);

            World world = NewWorld();
            world.WorldData = (int[,,])formatter.Deserialize(stream);
            stream.Close();


            Debug.WriteLine("loaded world {0}", (object)world.worldName);

            return world;
        }
    
        //Creatates a new world
        public static World NewWorld(string worldname="default")
        {

            return new World(Program.Settings.GetIntOrZero("world_width"), Program.Settings.GetIntOrZero("world_height"), Program.Settings.GetIntOrZero("world_depth"), worldname);
        }
    }
}
