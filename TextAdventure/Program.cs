using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace TextAdventure
{
    class Program
    {

        public enum States : int
        {
            INVALID,
            WORLD,
            MENU,
            OPTIONS,
            HELP,
            CREDITS,
            GAMEOVER
        }

        public const int DEFAULT_WORLD_WIDTH = 2028;
        public const int DEFAULT_WORLD_HEIGHT = 2028;
        public const int DEFAULT_FOLIAGE_COUNT = World.WORLD_MAX_FOLIAGE / 2;
        public const int DEFAULT_ROOM_COUNT = World.ROOM_MAX / 4;
        public const int DEFAULT_STRUCTURE_COUNT = 164;
        public const int DEFAULT_ISLAND_VALUE = 2;
        public const int HUD_BUFFER_ZONE = 2;
        public const bool enableScreenBuffer = false;

        public static bool developerCommands = true;
        public static int windowWidth = Console.LargestWindowWidth / 2 + (Console.LargestWindowWidth / 4);
        public static int windowHeight = Console.LargestWindowHeight / 2 + (Console.LargestWindowHeight / 3);

        protected static World world;
        protected static Player player;
        protected static bool autoScroll = true;
        protected static bool autoTurn = true;
        protected static bool skipMove = false;
        protected static bool enableColours = true;
        protected static string line = "";
        protected static int turn = 0;
        protected static List<string> feedback = new List<string>();
        protected static int[] turnChanges = new int[5];

        private static States state;
        private static ConsoleColor currentForegroundColour;
        private static ConsoleColor currentBackgroundColour;
        private static Music music;

        private static int page = 0;
        private static int pageMax = 0;
        private static int feedbackMax = windowHeight - Player.HUDH - 1;
        private static int screenY = 0;
        private static int turnMoves = 4;
        private static bool skipRead = false;
        private static bool skipClean = false;
        private static bool redraw = false;
        private static bool cleanFeedback = true;
        private static bool _running = true;
        private static string[] screen_buffer = new string[windowHeight];

        internal static States State { get => state; set => state = value; }
        public static ConsoleColor CurrentForegroundColour { get => currentForegroundColour; }
        public static ConsoleColor CurrentBackgroundColour { get => currentBackgroundColour; }
        public static Music Music { get => music; }

        static void Main(string[] args)
        {

            Console.OutputEncoding = Encoding.UTF8;
            Console.SetWindowSize(windowWidth, windowHeight);
            Console.SetBufferSize(windowWidth, windowHeight);

            setGameState(States.MENU);

            while (_running)
            {

                redrawHere:
                if (!skipClean)
                    Clear();

                //this means that the next command will be read.
                skipRead = false;
                skipClean = false;
                skipMove = false;

                if (music == null)
                {

                    music = new Music();
                    music.setOutputDevice();
                }
                else
                    if (getGameState() != States.MENU)
                    music.jukebox();

                update();

                switch (state)
                {

                    case States.WORLD:
                        player.update();
                        //pritn feedback
                        printFeedback(Player.CAMERA_WIDTH + 1, 1, Player.HUDW, page * feedbackMax, feedbackMax);
                        //print world
                        player.drawHud("turn " + turn + $" / moves remaining {turnMoves}");
                        World.printWorld(player, world, HUD_BUFFER_ZONE);
                        break;
                    case States.HELP:
                        //prints help text file
                        printFile("help.txt");

                        writeLine("[r]eturn | [h]elp | [o]ptions | [d]onate | [w]ebsite | [p]age <number> / [n]ext / [b]ack");
                        break;
                    case States.CREDITS:
                        //prints credits text file
                        printFile("credits.txt", false );

                        writeLine();
                        writeLine("[r]eturn | [h]elp | [o]ptions | [d]onate | [w]ebsite | [p]age <number> / [n]ext / [b]ack");
                        break;
                    case States.OPTIONS:

                        write("Options (options in red you cannot change)");
                        writeLine();
                        writeLine();

                        //print settings
                        writeLine("Music");
                        writeLine();

                        if (music != null)
                        {
                            if (music.isPlaying())
                                writeLine("playing track {0}", music.getCurrentTrack());

                            writeLine("volume {0}", music.getVolume());
                        }

                        writeLine();
                        writeLine("Graphics");
                        writeLine();
                        writeLine("colours {0}", enableColours);
                        setColour(ConsoleColor.Red);
                        writeLine("*screen_buffer {0}", enableScreenBuffer);
                        writeLine("*screen_w {0}", windowWidth);
                        writeLine("*screen_h {0}", windowHeight);
                        setColour(ConsoleColor.White);
                        writeLine();
                        writeLine("Gameplay");
                        writeLine();
                        writeLine("autoclear {0}", autoScroll);
                        writeLine("autoturn {0}", autoTurn);
                        writeLine("clearfeedback {0}", cleanFeedback);
                        writeLine();
                        writeLine("Advanced");
                        writeLine();
                        writeLine("developer_commands {0}", developerCommands);
                        writeLine();
                        writeLine("[r]eturn | [h]elp | [c]redits | change <setting_name> <value if needed>");
                        break;
                    case States.MENU:

                        if (world != null && !world.isWorldEmpty())
                        {

                            writeLine("[r]esume game");
                            writeLine("[d]destroy game");
                            writeLine("[s]ave game");
                            writeLine("[l]oad");
                            writeLine("[c]redits");
                            writeLine("[o]ptions");
                            writeLine("[h]elp <reading suggested>");
                            writeLine("[e]xit");
                        }
                        else
                            printFile("menu.txt", false);

                        break;
                    default:
                        setColour(ConsoleColor.Red);
                        writeLine("Invalid gamestate: {0}", state);
                        break;
                }

                processCommand(line);

                if(redraw)
                {
                    skipClean = false;
                    redraw = false;
                    goto redrawHere;
                }    

                if (player != null)
                    

                setColour(ConsoleColor.White);

                if (enableScreenBuffer)
#pragma warning disable CS0162 // Unreachable code detected
                    for (int i = 0; i < screen_buffer.Length; i++)
#pragma warning restore CS0162 // Unreachable code detected
                        Console.WriteLine("{1}", i, screen_buffer[i]);

                if (skipRead == false)
                {

                    write("> ");
                    retry:

                    line = Console.ReadLine();


                    if (line == "" || line.Length == 0)
                        goto retry;
                }
            }

            if (music != null)
                music.dispose();
        }

        public static void update()
        {

            windowWidth = Console.LargestWindowWidth / 2 + (Console.LargestWindowWidth / 4);
            windowHeight = Console.LargestWindowHeight / 2 + (Console.LargestWindowHeight / 3);

            if(Console.WindowHeight!=windowHeight||Console.WindowWidth!=windowWidth)
            {
                Console.SetWindowSize(windowWidth, windowHeight);
                Console.SetBufferSize(windowWidth, windowHeight);
            }
        }

        public static void resetChanges()
        {

            turnChanges = new int[5];
        }

        public static void displayChanges()
        {

            int counter = 0;
            foreach(int change in turnChanges)
            {

                string sign = "+";
                if (change < 0)
                    sign = "-";
                else
                if (change == 0)
                    sign = "*";
               

                switch(counter)
                {
                    case 0:
                        addFeedback("{0} S {1}", sign, change);
                        break;   
                    case 1:
                        addFeedback("{0} F {1}", sign, change);
                        break;   
                    case 2:
                        addFeedback("{0} H {1}", sign, change);
                        break;
                    case 3:
                        addFeedback("{0} xp {1}", sign, change);
                        break;
                    case 4:
                        addFeedback("{0} mana {1}", sign, change);
                        break;
                }

                counter++;         
            }
        }

        public static void addChange(int[] changes)
        {


            turnChanges[0] += changes[0];

            if(changes.Length > 1 )
                turnChanges[1] += changes[1];

            if (changes.Length > 2)
                turnChanges[2] += changes[2];

            if (changes.Length > 3)
                turnChanges[3] += changes[3];

            if (changes.Length > 4)
                turnChanges[4] += changes[4];
        }

        public static void addChange(string type, int amount)
        {

            switch(type)
            {
                case "stanima":
                    turnChanges[0] += amount;
                    break;
                case "hunger":
                    turnChanges[1] += amount;
                    break;
                case "health":
                    turnChanges[2] += amount;
                    break;
                case "xp":
                    turnChanges[3] += amount;
                    break;     
                case "mana":
                    turnChanges[4] += amount;
                    break;
            }
        }

        public static void addFeedback(string line, params object[] objects)
        {

            feedback.Add(String.Format(line, objects));
        }

        public static void printFeedback(int startx, int starty = 1, int width = 64, int start=0, int count=12)
        {

            int currentx = Console.CursorLeft;
            int currenty = Console.CursorTop;
            int y = 0;
            int x = 0;

            void header()
            {

                y = 0;
                x = 0;
 
                setColour(ConsoleColor.White);
                Console.SetCursorPosition(startx, starty + y);

                string header = "#[feedback (" + page + "/" + (int)Math.Floor((decimal)feedback.Count / feedbackMax) + ")]";
                Program.write(header);

                for (int i = 0; i < width - header.Length; i++)
                    Program.write("-");

                y++;
                y++;
            }

            Console.SetCursorPosition(startx, starty + y);

            bool function(int i1)
            {

                i1 = start + i1;

                if (i1 >= feedback.Count)
                    return false;


                string line = feedback[i1];
                setColour(ConsoleColor.White);

                Console.SetCursorPosition(startx, starty + y);

                if (line.StartsWith('*'))
                    setColour(ConsoleColor.Blue);

                if (line.StartsWith('+'))
                    setColour(ConsoleColor.Green);

                if (line.StartsWith('?'))
                    setColour(ConsoleColor.Blue);

                if (line.StartsWith('-'))
                    setColour(ConsoleColor.Red);

                if (y > windowHeight)
                    return false;

                if (line == "" || line == null)
                    return true;

                for (int i = 0; i < width; i++)
                {
                    string sline;
                    if (line.Length - 1 < i)
                        sline = " ";
                    else
                        sline = line[i].ToString();

                    if (x < width)
                        write(sline);
                    else
                    {

                        if (y < windowHeight)
                        {

                            x = 0;
                            Console.SetCursorPosition(startx, starty + y);
                            write(sline);
                            continue;
                        }
                        else
                            y = 0;
                    }

                    if (y > Player.HUDY - 5||y > count)
                    {
                        if (autoScroll)
                        {
                            page++;
                            Console.Clear();
                            header();
                            Console.SetCursorPosition(startx, starty + y);
                            write(sline);

                        }
                        else
                        {
                            Console.SetCursorPosition(startx, starty + y);
                            write("scroll to next page...");
                            return false;
                        }
                    
                                         
                    }
                    x++;
                }

                y++;
                x = 0;

                return true;
            }

            header();

            for (int i1 = 0; i1 < feedback.Count; i1++)
                if (!function(i1))
                    break;

            Console.SetCursorPosition(currentx, currenty);
        }

        public static void clearFeedback()
        {

            page = 0;
            feedback = new List<string>();
        }

        public static void Clear()
        {

            Console.Clear();
            screen_buffer = new string[windowHeight];
            screenY = 0;
        }

        public static void writeLine(string line = "", params object[] data)
        {

            if (!enableScreenBuffer)
                Console.WriteLine(line, data);
            else
            {

#pragma warning disable CS0162 // Unreachable code detected
                if (screenY < windowHeight - 1)
#pragma warning restore CS0162 // Unreachable code detected
                {
                    screenY++;
                    screen_buffer[screenY] = String.Format(line, data);
                    //Advance screen line
                }
            }
        }

        public static void setColour(ConsoleColor color)
        {

            if (!enableColours)
                return;

            if (currentForegroundColour != color)
            {

                Console.ForegroundColor = color;
                currentForegroundColour = color;
            }
        }

        public static void setBackgroundColour(ConsoleColor color)
        {

            if (currentBackgroundColour != color)
            {

                Console.ForegroundColor = color;
                currentBackgroundColour = color;
            }
        }

        public static void write(string line = "", params object[] data)
        {

            if (enableScreenBuffer)
#pragma warning disable CS0162 // Unreachable code detected
                screen_buffer[screenY] = screen_buffer[screenY] + line;
#pragma warning restore CS0162 // Unreachable code detected
            else
                Console.Write(line, data);
        }

        public static States getGameState()
        {

            return state;
        }

        public static bool newgame(ref Player player, ref World world, string[] arguments)
        {

            Random r = new Random((int)DateTime.UtcNow.Year);

            int rooms = DEFAULT_ROOM_COUNT;
            int foliage = DEFAULT_FOLIAGE_COUNT;
            int width = DEFAULT_WORLD_WIDTH;
            int height = DEFAULT_WORLD_HEIGHT;
            int island_value = r.Next(DEFAULT_ISLAND_VALUE, DEFAULT_ISLAND_VALUE * 100);
            int structure_count = DEFAULT_STRUCTURE_COUNT;

            turn = 0;

            if (arguments.Length != 0)
            {

                if (arguments.Length > 0)
                    if (arguments[0] != "")
                        if (int.TryParse(arguments[0], out int _parse))
                        {
                            if (_parse != 0)
                                rooms = _parse;
                        }

                if (arguments.Length > 1)
                    if (arguments[1] != "")
                        if (int.TryParse(arguments[1], out int _parse))
                        {
                            if (_parse != 0)
                                foliage = _parse;
                        }

                if (arguments.Length > 2)
                    if (arguments[2] != "")
                        if (int.TryParse(arguments[2], out int _parse))
                        {
                            if (_parse != 0)
                                width = _parse;
                        }

                if (arguments.Length > 3)
                    if (arguments[3] != "")
                        if (int.TryParse(arguments[3], out int _parse))
                        {
                            if (_parse != 0)
                                height = _parse;
                        }

                if (arguments.Length > 4)
                    if (arguments[4] != "")
                        if (int.TryParse(arguments[4], out int _parse))
                        {
                            if (_parse != 0)
                                island_value = _parse;
                        }

                if (arguments.Length > 5)
                    if (arguments[5] != "")
                        if (int.TryParse(arguments[5], out int _parse))
                        {
                            if (_parse != 0)
                                structure_count = _parse;
                        }
            }

            if (width < 0 + World.WORLD_BUFFER || height < 0 + World.WORLD_BUFFER)
            {
                addFeedback("world width/height is too low. press enter to try again...");
                return false;
            }

            if (world == null)
                world = createWorld(width, height);
            else
                world.destroyWorld();

            if (rooms > World.ROOM_MAX)
            {
                addFeedback("to many rooms, must be below {0}", World.ROOM_MAX);
                return false;
            }

            world.generateWorld(rooms, foliage, island_value, structure_count);

            if (player == null)
            {
                tryagain:
                Console.Clear();
                Console.WriteLine("please enter your character name");
                string name = Console.ReadLine();

                if (name == "")
                {
                    Console.WriteLine("please enter something. press enter to try again..", World.ROOM_MAX);
                    Console.ReadLine();
                    goto tryagain;
                }

                player = createPlayer(name);
                player.reset();
                resetChanges(); //resets the changes done by spawning a new player
            }

            return true;
        }

        private static void printFile(string filename, bool show_pagenumbers = true)
        {

            int counter = 0;
            int start = (((Player.CAMERA_HEIGHT) - Program.HUD_BUFFER_ZONE) * page);
            string[] help = System.IO.File.ReadAllLines(filename);

            pageMax = (int)Math.Floor((decimal)(help.Length / ((Player.CAMERA_HEIGHT))));


            if (show_pagenumbers)
            {

                setColour(ConsoleColor.White);
                writeLine("{0}: page {1}/{2}", filename, page, pageMax);
                counter++;
            }

            for (int l = 0; l < help.Length; l++)
            {

                setColour(ConsoleColor.White);

                if (l + start >= help.Length)
                    break;

                if (l + start < 0)
                    continue;

                string trimed_line = help[l + start].Trim();

                if (trimed_line.StartsWith('-'))
                    setColour(ConsoleColor.Red);

                if (trimed_line.StartsWith("#"))
                    setColour(ConsoleColor.DarkBlue);

                if (trimed_line.StartsWith("|"))
                    setColour(ConsoleColor.Green);

                if (trimed_line.StartsWith('~'))
                    setColour(ConsoleColor.Magenta);

                if (trimed_line.StartsWith('?'))
                    setColour(ConsoleColor.Magenta);

                if (trimed_line.StartsWith('*'))
                    setColour(ConsoleColor.Blue);

                if (trimed_line.StartsWith('>'))
                    setColour(ConsoleColor.DarkYellow);

                if (trimed_line.StartsWith('+'))
                    setColour(ConsoleColor.Green);

                if (counter >= (Player.CAMERA_HEIGHT - Program.HUD_BUFFER_ZONE))
                {
                    write("    ");
                    writeLine(help[l + start]);
                    break;
                }

                write("    ");
                writeLine(help[l + start]);
                counter++;
            }

            writeLine(" ");
            setColour(ConsoleColor.White);
        }

        public static void setGameState(States gamestate)
        {

            page = 0;
            pageMax = 0;
            state = gamestate;
        }

        public static void newTurn(bool displayHeaders=true)
        {

            if (getGameState() == States.WORLD)
            {
                turnMoves = player.Moves;
                player.processTurn(world.isInRoomAndClaimed(player));


                if (cleanFeedback)
                    clearFeedback();

                if (displayHeaders)
                {
                    addFeedback("----------------------------------");
                    addFeedback(" end of turn {0} ", turn);
                }

                ++turn;

                displayChanges();

                if (displayHeaders)
                {
                    addFeedback(" start of turn {0} ", turn);
                    addFeedback("----------------------------------");
                    addFeedback("* you have {0} moves", turnMoves);
                }
                resetChanges();
            }
        }

        private static void setSpawn(ref Player player, ref World world)
        {

            int[] spawn_room = World.SpawnRoom;
            player.setPosition(spawn_room[0], spawn_room[1]);
        }

        private static bool movePlayer(ref Player player, string direction, int amount = 1)
        {

            if (turnMoves == 0)
            {

                if(!autoTurn)
                    addFeedback("* you have no moves left type 'newturn'");

                skipMove = true;
                return true;
            }

            if (amount > Player.MAX_MOVE_DISTANCE)
            {

                int difference = amount - Player.MAX_MOVE_DISTANCE;
                amount = amount - difference;

                addFeedback("too far for one turn. you manage {0} spaces.", amount);
            }

            if (player.willExhaustPlayer(amount))
            {
                addFeedback("this will make you too weak to move");
                return false;
            }

            if (player.Stanima < 10)
            {
                addFeedback("you are too weak to move");
                return false;
            }

            if (player.Hunger < 10)
            {
                addFeedback("you are too hungry to move");
                return false;
            }

            int _ = 0;

            switch (direction)
            {

                case "left":
                    while (_ < amount && player.Position[0] - (_ + 1) > 0 && !world.isSolid(player.Position[0] - (_ + 1), player.Position[1]))
                        _++;

                    if (_ != 0 && (player.Position[0] - _) > 0)
                    {

                        player.setPosition(player.Position[0] - _, player.Position[1]);
                        player.decreaseHunger(_);
                        player.decreaseStanima(_);

                        if (_ == amount)
                            addFeedback("you moved {0} spaces.", _);
                        else
                            addFeedback("you moved {0} spaces, hit wall.", _);
                    }
                    else
                    {
                        addFeedback("you move into a wall.");
                        skipMove = true;
                    }

                    break;
                case "right":
                    while (_ < amount && player.Position[0] + (_ + 1) < world.WorldWidth && !world.isSolid(player.Position[0] + (_ + 1), player.Position[1]))
                        _++;

                    if (_ != 0 && (player.Position[0] + _) < world.WorldWidth)
                    {

                        player.setPosition(player.Position[0] + _, player.Position[1]);
                        player.decreaseHunger(_);
                        player.decreaseStanima(_);

                        if (_ == amount)
                            addFeedback("you moved {0} spaces.", _);
                        else
                            addFeedback("you moved {0} spaces.", _);
                    }
                    else
                    {
                        addFeedback("you move into a wall.");
                        skipMove = true;
                    }

                    break;
                case "up":
                    while (_ < amount && player.Position[1] - (_ + 1) > 0 && !world.isSolid(player.Position[0], player.Position[1] - (_ + 1)))
                        _++;

                    if (_ != 0 && (player.Position[1] - _) > 0)
                    {

                        player.setPosition(player.Position[0], player.Position[1] - _);
                        player.decreaseHunger(_);
                        player.decreaseStanima(_);

                        if (_ == amount)
                            addFeedback("you moved {0} spaces.", _);
                        else
                            addFeedback("you moved {0} spaces, hit wall.", _);
                    }
                    else
                    {
                        addFeedback("you move into a wall.");
                        skipMove = true;
                    }

                    break;
                case "down":
                    while (_ < amount && player.Position[1] + (_ + 1) < world.WorldHeight && !world.isSolid(player.Position[0], player.Position[1] + (_ + 1)))
                        _++;

                    if (_ != 0 && (player.Position[1] + _) < world.WorldHeight)
                    {

                        player.setPosition(player.Position[0], player.Position[1] + _);
                        player.decreaseHunger(_);
                        player.decreaseStanima(_);

                        if (_ == amount)
                            addFeedback("you moved {0} spaces.", _);
                        else
                            addFeedback("you moved {0} spaces, hit wall.", _);
                    }
                    else
                    {
                        addFeedback("invalid move. you lost stanima and hunger trying.");
                        skipMove = true;
                    }

                    break;
            }

            if (_ != 0)
            {
                double[] loses = player.getLoses(_);
                addFeedback("- S {0}", loses[0]);
                addFeedback("- F {0}", loses[1]);
            }

            return true;
        }

        private static World.Foliage query(ref Player player, ref World world, int scope = 4)
        {
            for (int x = 0 - scope; x < scope; x++)
                for (int y = 0 - scope; y < scope; y++)
                    if (x + player.Position[0] < world.WorldWidth && y + player.Position[1] < world.WorldHeight)
                        if (world.hasFoliage(player.Position[0] + x, player.Position[1] + y))
                            return world.getFoliage(player.Position[0] + x, player.Position[1] + y);

            return World.Foliage.NULL;
        }

        private static bool openDoor(ref Player player, int scope = 5)
        {

            int x = player.Position[0] - scope / 2;
            int y = player.Position[1] - scope / 2;

            x = Math.Abs(x);
            y = Math.Abs(y);

            for (int searchx = 0; searchx < scope; searchx++)
                for (int searchy = 0; searchy < scope; searchy++)
                {

                    int _x = x - searchx, _y = y - searchy;

                    if (_x > 0 && _x < world.WorldWidth && _y > 0 && _y < world.WorldHeight)
                        if (world.isDoor(_x, _y) && world.isDoorClosed(_x, _y))
                        {

                            while (world.isDoorClosed(_x, _y))
                            {
                                world.setBlock(_x, _y, World.Blocks.DOOR_OPEN);

                                if (world.isDoorClosed(_x + 1, _y))
                                    _x++;
                                else if (world.isDoorClosed(_x, _y + 1))
                                    _y++;
                                else if (world.isDoorClosed(_x - 1, _y))
                                    _x--;
                                else if (world.isDoorClosed(_x, _y - 1))
                                    _y--;
                                else if (world.isDoorClosed(_x + 1, _y - 1))
                                {
                                    _x++;
                                    _y--;
                                }
                                else if (world.isDoorClosed(_x - 1, _y + 1))
                                {
                                    _x--;
                                    _y++;
                                }
                            }

                            return true;
                        }
                        else
                        {
                            _x = x + searchx;
                            _y = y + searchy;

                            if (_x > 0 && _x < world.WorldWidth && _y > 0 && _y < world.WorldHeight)
                                if (world.isDoor(_x, _y) && world.isDoorClosed(_x, _y))
                                {


                                    while (world.isDoorClosed(_x, _y))
                                    {
                                        world.setBlock(_x, _y, World.Blocks.DOOR_OPEN);

                                        if (world.isDoorClosed(_x + 1, _y))
                                            _x++;
                                        else if (world.isDoorClosed(_x, _y + 1))
                                            _y++;
                                        else if (world.isDoorClosed(_x - 1, _y))
                                            _x--;
                                        else if (world.isDoorClosed(_x, _y - 1))
                                            _y--;
                                        else if (world.isDoorClosed(_x + 1, _y - 1))
                                        {
                                            _x++;
                                            _y--;
                                        }
                                        else if (world.isDoorClosed(_x - 1, _y + 1))
                                        {
                                            _x--;
                                            _y++;
                                        }
                                    }

                                    return true;
                                }
                        }
                }

            return false;
        }

        /**
         * Command Processing
         **/
        private static void processCommand(string command)
        {

            if (command == "")
                return;

            //lower
            command = command.ToLower();
            string[] arguments = command.Split(" ").ToArray();

            if (arguments.Length != 0)
            {
                command = arguments[0];
                arguments = arguments.Skip(1).ToArray();
            }

            switch (state)
            {

                case States.WORLD:
                    processWorldCommand(command, arguments);
                    break;
                case States.OPTIONS:
                    processOptionsCommand(command, arguments);
                    break;
                case States.MENU:
                    processMenuCommand(command, arguments);
                    break;
                case States.HELP:
                    processHelpCommand(command, arguments);
                    break;
                case States.CREDITS:
                    processCreditsCommand(command, arguments);
                    break;
                case States.GAMEOVER:
                    //processGameoverCommand(command, arguments);
                    break;
            }

            if (developerCommands)
                processDevCommand(command, arguments);

            line = "";
        }

        /**
         * World Command Processing
         **/
        private static void processWorldCommand(string command, string[] arguments)
        {
            int _ = 1;
            string file_name = "world";

            if (turnMoves == 0 && !autoTurn)
                addFeedback("- you have zero moves left");

            switch (command)
            {
                default:
                    addFeedback("invalid world command '{0}'", command);
                    skipRead = true;
                    skipMove = true;
                    break;
                case "recepie":
                case "blueprint":

                    if (player.Crafting.craftingRecepieExists(arguments[0].ToLower()))
                    {
                        Crafting.CraftingRecepies recepie = player.Crafting.getCraftingRecepie(arguments[0].ToLower());
                        Dictionary<Player.Items, int> craftingRecepie = player.Crafting.getRecepie(recepie);

                        addFeedback("to craft a {0}", recepie);

                        foreach (KeyValuePair<Player.Items, int> keyValuePair in craftingRecepie)
                            addFeedback("* {0} x{1}", keyValuePair.Key, keyValuePair.Value);
                    }

                    skipRead = true;
                    skipMove = true;

                    break;
                case "blueprints":
                case "recepies":
                    var names = Enum.GetNames(typeof(Crafting.CraftingRecepies));

                    foreach (string name in names)
                        addFeedback(name);
                    
                    skipRead = true;
                    skipMove = true;
                    break;
                case "make":
                case "craft":
                    if (arguments.Length == 0 || arguments[0] == "")
                        addFeedback("you must enter the recepie name"); 
                    else
                    {
                        _ = 1;
                        if(arguments.Length>1)
                            if(arguments[1]!="")
                                if (int.TryParse(arguments[1], out int parse))
                                {
                                    if (parse != 0)
                                        _ = Math.Abs(parse);
                                }

                        if(_>4)
                            addFeedback("you can only craft a maximum of 4 items");
                        else
                        {

                            for (int i = 0; i < _; i++)
                            {
                                if (player.Crafting.craftingRecepieExists(arguments[0].ToLower()))
                                {

                                    Crafting.CraftingRecepies recepie = player.Crafting.getCraftingRecepie(arguments[0].ToLower());

                                    if (player.Crafting.canCraft(recepie, ref player))
                                    {
                                        addFeedback("crafting {0}", recepie);
                                        player.Crafting.craft(recepie, ref player);
                                        addFeedback("+ {0}", player.Crafting.getReward(recepie));
                                        skipMove = true;
                                    }
                                    else
                                        addFeedback("you cannot craft that!");

                                }
                                else
                                    addFeedback("item does not exist {0}", arguments[0].ToLower());
                            }
                        }
                    }
                    skipMove = true;
                    skipRead = true;
                    break;
                case "conqure":
                case "capture":
                case "claim":
                    if(world.claimRoom(player)==false)
                    {
                        addFeedback("- you failed to claim a room");
                        skipMove = true;
                    }
                    else
                    {

                        addFeedback("you have successfully claimed a room!");
                        player.addXP(200);
                        player.addItem(Player.Items.KEY);
                        addFeedback("+ xp 200");
                        addFeedback("+ key 1");
                    }
                    skipRead = true;
                    break;
                case "autoscroll":
                    autoScroll = !autoScroll;

                    addFeedback("autoscroll {0}", autoScroll);
                    skipRead = true;
                    skipMove = true;
                    break;
                case "-":
                case "past":
                    if (autoScroll)
                        autoScroll = false;

                    if (page > 0)
                        page--;
                    skipRead = true;
                    skipMove = true;
                    break;
                case "+":
                case "present":
                    if (autoScroll)
                        autoScroll = false;

                    if (page < (int)Math.Floor((decimal)feedback.Count / feedbackMax))
                        page++;

                    skipRead = true;
                    skipMove = true;
                    break;
                case "=":
                case "++":
                case "latest":
                    if (autoScroll)
                        autoScroll = false;

                    page = (int)Math.Floor((decimal)feedback.Count / feedbackMax);
                    skipRead = true;
                    skipMove = true;
                    break;
                case "--":
                case "oldest":
                    if (autoScroll)
                        autoScroll = false;

                    page = 0;
                    skipRead = true;
                    skipMove = true;
                    break;
                case "cls":
                case "clear":
                    clearFeedback();
                    page = 0;
                    skipRead = true;
                    skipMove = true;
                    break;
                case "q":
                case "turn":
                case "nextturn":
                case "newturn":
                case "endturn":

                    if (turnMoves != 0)
                        addFeedback("you have not finished your turn yet and still have {0} moves");
                    else
                        newTurn();

                    skipRead = true;
                    skipMove = true;
                    break;
                case "save":
                    if (arguments.Length != 0 && arguments[0] != "")
                        file_name = arguments[0];

                    addFeedback("saving world {0}", file_name);
                    //World.saveWorld(ref world, file_name);
                    skipRead = true;
                    skipMove = true;
                    break;
                case "load":
                    if (arguments.Length != 0 && arguments[0] != "")
                        break;

                    addFeedback("loading world {0}", file_name);
                    //world = World.loadWorld(file_name);
                    skipRead = true;
                    skipMove = true;
                    break;
                case "inv":
                case "inventory":
                    player.printInventory();

                    skipMove = true;
                    skipRead = true;
                    break;
                case "use":
                    if (arguments.Length == 0 || arguments[0] == "")
                    {
                        addFeedback("you must enter the items name");
                    }
                    else
                    {
                        if (player.itemExists(arguments[0].ToLower()))
                        {

                            Player.Items item = player.getItemFromName(arguments[0].ToLower());
                            if (player.hasItem(item))
                            {
                                addFeedback("using item {0} ({1} left)", item, player.getItemQuantity(item) - 1);

                                if (player.useItem(item))
                                    player.removeItem(item);
                            }
                            else
                                addFeedback("you don't even have any {0} anymore", item);
                        }
                    }

                    
                    skipMove = true;
                    skipRead = true;
                    break;
                case "track":
                case "changemusic":
                case "music":
                    _ = 0;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = Math.Abs(parse);
                        }

                    if (_ < Enum.GetValues(typeof(Music.Tracks)).Length)
                    {

                        if (music.isPlaying())
                            music.fadeOut();

                        music.changeTrack((Music.Tracks)_);
                        music.playTrack();
                    }

                    skipMove = true;
                    skipRead = true;
                    break;
                case "z":
                case "zoom":
                    _ = 0;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = Math.Abs(parse);
                        }

                    if (_ < 4)
                        Player.CAMERA_ZOOM_FACTOR = _;

                    skipMove = true;
                    skipRead = true;
                    break;
                case "last":
                case "last_position":
                case "back":
                    if (turnMoves == 0)
                    {
                        addFeedback("you are out of moves");
                        skipMove = true;
                    }
                    else
                    if (player.hasLastPosition())
                    {
                        int[] last_position = Player.LastPosition;
                        if (last_position[1] == player.Position[1])
                            if (player.Position[0] > last_position[0])
                                _ = player.Position[0] - last_position[0];
                            else
                                _ = last_position[0] - player.Position[0];
                        else
                             if (player.Position[1] > last_position[1])
                            _ = player.Position[1] - last_position[1];
                        else
                            _ = last_position[1] - player.Position[0];

                        player.decreaseHunger(_);
                        player.decreaseStanima(_);
                        player.setPosition(last_position[0], last_position[1]);

                        double[] _loses = player.getLoses(_);

                        addFeedback("you moved back from whence you came");
                        addFeedback("- S {0}", _loses[0]);
                        addFeedback("- F {0}", _loses[1]);
                    }

                    skipRead = true;
                    break;
                case "plan":
                    _ = 1;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = Math.Abs(parse);
                        }

                    double[] loses = player.getLoses(_);

                    addFeedback("you will lose by moving {0} spaces", _);
                    addFeedback("- S {0}", loses[0]);
                    addFeedback("- F {0}", loses[1]);
                    skipMove = true;
                    skipRead = true;
                    break;
                case "i":
                case "info":
                    _ = Player.MAX_HARVEST_DISTANCE;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    World.Foliage foliage = query(ref player, ref world, _);

                    if (foliage == World.Foliage.NULL)
                    {
                        addFeedback("you are not around any foliage");
                    }
                    else
                    {

                        addFeedback("this is a {0}", Enum.GetName(typeof(World.Foliage), foliage));
                        addFeedback("its texture is {0}", World.foliageTextures[foliage]);
                        addFeedback("its colour is {0}", World.foliageColours[foliage]);
                    }
                    skipMove = true;
                    skipRead = true;
                    break;
                case "harvest":
                case "collect":
                case "scavange":
                case "pick":
                case "p":
                    _ = Player.MAX_HARVEST_DISTANCE;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (turnMoves == 0)
                    {
                        addFeedback("you have no moves left", Player.MAX_HARVEST_DISTANCE);
                        skipMove = true;
                        skipRead = true;
                        break;
                    }

                    if (_ > Player.MAX_HARVEST_DISTANCE)
                    {
                        addFeedback("you cannot harvest as far as that. you may only harvest a maximum of {0} spaces.", Player.MAX_HARVEST_DISTANCE);
                        skipMove = true;
                        skipRead = true;
                        break;
                    }

                    double[] pick_loses = player.getLoses(2);
                    double[] pick_loses_two = player.getLoses(20);

                    if (world.newInteraction(ref player, _))
                    {
                        player.decreaseHunger(2);
                        player.decreaseStanima(20);
                        addFeedback("success!");
                        addFeedback("- S {0}", pick_loses[0]);
                        addFeedback("- F {0}", pick_loses_two[1]);

                    }
                    else
                    {
                        player.decreaseHunger(2);
                        player.decreaseStanima(20);
                        addFeedback("failure!");
                        addFeedback("- S {0}", pick_loses[0]);
                        addFeedback("- F {0}", pick_loses_two[1]);
                    }

                    skipRead = true;
                    break;
                case "h":
                case "help":
                    setGameState(States.HELP);
                    skipMove = true;
                    skipRead = true;
                    break;
                case "o":
                case "options":
                    setGameState(States.OPTIONS);
                    skipMove = true;
                    skipRead = true;
                    break;
                case "m":
                case "menu":
                    setGameState(States.MENU);
                    skipMove = true;
                    skipRead = true;
                    break;
                case "idle":
                case "wait":
                    _ = 1;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (_ <= 10)
                    {

                        addFeedback("you decide to wait {0} turns.", _);
                        for (int i = 0; i < _; i++)
                        {

                            newTurn(false);
                            addFeedback("- turn {0}", turn);
                        }
                          
                    }

                    skipRead = true;
                    skipMove = true;
                    break;
                case "unlock":
                    if (player.canOpenDoor())
                        if (openDoor(ref player))
                        {
                            addFeedback("used a key to unlock a door.");
                            addFeedback("+ xp 60");
                            player.addXP(60);
                            
                            player.removeItem(Player.Items.KEY);
                        }
                        else
                        {
                            addFeedback("unable to use a key on a door.");
                        }
                    else
                    {
                        addFeedback("no keys.");
                    }

                    skipRead = true;
                    break;
                case "a":
                case "east":
                case "left":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (!movePlayer(ref player, "left", _))
                        skipMove = true;

                    skipRead = true;
                    break;
                case "d":
                case "west":
                case "right":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (!movePlayer(ref player, "right", _))
                        skipMove = true;

                    skipRead = true;
                    break;
                case "w":
                case "north":
                case "up":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (!movePlayer(ref player, "up", _))
                        skipMove = true;

                    skipRead = true;
                    break;
                case "s":
                case "south":
                case "down":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (!movePlayer(ref player, "down", _))
                        skipMove = true;

                    skipRead = true;
                    break;
            }

            if (!skipMove)
                if (turnMoves > 0)
                {

                    addFeedback($"* {--turnMoves} moves left");

                    if (turnMoves == 0)
                    {

                        if(!autoTurn)
                            addFeedback("- you have zero moves left type 'newturn'");

                        if (autoTurn)
                            newTurn();
                    }
                }
        }

        private static void processOptionsCommand(string command, string[] arguments)
        {

            int _ = 0;
            skipMove = true;
            switch (command)
            {
                case "change":
                    if (arguments.Length > 1)
                    {
                        if (int.TryParse(arguments[1], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    }

                    switch (arguments[0])
                    {
                        case "fastrender":
                        case "colors":
                        case "colours":
                            enableColours = !enableColours;
                            break;
                        case "volume":
                            if (_ < 10)
                            {

                                float value = (float)(_ / 10.0f);
                                music.setVolume(value);
                            }

                            break;
                        case "developer_commands":
                            developerCommands = !developerCommands;
                            break;
                        case "autoscroll":
                            autoScroll = !autoScroll;
                            break;
                        case "autoturn":
                            autoTurn = !autoTurn;
                            break;
                        case "clearfeedback":
                            cleanFeedback = !cleanFeedback;
                            break;
                    }
                    skipRead = true;
                    break;
                case "r":
                case "return":
                    if (world != null && !world.isWorldEmpty())
                        setGameState(States.WORLD);
                    else
                        setGameState(States.MENU);
                    skipRead = true;
                    break;
                case "c":
                case "credits":
                    setGameState(States.CREDITS);
                    skipRead = true;
                    break;
                case "h":
                case "help":
                    setGameState(States.HELP);
                    skipRead = true;
                    break;
            }
        }

        private static void processHelpCommand(string command, string[] arguments)
        {

            switch (command)
            {
                case "r":
                case "return":
                    if (world != null && !world.isWorldEmpty())
                        setGameState(States.WORLD);
                    else
                        setGameState(States.MENU);
                    skipRead = true;
                    break;
                case "o":
                case "options":
                    setGameState(States.OPTIONS);
                    skipRead = true;
                    break;
                case "c":
                case "credits":
                    setGameState(States.CREDITS);
                    skipRead = true;
                    break;
            }

            processPageCommand(command, arguments);
        }

        private static void processPageCommand(string command, string[] arguments)
        {
            int _ = 1;
            switch (command)
            {
                case "p":
                case "page":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    if (_ < pageMax)
                        page = _;
                    skipRead = true;
                    break;
                case "n":
                case "forward":
                case "next":
                    if (page < pageMax)
                        page++;
                    skipRead = true;
                    break;
                case "b":
                case "back":
                    if (page > 0)
                        page--;
                    skipRead = true;
                    break;
                case "s":
                case "start":
                    page = 0;
                    skipRead = true;
                    break;
            }
        }

        private static void processCreditsCommand(string command, string[] arguments)
        {

            switch (command)
            {
                case "r":
                case "return":
                    if (world != null && !world.isWorldEmpty())
                        setGameState(States.WORLD);
                    else
                        setGameState(States.MENU);
                    skipRead = true;
                    break;
                case "o":
                case "options":
                    setGameState(States.OPTIONS);
                    skipRead = true;
                    break;
                case "h":
                case "help":
                    setGameState(States.HELP);
                    skipRead = true;
                    break;
                case "w":
                case "website":
                    //
                    break;
                case "d":
                case "donate":
                    //
                    break;
            }

            processPageCommand(command, arguments);
        }

        /**
         * Menu Comamnd Processing
         **/
        private static void processMenuCommand(string command, string[] arguments)
        {

            switch (command)
            {
                case "new":
                case "start:":
                case "newgame":
                case "n":

                    Console.Clear();
                    if (newgame(ref player, ref world, arguments))
                    {
                        setGameState(States.WORLD);
                        setSpawn(ref player, ref world);
                    }
                    skipRead = true;
                    break;
                case "o":
                case "options":
                    setGameState(States.OPTIONS);
                    skipRead = true;
                    break;
                case "c":
                case "credits":
                    setGameState(States.CREDITS);
                    skipRead = true;
                    break;
                case "h":
                case "help":
                    setGameState(States.HELP);
                    skipRead = true;
                    break;
                case "load":
                    break;
                case "r":
                case "resume":
                    if (world == null || world.isWorldEmpty())
                        break;
                    else
                        setGameState(States.WORLD);
                    skipRead = true;
                    break;
                case "d":
                case "destroy":
                    if (world == null || world.isWorldEmpty())
                        break;
                    else
                        world.destroyWorld();

                    skipRead = true;
                    break;
                case "e":
                case "exit":
                case "close":
                    _running = false;
                    skipRead = true;
                    break;
            }
        }

        private static void processDevCommand(string command, string[] arguments)
        {

            if (world == null)
                return;

            if (player == null)
                return;

            int _ = 1;
            var counter = 0;
            switch (command)
            {
                case "tracks":
                    var names = Enum.GetNames(typeof(Music.Tracks));
                    counter = 0;

                    foreach (string name in names)
                    {
                        addFeedback("{0} = {1}", name, counter);
                        counter++;
                    }

                    skipRead = true;
                    break;
                case "items":
                    var items = Enum.GetNames(typeof(Player.Items));
                    counter = 0;

                    foreach (string name in items)
                    {
                        addFeedback("{0} = {1}", name, counter);
                        counter++;
                    }

                    skipRead = true;
                    break;
                case "blocks":
                    var blocks = Enum.GetNames(typeof(World.Blocks));
                    counter = 0;

                    foreach (string name in blocks)
                    {
                        addFeedback("{0} = {1}", name, counter);
                        counter++;
                    }

                    skipRead = true;
                    break;
                case "foliage":
                    var foliage = Enum.GetNames(typeof(World.Foliage));
                    counter = 0;

                    foreach (string name in foliage)
                    {
                        addFeedback("{0} = {1}", name, counter);
                        counter++;
                    }

                    skipRead = true;
                    break;
                case "structures":
                    var structures = Enum.GetNames(typeof(World.Structures));
                    counter = 0;

                    foreach (string name in structures)
                    {
                        addFeedback("{0} = {1}", name, counter);
                        counter++;
                    }

                    skipRead = true;
                    break;
                case "colours_off":
                    enableColours = !enableColours;
                    skipRead = true;
                    break;
                case "open":
                    _ = 5;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    openDoor(ref player, _);
                    skipRead = true;
                    break;
                case "g":
                case "generate":
                    if (newgame(ref player, ref world, arguments))
                        setSpawn(ref player, ref world);

                    skipRead = true;
                    break;
                case "generate_foliage":
                    _ = DEFAULT_FOLIAGE_COUNT;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    world.cleanFoliage();
                    world.generateFoliage(_);
                    skipRead = true;
                    break;
                case "generate_rooms":
                    _ = DEFAULT_ROOM_COUNT;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }
                    world.cleanRooms();
                    world.placeRooms(_);
                    skipRead = true;
                    break;
                case "generate_structures":
                    _ = DEFAULT_STRUCTURE_COUNT;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }
                    world.placeStructures(_);
                    skipRead = true;
                    break;
                case "add_rooms":
                    _ = 1;
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }
                    world.placeRooms(_);
                    skipRead = true;
                    break;
                case "generate_doors":
                    world.placeDoors();
                    skipRead = true;
                    break;
                case "colours":
                case "colors":
                case "color":
                case "colour":
                    if (arguments.Length > 1)
                    {

                        if (arguments[0] != "")
                            if (int.TryParse(arguments[0], out int parse))
                            {
                                world.setColour(World.Blocks.WALL_SOLID, (ConsoleColor)parse);
                            }

                        if (arguments[1] != "")
                            if (int.TryParse(arguments[1], out int parse))
                            {
                                world.setColour(World.Blocks.FLOOR, (ConsoleColor)parse);
                            }
                    }
                    else
                    {
                        world.resetColours();
                        world.randomizeColours();
                    }
                    skipRead = true;
                    break;
                case "addxp":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    player.addXP(_);
                    skipRead = true;
                    break;
                case "removexp":
                    if (arguments.Length != 0 && arguments[0] != "")
                        if (int.TryParse(arguments[0], out int parse))
                        {
                            if (parse != 0)
                                _ = parse;
                        }

                    player.addXP(_);          
                    skipRead = true;
                    break;
                case "give":
                    if (arguments.Length == 0 && arguments[0] == "")
                        addFeedback("please specify an item");
                    else
                    {
                        if (player.itemExists(arguments[0].ToLower()))
                        {

                            Player.Items item = player.getItemFromName(arguments[0].ToLower());

                            if (arguments.Length > 1 && arguments[1] != "")
                                if (int.TryParse(arguments[1], out int parse))
                                {
                                    if (parse != 0)
                                        _ = parse;
                                }

                            player.addItem(item, _);
                            addFeedback($"added item {item} x{_}");
                        }
                    }

                    skipRead = true;
                    break;
            }
        }

        private static World createWorld(int width, int height)
        {

            World world = new World(width, height);
            world.loadStructures();
            return world;
        }

        private static Player createPlayer(string name)
        {

            Player player = new Player(name);
            player.heal(100);
            player.addItem(Player.Items.KEY, 1);
            player.addItem(Player.Items.HEALTH_POTION, 1);
            player.addItem(Player.Items.MANA_POTION, 1);
            return player;
        }
    }
}