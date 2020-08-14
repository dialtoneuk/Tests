using System;
using System.Collections.Generic;
using System.IO;

namespace TextAdventure
{

    public class World
    {
        public enum Blocks : int
        {
            WALL_INVISIBLE,
            DOOR_CLOSED,
            DEEP_WATER,
            WALL_SOLID_HORIZONTAL,
            WALL_SOLID_VERTICAL,
            WOOD,
            WALL_SOLID, //Everything below this is treated as a solid object by the games collision detection and placement detection
            DOOR_OPEN,
            WATER,
            FLOOR,
            GRASS,
            GRAVEL,
            SAND,
            DIRT,
            HILLY_GRASS,
            MOUNTAIN_GRASS,
            STONE,
            SNOW,
            DEEP_SNOW,
        }

        public enum Foliage : int
        {
            NULL,
            BUSH_BERRIES,
            BUSH_BLACKBERRIES,
            TREE,
            TREE_OAK,
            TREE_PINE,
            BUSH_BLUEBERRIES,
            BUSH_STRAWBERRIES,
            TREE_APPLE,
            TREE_GOLDEN_APPLE,
            TREE_HONEY,
            TREE_PLUM,
            TREE_PEAR,
            PLANT_EGGPLANT,
            PLANT_ROSES,
            PLANT_MANA,
            PLANT_OXYGEN,
            PLANT_XP,
        }

        public enum Structures : int
        {

            NULL,
            RUIN_SMALL,
            RUIN_MEDIUM,
            RUIN_LARGE,
            RUIN_HUGE,
            POND,
            POND_LARGE,
        }

        public readonly Dictionary<Structures, Blocks[,]> structures = new Dictionary<Structures, Blocks[,]>
        {
            //
        };

        public readonly Dictionary<Foliage, string> foliageTextures = new Dictionary<Foliage, string>
        {
            { Foliage.NULL, " " },
            { Foliage.BUSH_BERRIES, "b" },
            { Foliage.BUSH_BLACKBERRIES, "B" },
            { Foliage.TREE, "T" },
            { Foliage.TREE_PINE, "T" },
            { Foliage.TREE_OAK, "T" },
            { Foliage.TREE_PLUM, "P" },
            { Foliage.TREE_PEAR, "p" },
            { Foliage.BUSH_BLUEBERRIES, "b" },
            { Foliage.BUSH_STRAWBERRIES, "S" },
            { Foliage.TREE_APPLE, "A" },
            { Foliage.TREE_GOLDEN_APPLE, "G" },
            { Foliage.TREE_HONEY, "H" },
            { Foliage.PLANT_EGGPLANT, "8" },
            { Foliage.PLANT_ROSES, "R" },
            { Foliage.PLANT_MANA, "M" },
            { Foliage.PLANT_OXYGEN, "+" },
            { Foliage.PLANT_XP, "$" },
        };

        public readonly Dictionary<Foliage, ConsoleColor> foliageColours = new Dictionary<Foliage, ConsoleColor>
        {
            { Foliage.NULL, ConsoleColor.Black },
            { Foliage.BUSH_BLUEBERRIES, ConsoleColor.Blue },
            { Foliage.BUSH_BLACKBERRIES, ConsoleColor.DarkBlue },
            { Foliage.TREE, ConsoleColor.Green },
            { Foliage.TREE_PINE, ConsoleColor.DarkGreen },
            { Foliage.TREE_OAK, ConsoleColor.DarkGreen },
            { Foliage.TREE_PEAR, ConsoleColor.Green },
            { Foliage.TREE_PLUM, ConsoleColor.Green },
            { Foliage.BUSH_BERRIES, ConsoleColor.Blue},
            { Foliage.BUSH_STRAWBERRIES,ConsoleColor.Red},
            { Foliage.TREE_APPLE, ConsoleColor.DarkGreen },
            { Foliage.TREE_GOLDEN_APPLE, ConsoleColor.Yellow },
            { Foliage.TREE_HONEY, ConsoleColor.DarkYellow },
            { Foliage.PLANT_EGGPLANT, ConsoleColor.Magenta },
            { Foliage.PLANT_ROSES, ConsoleColor.Red },
            { Foliage.PLANT_MANA, ConsoleColor.Blue },
            { Foliage.PLANT_OXYGEN, ConsoleColor.Red },
            { Foliage.PLANT_XP, ConsoleColor.Cyan },
        };

        public readonly Dictionary<Blocks, string> blockTextures = new Dictionary<Blocks, string>
        {
            { Blocks.DEEP_WATER, "/" },
            { Blocks.WATER, "\\" },
            { Blocks.WALL_SOLID, "0" },
            { Blocks.WALL_SOLID_HORIZONTAL, "-" },
            { Blocks.WALL_SOLID_VERTICAL, "|" },
            { Blocks.WALL_INVISIBLE, " " },
            { Blocks.DIRT, "." },
            { Blocks.SAND, "~" },
            { Blocks.GRASS, "'" },
            { Blocks.STONE, "`" },
            { Blocks.SNOW, "^" },
            { Blocks.DEEP_SNOW, "#" },
            { Blocks.HILLY_GRASS, "," },
            { Blocks.MOUNTAIN_GRASS, "." },
            { Blocks.DOOR_CLOSED, "C" },
            { Blocks.DOOR_OPEN, "O" },
            { Blocks.FLOOR, "_" },
            { Blocks.WOOD, "=" },
            { Blocks.GRAVEL, "n" },
        };

        private Dictionary<Blocks, ConsoleColor> blockColours = new Dictionary<Blocks, ConsoleColor>
        {
            { Blocks.DEEP_WATER, ConsoleColor.DarkBlue },
            { Blocks.WATER, ConsoleColor.Blue},
            { Blocks.DIRT, ConsoleColor.DarkYellow },
            { Blocks.SAND, ConsoleColor.Yellow },
            { Blocks.GRASS, ConsoleColor.Green},
            { Blocks.HILLY_GRASS, ConsoleColor.DarkGreen},
            { Blocks.STONE, ConsoleColor.DarkGray},
            { Blocks.GRAVEL, ConsoleColor.DarkGray},
            { Blocks.SNOW, ConsoleColor.Gray},
            { Blocks.DEEP_SNOW, ConsoleColor.White},
            { Blocks.MOUNTAIN_GRASS, ConsoleColor.Green },
            { Blocks.DOOR_CLOSED, ConsoleColor.Red },
            { Blocks.DOOR_OPEN, ConsoleColor.Green },
            { Blocks.WOOD, ConsoleColor.DarkYellow },
        };

        public const bool ROOM_SYMMETICAL = true;
        public const int ROOM_MAX_SIZE = 20;
        public const int ROOM_MIN_SIZE = 12;
        public const int ROOM_MAX = 2048;
        public const int ROOM_BUFFER = 4; //do not change
        public const int ROOM_DOOR_SIZE = 3;
        public const int WORLD_MAX = 64000;
        public const int WORLD_GENERATION_TIMEOUT = 200; //seconds
        public const int ROOM_GENERATION_TIMEOUT = WORLD_GENERATION_TIMEOUT / 4; //seconds
        public const int FOLIAGE_GENERATION_TIMEOUT = WORLD_GENERATION_TIMEOUT / 4; //seconds
        public const int STRUCTURE_GENERATION_TIMEOUT = WORLD_GENERATION_TIMEOUT / 4; //seconds
        public const int WORLD_BUFFER = 4; //do not change
        public const int WORLD_MAX_FOLIAGE = 64000;
        public const int STRUCTURE_MAX_SIZE = 16;
        public const float FREQUENCY_INTERVAL = 0.000075f;

        private Blocks[,] worldData;
        private int[][] roomData;
        private int[,] foliageData;
        private int worldWidth = WORLD_MAX;
        private int worldHeight = WORLD_MAX;

        private static int consoleWidth;
        private static int consoleHeight;
        private static int islandValue;
        private static int roomCount;
        private static int[] spawnRoom;

        public Blocks[,] WorldData { get => worldData; }
        public int[][] RoomData { get => roomData; }
        public int[,] FoliageData { get => foliageData; }
        public int WorldWidth { get => worldWidth; }
        public int WorldHeight { get => worldHeight; }
        public static int IslandValue { get => islandValue; }
        public static int RoomCount { get => roomCount; }
        public static int[] SpawnRoom { get => spawnRoom; set => spawnRoom = value; }

        public World()
        {

            if (this.worldWidth == 0)
                this.worldWidth = WORLD_MAX;

            if (this.worldHeight == 0)
                this.worldHeight = WORLD_MAX;
        }

        public World(int width, int height)
        {

            this.worldData = new Blocks[width, height];
            this.worldWidth = width;
            this.worldHeight = height;

            consoleWidth = Console.WindowWidth;
            consoleHeight = Console.WindowHeight;
        }

        /**
        public void save(string file_name="world1")
        {
            //TODO: coming soon since this is pretty damn complex tbf
        }

        private void saveWorld(World world, string file_name = "world1")
        {
            XmlSerializer x = CreateOverrider();
            Stream file = new FileStream($"saves/{file_name}", FileMode.Create, FileAccess.Write);
            x.Serialize(file, world);
            file.Close();
        }

        private World loadWorld(string file_name = "world1")
        {

            if (File.Exists($"saves/{file_name}"))
            {
                XmlSerializer x = CreateOverrider();
                Stream file = new FileStream($"saves/{file_name}", FileMode.Create, FileAccess.Write);
                World world = (World)x.Deserialize(file);
                return (world);
            }
            else
                throw new FileNotFoundException($"saves/{file_name} not found");
        }

        private static XmlSerializer CreateOverrider()
        {
          
        XmlAttributes attrs = new XmlAttributes();

            attrs = new XmlAttributes();
            attrs.XmlIgnore = true;
            xOver.Add(typeof(World), "structures", attrs);
            xOver.Add(typeof(World), "blockColours", attrs);
            xOver.Add(typeof(World), "blockTextures", attrs);
            xOver.Add(typeof(World), "foliageColours", attrs);
            xOver.Add(typeof(World), "foliageTextures", attrs);

            XmlSerializer xSer = new XmlSerializer(typeof(World), xOver);
            return xSer;
        }
        **/

        public static void printWorld(Player player, World world, int buffer_zone = 5)
        {

            int scale;
            if (Player.CAMERA_ZOOM_FACTOR > 1)
                scale = Player.CAMERA_ZOOM_FACTOR;
            else
                scale = 1;

            if (world.worldData.Length == 0)
                return;

            int xcounter = 0;
            int ycounter = 0;

            for (int y = player.Position[1] - (((Player.CAMERA_HEIGHT - buffer_zone)) / 2 + (1 / scale)) * scale; y < player.Position[1] + (((Player.CAMERA_HEIGHT - buffer_zone)) / 2 + (1 / scale)) * scale; y++)
            {

                xcounter = 0;

                if (ycounter > Program.WINDOW_HEIGHT - buffer_zone)
                    continue;

                if (ycounter > consoleHeight - buffer_zone)
                    continue;

                if (Player.CAMERA_ZOOM_FACTOR > 1)
                    if (y % Player.CAMERA_ZOOM_FACTOR == 0)
                        if (y != player.Position[1])
                            continue;

                for (int x = player.Position[0] - (((Player.CAMERA_WIDTH - buffer_zone)) / 2 + (1 / scale)) * scale; x < player.Position[0] + (((Player.CAMERA_WIDTH - buffer_zone)) / 2 + (1 / scale)) * scale; x++)
                {



                    if (x == player.Position[0] && y == player.Position[1])
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Program.setColour(ConsoleColor.Black);
                        Program.write("P");
                        Console.BackgroundColor = ConsoleColor.Black;
                        xcounter++;
                        continue;
                    }

                    if (Player.CAMERA_ZOOM_FACTOR > 1)
                        if (x % Player.CAMERA_ZOOM_FACTOR == 0)
                            continue;

                    if (xcounter > Program.WINDOW_WIDTH - buffer_zone)
                        continue;

                    if (xcounter > consoleWidth - buffer_zone)
                        continue;

                    if (y >= world.worldHeight || x >= world.worldWidth)
                    {

                        Program.setColour(ConsoleColor.Gray);

                        if (y % 2 == 0 && x % 2 == 0)
                            Program.write("\\");
                        else
                            Program.write("/");

                        continue;
                    }

                    if (y < 0 || x < 0)
                    {

                        Program.setColour(ConsoleColor.DarkGray);

                        if (y % 2 == 0 && x % 2 == 0)
                            Program.write("\\");
                        else
                            Program.write("/");
                        continue;
                    }

                    if (world.hasFoliage(x, y) && world.getFoliage(x, y) != Foliage.NULL)
                    {
                        Foliage foliage = world.getFoliage(x, y);

                        Program.setColour(world.foliageColours[foliage]);
                        Program.write(world.foliageTextures[foliage]);
                    }
                    else
                    {

                        Blocks block;

                        if (Player.CAMERA_ZOOM_FACTOR != 0)
                        {
                            if (x % Player.CAMERA_ZOOM_FACTOR + 1 == 1 && y % Player.CAMERA_ZOOM_FACTOR + 1 == 1)
                                block = (Blocks)world.worldData[x, y];
                            else
                            {

                                if (x - 1 > 0 && y + 1 < world.worldHeight)
                                    block = (Blocks)world.worldData[x - 1, y + 1];
                                else if (x + 1 < world.worldWidth && y - 1 > 0)
                                    block = (Blocks)world.worldData[x + 1, y - 1];
                                else if (x - 2 > 0 && y + 2 < world.worldHeight)
                                    block = (Blocks)world.worldData[x - 2, y + 2];
                                else if (x + 1 < world.worldWidth && y - 1 > 0)
                                    block = (Blocks)world.worldData[x + 2, y - 2];
                                else
                                    block = Blocks.WALL_INVISIBLE;
                            }
                        }
                        else
                            block = (Blocks)world.worldData[x, y];

                        if (world.isRoomCenter(x, y))
                            if(world.getRoom(x,y)[4]==1)
                                Program.write("C");
                            else
                                Program.write("R");
                        else
                        {
                            Program.setColour(world.getColor(block));
                            Program.write(world.blockTextures[block]);
                        }
                    }

                    xcounter++;
                }

                Program.writeLine();

                ycounter++;
            }
        }

        public void loadStructures()
        {

            Program.setColour(ConsoleColor.Red);
            Console.WriteLine("reading structures");

            foreach (Structures s in Enum.GetValues(typeof(Structures)))
            {

                Blocks[,] structure = new Blocks[STRUCTURE_MAX_SIZE, STRUCTURE_MAX_SIZE];

                string file_name = "structures/" + Enum.GetName(typeof(Structures), s).ToLower() + ".st";

                if (System.IO.File.Exists(file_name))
                {
                    string[] file = System.IO.File.ReadAllLines(file_name);

                    int x = 0;
                    int y = 0;
                    int times = 0;

                    foreach (string line in file)
                    {

                        if (y >= STRUCTURE_MAX_SIZE)
                            continue;

                        for (int i = 0; i < line.Length; i++)
                        {


                            if (x >= STRUCTURE_MAX_SIZE)
                                continue;

                            if (line[i] == '_')
                            {
                                continue;
                            }

                            if (line[i] == '^' || line[i] == '*' || line[i] == '+')
                            {
                                times++;
                                continue;
                            }

                            if (line[i] == '~')
                            {
                                times = 2;
                                continue;
                            }

                            if (line[i] == '.')
                            {
                                times = 3;
                                continue;
                            }

                            if (line[i] == '`')
                            {
                                times = 4;
                                continue;
                            }

                            if (int.TryParse(line[i].ToString(), out int parse))
                            {

                                if (times > 0)
                                {

                                    if (parse == 0)
                                        parse = (10 * times);
                                    else
                                        parse = parse + (10 * times);

                                    times = 0;
                                }

                                structure[x, y] = (Blocks)parse;
                            }
                            else
                                throw new ApplicationException(String.Format("invalid data in structure file at {0}/{1}", x, y));

                            x++;
                        }
                        x = 0;
                        y++;
                    }
                }
                else
                    throw new FileNotFoundException("could not find structure file", file_name);

                Program.setColour(ConsoleColor.Red);
                Console.WriteLine("read structure {0}", file_name);

                this.structures.Add(s, structure);
            }


            Program.setColour(ConsoleColor.Green);
            Console.WriteLine("finished reading structures");
        }

        public void generateWorld(int rooms, int foliage = 100, int island_value = 1, int structure_amount = 64)
        {

            if (this.roomData == null)
                this.roomData = new int[rooms][];

            if (this.foliageData == null)
                this.foliageData = new int[foliage, foliage];

            if (this.worldData == null)
                this.worldData = new Blocks[worldWidth, worldHeight];

            roomCount = 0;
            islandValue = island_value;
            terraform(island_value);

            var later = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(WORLD_GENERATION_TIMEOUT));

            while (!placeRooms(rooms) && DateTimeOffset.UtcNow < later)
            {
                destroyWorld();

                if (islandValue - 1 > 1)
                {

                    terraform(islandValue - 1);

                    Program.setColour(ConsoleColor.Red);
                    Console.WriteLine("attempting to place rooms with new island value of {0}", islandValue);
                }
            }

            if (roomCount == 0)
                throw new Exception($"failed to place {rooms} rooms in world size of w{worldWidth}/h{worldHeight}");

            placeDoors();
            connectDoors();
            randomizeColours();
            placeStructures(structure_amount);
            generateFoliage(foliage);
        }

        public void placeStructures(int amount = 12)
        {

            Program.setColour(ConsoleColor.Yellow);
            Console.WriteLine("placing structures...");


            Random r = new Random((int)DateTime.UtcNow.ToBinary());
            int beginningamount = amount;
            var later = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(STRUCTURE_GENERATION_TIMEOUT));

            while (amount > 0 && later > DateTime.UtcNow)
            {

                if (structures.Count == 0)
                    continue;

                int[] location = getRandomRoomPosition();
                Blocks[,] structure;

                if (r.Next(100) > 50)
                {

                    structure = structures[Structures.RUIN_SMALL];

                    if (r.Next(100) > 50)
                        structure = structures[Structures.RUIN_SMALL];
                    else
                        structure = structures[Structures.POND];
                }
                else
                {
                    structure = structures[Structures.RUIN_SMALL];

                    if (r.Next(100) > 50)
                    {
                        if (r.Next(100) > 50)
                            structure = structures[Structures.RUIN_HUGE];
                        else
                            structure = structures[Structures.POND_LARGE];
                    }
                }

                if (!canPlace(location[0], location[1], STRUCTURE_MAX_SIZE, STRUCTURE_MAX_SIZE, ROOM_BUFFER))
                    continue;
                else
                    for (int x = 0; x < STRUCTURE_MAX_SIZE; x++)
                        for (int y = 0; y < STRUCTURE_MAX_SIZE; y++)
                        {


                            if (structure[x, y] == Blocks.WALL_INVISIBLE)
                                continue;

                            worldData[location[0] + x, location[1] + y] = structure[x, y];
                        }


                amount--;
                Program.setColour(ConsoleColor.Red);
                Console.WriteLine("placed structure at {0}/{1} | {2} left", location[0], location[1], amount);
            }

            Program.setColour(ConsoleColor.Yellow);
            Console.WriteLine("finished.");
        }

        public void cleanFoliage()
        {

            this.foliageData = new int[WORLD_MAX_FOLIAGE, WORLD_MAX_FOLIAGE];
        }

        public void cleanRooms()
        {

            this.roomData = new int[ROOM_MAX][];
        }

        /**
         * Foliage generation
         **/
        public void generateFoliage(int amount = 10)
        {
            Random r = new Random((int)DateTime.UtcNow.ToBinary());
            int beginningtotal = amount;
            int modulas_amount = 500;

            var later = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(FOLIAGE_GENERATION_TIMEOUT));

            while (amount > 0 && later > DateTime.UtcNow)
            {
                for (int x = 0; x < worldWidth; x++)
                {

                    r.Next(1, 5);
                    for (int y = 0; y < worldHeight; y++)
                    {

                        r.Next(1, 5);

                        if (amount == 0)
                            break;

                        if ((Blocks)worldData[x, y] == Blocks.GRASS || (Blocks)worldData[x, y] == Blocks.MOUNTAIN_GRASS)
                        {

                            var v = Enum.GetValues(typeof(Foliage));

                            if (r.Next(30) < 10)
                            {

                                Foliage entity;

                                if (r.Next(60) < 30)
                                {

                                    if (r.Next(60) < 30)
                                    {
                                        var length = Enum.GetValues(typeof(Foliage)).Length;
                                        entity = (Foliage)Enum.GetValues(typeof(Foliage)).GetValue(r.Next(1, length - 1));
                                    }
                                    else
                                        if (r.Next(10) > 5)
                                        entity = Foliage.TREE_OAK;
                                    else
                                        entity = Foliage.TREE;

                                    int[] newEntity = new int[5];
                                    newEntity[0] = x;
                                    newEntity[1] = y;
                                    newEntity[2] = 1;
                                    newEntity[3] = 1;
                                    newEntity[4] = (int)entity;

                                    while ((!canPlace(newEntity) || hasFoliage(newEntity[0], newEntity[1])) && DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(5)) > DateTime.UtcNow)
                                    {

                                        int[] position = getRandomRoomPosition();
                                        newEntity[0] = position[0];
                                        newEntity[1] = position[1];
                                    }

                                    if (worldData[newEntity[0], newEntity[1]] == Blocks.SNOW)
                                        continue;

                                    if (amount < 3000)
                                        modulas_amount = 250;
                                    if (amount < 1500)
                                        modulas_amount = 100;
                                    if (amount < 1000)
                                        modulas_amount = 50;
                                    if (amount < 500)
                                        modulas_amount = 10;
                                    if (amount < 250)
                                        modulas_amount = 1;

                                    if (amount != 0 && amount % modulas_amount == 0)
                                    {
                                        if (Console.ForegroundColor != ConsoleColor.Blue)
                                            Console.ForegroundColor = ConsoleColor.Blue;

                                        Console.WriteLine("placed {0} foliage | {1} left", beginningtotal - amount, amount);
                                    }

                                    if (!hasFoliage(newEntity[0], newEntity[1]))
                                    {
                                        foliageData[newEntity[0], newEntity[1]] = newEntity[4];
                                        amount--;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        public void randomizeColours()
        {

            Random r = new Random((int)DateTime.UtcNow.ToBinary());
            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

            foreach (Blocks block in Enum.GetValues(typeof(Blocks)))
                if (block != Blocks.DOOR_CLOSED || block != Blocks.DOOR_OPEN)
                    if (!this.blockColours.ContainsKey(block))
                    {
                        ConsoleColor color = colors[r.Next(1, colors.Length)];

                        if (color == ConsoleColor.Black)
                            color = ConsoleColor.White;

                        this.blockColours.Add(block, color);
                    }


            ConsoleColor wall = this.blockColours[Blocks.WALL_SOLID];
            this.blockColours[Blocks.WALL_SOLID_HORIZONTAL] = wall;
            this.blockColours[Blocks.WALL_SOLID_VERTICAL] = wall;

        }

        public void destroyWorld()
        {

            this.roomData = null;
            this.foliageData = null;
            this.worldData = null;
            spawnRoom = null;
        }

        public void resetColours()
        {

            this.blockColours.Remove(Blocks.WALL_SOLID);
            this.blockColours.Remove(Blocks.WALL_SOLID_HORIZONTAL);
            this.blockColours.Remove(Blocks.WALL_SOLID_VERTICAL);
            this.blockColours.Remove(Blocks.FLOOR);
        }

        public void connectDoors()
        {

            Program.setColour(ConsoleColor.Yellow);

            Console.WriteLine("connecting doors...");


            for (int x = 0; x < worldWidth; x++)
                for (int y = 0; y < worldHeight; y++)
                {
                    if (worldData[x, y] == Blocks.DOOR_CLOSED)
                        if ((x + 1 < worldWidth && worldData[x + 1, y] == Blocks.DOOR_CLOSED) || (x - 1 > 0 && worldData[x - 1, y] == Blocks.DOOR_CLOSED))
                        {
                            if (y + 1 < worldHeight && worldData[x, y + 1] == Blocks.WALL_SOLID)
                                worldData[x, y + 1] = Blocks.DOOR_CLOSED;
                            else if (y - 1 > 0 && worldData[x, y - 1] == Blocks.WALL_SOLID)
                                worldData[x, y - 1] = Blocks.DOOR_CLOSED;
                        }
                        else if ((y + 1 < worldHeight && worldData[x, y + 1] == Blocks.DOOR_CLOSED) || (y - 1 > 0 && worldData[x, y - 1] == Blocks.DOOR_CLOSED))
                        {
                            if (x + 1 < worldWidth && worldData[x + 1, y] == Blocks.WALL_SOLID)
                                worldData[x + 1, y] = Blocks.DOOR_CLOSED;
                            else if (x - 1 > 0 && worldData[x - 1, y] == Blocks.WALL_SOLID)
                                worldData[x - 1, y] = Blocks.DOOR_CLOSED;
                        }
                }

            Program.setColour(ConsoleColor.Green);

            Console.WriteLine("finished connecting doors...");
        }

        public void terraform(int island_value = 1)
        {

            float frequency = FREQUENCY_INTERVAL * island_value;
            float value;
            Random r = new Random((int)DateTime.UtcNow.Ticks * (int)frequency);
            FastNoise noise = new FastNoise();

            islandValue = island_value;

            noise.SetSeed((int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()/2*r.Next(10,100));
            noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            noise.SetInterp(FastNoise.Interp.Quintic);
            noise.SetFractalOctaves(6);
            noise.SetFractalGain(0.100f);
            noise.SetFrequency(frequency);

            Program.setColour(ConsoleColor.Yellow);

            Console.WriteLine("terraforming with frequency of {0}...", frequency);

            for (int x = 0; x < worldWidth; x++)
                for (int y = 0; y < worldHeight; y++)
                {
                    value = noise.GetNoise((float)x, (float)y) * getBlockTotal() + 1;

                    if (r.Next(5, 100) > 50)
                        r.Next(0, 100);

                    if (value > 2.0f)
                        if (r.Next(0, 100) > 50)
                            value = Math.Abs((int)Math.Floor(value));
                        else
                            value = Math.Abs((int)Math.Round(value));
                    else
                        value = Math.Abs((int)Math.Floor(value));

                    if (isEmpty(x, y))
                        switch (value)
                        {
                            case 0:
                                setBlock(x, y, Blocks.DEEP_WATER);
                                break;
                            case 1:
                                setBlock(x, y, Blocks.WATER);
                                break;
                            case 2:
                                setBlock(x, y, Blocks.SAND);
                                break;
                            case 3:
                                setBlock(x, y, Blocks.GRASS);
                                break;
                            case 4:
                                setBlock(x, y, Blocks.HILLY_GRASS);
                                break;
                            case 5:
                                setBlock(x, y, Blocks.MOUNTAIN_GRASS);
                                break;
                            case 6:
                                setBlock(x, y, Blocks.DIRT);
                                break;
                            case 7:
                                setBlock(x, y, Blocks.GRAVEL);
                                break;
                            case 8:
                                setBlock(x, y, Blocks.STONE);
                                break;
                            case 9:
                            case 10:
                                setBlock(x, y, Blocks.SNOW);
                                break;
                            case 11:
                            case 12:
                            case 13:
                                setBlock(x, y, Blocks.DEEP_SNOW);
                                break;
                            default:
                                setBlock(x, y, Blocks.WALL_INVISIBLE);
                                break;
                        }
                }

            Program.setColour(ConsoleColor.Green);

            Console.WriteLine("terraforming finished...");
        }

        public void placeDoors()
        {

            Program.setColour(ConsoleColor.Yellow);

            Console.WriteLine("placing doors...");

            int doorsize = ROOM_DOOR_SIZE;

            foreach (var room in this.roomData)
            {

                if (room == null)
                    continue;

                Random r = new Random((int)DateTime.UtcNow.ToBinary());
                int direction = r.Next(6);
                int[] doorposition = new int[4];
                switch (direction)
                {
                    default:
                    case 0:
                    case 1:

                        doorposition[0] = room[0] - (int)Math.Floor((decimal)room[2] / 2);
                        doorposition[1] = room[1] - doorsize / 2;
                        doorposition[2] = doorposition[0];
                        doorposition[3] = doorposition[1] + doorsize / 2 + 2;

                        placeDoor(doorposition);
                        break;
                    case 6:
                    case 2:
                        doorposition[0] = room[0] + (int)Math.Floor((decimal)room[2] / 2) - 1;
                        doorposition[1] = room[1] - doorsize / 2;
                        doorposition[2] = doorposition[0];
                        doorposition[3] = doorposition[1] + doorsize / 2 + 2;

                        placeDoor(doorposition);
                        break;
                    case 3:
                        doorposition[0] = room[0] - doorsize / 2;
                        doorposition[1] = room[1] - (int)Math.Floor((decimal)room[3] / 2);
                        doorposition[2] = doorposition[0] + doorsize / 2 + 2;
                        doorposition[3] = doorposition[1];

                        placeDoor(doorposition);
                        break;
                    case 5:
                    case 4:
                        doorposition[0] = room[0] - doorsize / 2;
                        doorposition[1] = room[1] + (int)Math.Floor((decimal)room[3] / 2) - 1;
                        doorposition[2] = doorposition[0] + doorsize / 2 + 2;
                        doorposition[3] = doorposition[1];

                        placeDoor(doorposition);
                        break;
                }
            }


            Program.setColour(ConsoleColor.Green);

            Console.WriteLine("placing doors finished...");
        }

        public void setColour(Blocks block, ConsoleColor color)
        {

            this.blockColours[block] = color;
        }

        public void placeDoor(int[] doorposition)
        {

            placeDoor(doorposition[0], doorposition[1], doorposition[2], doorposition[3]);
        }

        public void placeDoor(int startx, int starty, int endx, int endy)
        {

            if (starty == endy)
                for (int x = 0; x < endx - startx; x++)
                    this.worldData[startx + x, starty] = Blocks.DOOR_CLOSED;
            else
                if (startx == endx)
                for (int y = 0; y < endy - starty; y++)
                    this.worldData[startx, starty + y] = Blocks.DOOR_CLOSED;
            else
                for (int x = 0; x < endx - startx; x++)
                    for (int y = 0; y < endy - starty; y++)
                        this.worldData[startx + x, starty + y] = Blocks.DOOR_CLOSED;
        }

        public void placeRoom(int startx, int starty, int width, int height)
        {

            if (startx + width > worldWidth || startx + width < 0 || startx < 0)
                throw new System.ArgumentException("room exceeds or is under the world width", "x");

            if (starty + height > worldHeight || starty + height < 0 || starty < 0)
                throw new System.ArgumentException("room exceeds or is under the world height", "y");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if (x == 0 || y == 0)
                    {

                        if (x == 0 && y == 0)
                            worldData[startx + x, starty + y] = Blocks.WALL_SOLID;
                        else
                        if (y == 0)
                        {
                            if (x == width - 1)
                                worldData[startx + x, starty + y] = Blocks.WALL_SOLID;
                            else
                                worldData[startx + x, starty + y] = Blocks.WALL_SOLID_HORIZONTAL;
                        }
                        else if (x == 0)
                            if (y == height - 1)
                                worldData[startx + x, starty + y] = Blocks.WALL_SOLID;
                            else
                                worldData[startx + x, starty + y] = Blocks.WALL_SOLID_VERTICAL;
                    }
                    else if (x == width - 1 && y == height - 1)
                        worldData[startx + x, starty + y] = Blocks.WALL_SOLID;
                    else if (x == 0 && y == height - 1)
                        worldData[startx + x, starty + y] = Blocks.WALL_SOLID;
                    else if (y == height - 1)
                        worldData[startx + x, starty + y] = Blocks.WALL_SOLID_HORIZONTAL;
                    else if (x == width - 1)
                        worldData[startx + x, starty + y] = Blocks.WALL_SOLID_VERTICAL;
                    else
                        worldData[startx + x, starty + y] = Blocks.FLOOR;
                }
            }

            saveRoomPosition(startx + (width / 2), starty + (height / 2), width, height);
            roomCount++;
        }

        public void saveRoomPosition(int roomx, int roomy, int room_width, int room_height, bool claimed = false)
        {

            int _ = 0;

            if (claimed)
                _ = 1;

            int[] position = new int[5];
            position[0] = roomx;
            position[1] = roomy;
            position[2] = room_width;
            position[3] = room_height;
            position[4] = _;

            if (spawnRoom == null)
                spawnRoom = position;

            this.roomData[roomCount] = position;
        }

        public void setBlock(int x, int y, Blocks block)
        {

            worldData[x, y] = block;
        }

        public void removeFoliage(int x, int y)
        {

            foliageData[x, y] = 0;
        }

        public ConsoleColor getColor(Blocks block)
        {

            return this.blockColours[block];
        }

        public Foliage getFoliage(int x, int y)
        {

            return ((Foliage)foliageData[x, y]);
        }

        public int[] getRandomRoomSize()
        {

            Random r = new Random((int)DateTime.UtcNow.ToBinary());
            int[] random = new int[2];

            if (ROOM_SYMMETICAL)
            {

                random[0] = (int)Math.Floor(r.Next(ROOM_MIN_SIZE, ROOM_MAX_SIZE) * 2.0);

                if (random[0] % 2 != 0)
                    random[0]++;

                random[1] = (int)Math.Floor(random[0] * 0.5);


                if (random[1] % 2 != 0)
                    random[1]++;
            }
            else
            {
#pragma warning disable CS0162 // Unreachable code detected
                random[0] = (int)Math.Floor(r.Next(ROOM_MIN_SIZE, ROOM_MAX_SIZE) * 2.0);
#pragma warning restore CS0162 // Unreachable code detected

                if (random[0] % 2 != 0)
                    random[0]++;

                random[1] = (int)Math.Floor(r.Next(ROOM_MIN_SIZE, ROOM_MAX_SIZE) * 0.5);


                if (random[1] % 2 != 0)
                    random[1]++;
            }

            return random;
        }

        public int[] getRandomRoomPosition(int attempts = 1)
        {
            Random r = new Random((int)DateTime.UtcNow.ToBinary());
            int[] random = new int[2];
            random[0] = r.Next(WORLD_BUFFER, worldWidth - WORLD_BUFFER);
            random[1] = r.Next(WORLD_BUFFER, worldHeight - WORLD_BUFFER);

            return random;
        }

        public int[] getRoom(int roomx, int roomy)
        {

            foreach (int[] room in roomData)
            {

                if (room == null)
                    continue;

                if (room[0] == roomx && room[1] == roomy)
                    return room;
            }

            return null;
        }

        public int getBlockTotal()
        {

            return Enum.GetNames(typeof(Blocks)).Length;
        }

        public bool claimRoom(Player player)
        {

            foreach (int[] room in roomData)
                if (isInRoom(player, room))
                {

                    room[4] = 1;
                    return true;
                }

            return false;
        }

        public bool isInRoomAndClaimed(Player player)
        {

            foreach (int[] room in roomData)
            {

                if (!isInRoom(player, room))
                    continue;

                if (room[4] == 1)
                    return true;
            }
                
            return false;
        }

        public bool isInRoom(Player player, int[] room)
        {


            if (player.Position[0] < (room[0]+room[2]/2) &&
               player.Position[0] + 1 > (room[0]-room[2]/2) &&
               player.Position[1] < (room[1]+room[3]/ 2) &&
               player.Position[1] + 1 > (room[1]-room[3]/2))
                return true;

            return false;
        }

        public bool roomClaimed(int key)
        {

            int[] room = roomData[key];

            return (room[4] == 1);
        }

        public bool roomClaimed(int roomx, int roomy)
        {

            int[] room = getRoom(roomx, roomy);

            return (room[4] == 1);
        }

        public bool placeRooms(int rooms)
        {

            int placed_rooms = 0;
            int total_rooms = rooms;

            if (rooms > ROOM_MAX)
                throw new ArgumentException("rooms exceed maximum permitted", "rooms");

            var later = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(ROOM_GENERATION_TIMEOUT));

            while (rooms > 0 && DateTimeOffset.UtcNow < later)
            {

                int[] roomSize = getRandomRoomSize();
                int[] roomPosition = getRandomRoomPosition();

                if (canPlace(roomPosition[0], roomPosition[1], roomSize[0], roomSize[1]))
                {
                    placeRoom(roomPosition[0], roomPosition[1], roomSize[0], roomSize[1]);
                    rooms--;
                    placed_rooms++;


                    if (Console.ForegroundColor != ConsoleColor.Red)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("placed room at " + roomPosition[0] + "/" + roomPosition[1] + " | {0} left", rooms);
                }
            }

            if (rooms != 0)
            {
                Console.WriteLine("unable to place " + (total_rooms - placed_rooms) + " rooms within " + ROOM_GENERATION_TIMEOUT + " seconds");
                return false;
            }
            else
            {

                if (Console.ForegroundColor != ConsoleColor.Green)
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("finished placing rooms...");
            }

            return true;
        }

        public bool hasFoliage(int x, int y)
        {

            if (x < 0 || y < 0)
                return false;

            return (foliageData[x, y] != 0);
        }

        public bool isEmpty(int x, int y)
        {

            return (worldData[x, y] == Blocks.WALL_INVISIBLE);
        }

        public bool isRoomCenter(int roomx, int roomy)
        {

            foreach (int[] room in roomData)
            {

                if (room == null)
                    continue;

                if (room[0] == roomx && room[1] == roomy)
                    return true;
            }

            return false;
        }

        public bool isClearPath(int startx, int starty, int endx, int endy)
        {

            if (startx < 0 || starty < 0)
                return false;

            if (startx == endx)
            {
                for (int y = 0; y < endy - starty; y++)
                    if (isSolid(startx, starty + y))
                        return false;

                return true;
            }


            for (int x = 0; x < endx - startx; x++)
                if (starty == endy)
                    if (isSolid(startx + x, starty))
                        return false;
                    else
                        for (int y = 0; y < endy - starty; y++)
                            if (isSolid(startx, starty + y))
                                return false;
            return true;
        }

        public bool isWorldEmpty()
        {
            if (worldData == null)
                return true;

            if (worldData.Length == 0)
                return true;

            if (roomCount == 0)
                return true;

            return false;
        }

        public bool isSolid(int x, int y)
        {

            if ((int)worldData[x, y] <= (int)Blocks.WALL_SOLID)
                return true;

            return false;
        }

        public bool isDoor(int x, int y)
        {

            if (worldData[x, y] == Blocks.DOOR_OPEN)
                return true;

            if (worldData[x, y] == Blocks.DOOR_CLOSED)
                return true;

            return false;
        }

        public bool isDoorOpen(int x, int y)
        {

            if (x >= worldWidth || y >= worldHeight)
                return false;

            return (worldData[x, y] == Blocks.DOOR_OPEN);
        }

        public bool isDoorClosed(int x, int y)
        {

            if (x >= worldWidth || y >= worldHeight)
                return false;

            return (worldData[x, y] == Blocks.DOOR_CLOSED);
        }

        public bool canPlace(int[] dimensions)
        {

            return canPlace(dimensions[0], dimensions[1], dimensions[2], dimensions[3]);
        }

        public bool canPlace(int startx, int starty, int width, int height, int extra_buffer = 0)
        {

            if (startx == 0 || starty == 0)
                return false;

            if (startx + width + ROOM_BUFFER + extra_buffer > worldWidth || startx + width < 0 || startx < 0)
                return false;

            if (starty + height + ROOM_BUFFER + extra_buffer > worldHeight || starty + height < 0 || starty < 0)
                return false;

            for (int x = 0; x < width + ROOM_BUFFER + extra_buffer; x++)
            {
                for (int y = 0; y < height + ROOM_BUFFER + extra_buffer; y++)
                {

                    if (starty + y > worldHeight)
                        continue;

                    if (startx + x > worldWidth)
                        continue;

                    if (isSolid(startx + x, starty + y))
                        return false;

                    if ((Blocks)worldData[startx + x, starty + y] == Blocks.FLOOR)
                        return false;

                    if ((Blocks)worldData[startx + x, starty + y] == Blocks.WATER)
                        return false;

                    if ((Blocks)worldData[startx + x, starty + y] == Blocks.SAND)
                        return false;

                    if ((Blocks)worldData[startx + x, starty + y] == Blocks.STONE)
                        return false;

                    if (hasFoliage(startx + x, starty + y))
                        return false;
                }
            }

            for (int x = width - ROOM_BUFFER - extra_buffer; x > (0 - ROOM_BUFFER - extra_buffer); x--)
            {
                for (int y = height - ROOM_BUFFER - extra_buffer; y > (0 - ROOM_BUFFER - extra_buffer); y--)
                {

                    if (starty - y < 0)
                        continue;

                    if (startx - x < 0)
                        continue;

                    if (isSolid(startx - x, starty - y))
                        return false;

                    if ((Blocks)worldData[startx - x, starty - y] == Blocks.FLOOR)
                        return false;

                    if ((Blocks)worldData[startx - x, starty - y] == Blocks.WATER)
                        return false;

                    if ((Blocks)worldData[startx - x, starty - y] == Blocks.SAND)
                        return false;

                    if ((Blocks)worldData[startx - x, starty - y] == Blocks.SNOW)
                        return true;

                    if ((Blocks)worldData[startx - x, starty - y] == Blocks.STONE)
                        return false;

                    if ((Blocks)worldData[startx - x, starty - y] == Blocks.GRAVEL)
                        return false;

                    if (hasFoliage(startx - x, starty - y))
                        return false;
                }
            }

            return true;
        }
    }
}
