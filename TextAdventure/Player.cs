using System;

namespace TextAdventure
{
    class Player
    {


        public static int CAMERA_ZOOM_FACTOR = 0;
        public static int CAMERA_WIDTH = (int)(Program.WINDOW_WIDTH) - (Program.WINDOW_WIDTH / 5) - 18;
        public static int CAMERA_HEIGHT = (int)(Program.WINDOW_HEIGHT);
        public static int HUDX = CAMERA_WIDTH + 2;


        public const int MAX_MOVE_DISTANCE = 16;
        public const int MAX_HARVEST_DISTANCE = 4;

        public const int LEVEL_INTERVAL = 100;
        public const int MAX_HUNGER = 1000;
        public const int MAX_HEALTH = 100;
        public const int DEFAULT_HEALTH = 100;
        public const int MAX_STANIMA = 500;
        public const int MAX_MANA = 60;
        public const double STANIMA_FACTOR = 2.25;
        public const double STANIMA_INCREASE = 15;
        public const double HUNGER_FACTOR = 1.2;
        public const double HEALING_FACTOR = 0.25;
        public const int HUDY = Program.WINDOW_HEIGHT - 11;
        public const int HUDW = 46;
        public const int HUDH = 10;
        public const int HUDPADDING = 6;


        protected int health;
        protected int keys;
        protected int[] position = new int[2];
        protected int[] last_position = new int[2];
        protected long xp;
        protected int level;
        protected double hunger;
        protected double stanima;
        protected double mana;
        protected string playerName;

        public Player(string playerName)
        {

            this.playerName = playerName;
        }

        public void reset()
        {

            this.health = DEFAULT_HEALTH;
            this.hunger = MAX_HUNGER;
            this.stanima = MAX_STANIMA;
            this.mana = MAX_MANA;
        }

        public void drawHud(string header = "[player details]")
        {

            int currentx = Console.CursorLeft;
            int currenty = Console.CursorTop;

            Program.setColour(ConsoleColor.White);

            for (int y = 0; y < HUDH; y++)
                for (int x = 0; x < HUDW; x++)
                {

                    Console.SetCursorPosition(HUDX + x, HUDY + y);


                    if (y == 0 && x == 0)
                    {

                        Program.write("#");
                    }
                    else
                    if (y == 0 && x == 1)
                    {
                        Program.write(header);
                        x = header.Length;
                    }
                    else
                    if (x != 0 && y == 0)
                        Program.write("-");
                    else
                    if (x == 0 && y != 0)
                        Program.write("|");
                    else
                    if (x != 0 && y == HUDH - 1)
                        Program.write("-");
                    else
                    if (x == HUDW - 1 && y != 0)
                        Program.write("|");
                    else
                    if (x == HUDW - 1 && y == HUDH - 1)
                        Program.write("#");
                    else
                        Program.write(" ");
                }

            Console.SetCursorPosition(HUDX + HUDPADDING, HUDY + 2);
            Program.write("health: ");

            Program.setColour(ConsoleColor.Red);
            for (int i = 0; i < health; i++)
            {

                if (i % 10 == 0)
                    Program.write("#");
            }
            Program.write(" ({0}o2)", Math.Floor((double)health));

            Console.SetCursorPosition(HUDX + HUDPADDING, HUDY + 3);
            Program.write("stanima: ");

            Program.setColour(ConsoleColor.Yellow);
            for (int i = 0; i < stanima; i++)
            {

                if (i % 50 == 0)
                    Program.write(">");
            }

            Program.write(" ({0}J)", Math.Floor(stanima));

            Console.SetCursorPosition(HUDX + HUDPADDING, HUDY + 4);
            Program.write("calories: ");

            Program.setColour(ConsoleColor.Green);
            for (int i = 0; i < hunger; i++)
            {

                if (i % 100 == 0)
                    Program.write("C");
            }

            Program.write(" ({0}cal)", Math.Floor(hunger));

            Program.setColour(ConsoleColor.Yellow);
            Console.SetCursorPosition(HUDX + HUDPADDING, HUDY + 6);
            Program.write("level {0} ({1} to go)", level, ((level + 1) * LEVEL_INTERVAL) - xp);

            Program.setColour(ConsoleColor.Magenta);
            Console.SetCursorPosition(HUDX + HUDPADDING, HUDY + 7);
            Program.write("xp {0} ({1} levels)", xp, (xp / LEVEL_INTERVAL));

            Console.SetCursorPosition(currentx, currenty);
        }

        public void processTurn()
        {

            double hunger_reduction = HUNGER_FACTOR - Math.Abs(this.level / 10);
            double stanima_increase = STANIMA_INCREASE + Math.Abs(this.level / 10);
            double healing_factor = HEALING_FACTOR + Math.Abs(this.level / 10);

            if (this.hunger - hunger_reduction <= 0.0)
            {
                this.hunger = 0.0;
                this.health -= 1;
            }
            else
                this.hunger -= hunger_reduction;

            if (this.stanima + stanima_increase > MAX_STANIMA)
                this.stanima = MAX_STANIMA;
            else
                this.stanima += stanima_increase;

            if (this.health + healing_factor > MAX_HEALTH)
                this.health = MAX_HEALTH;
            else
                this.health += (int)Math.Floor(healing_factor);

            updateLevel();
        }

        public void decreaseStanima(int amount)
        {

            double stanima_decrease = (STANIMA_FACTOR + Math.Abs(this.level / 5)) * amount;

            if (this.stanima - stanima_decrease < 0)
                this.stanima = 0;

            this.stanima -= stanima_decrease;
        }

        public void decreaseHunger(int amount)
        {

            double hunger_decrease = (HUNGER_FACTOR + Math.Abs(this.level / 5)) * amount;

            if (this.hunger - hunger_decrease < 0)
                this.hunger = 0;

            this.hunger -= hunger_decrease;
        }

        public void setPosition(int x, int y)
        {

            int[] current_position = new int[2];
            current_position[0] = this.position[0];
            current_position[1] = this.position[1];

            this.last_position = current_position;
            this.position[0] = x;
            this.position[1] = y;
        }

        public int[] getLastPosition()
        {

            return this.last_position;
        }

        public bool hasLastPosition()
        {

            if (last_position[0] == 0 && last_position[1] == 0)
                return false;

            return true;
        }

        public void addKey(int amount)
        {

            this.keys += amount;
        }

        public void feed(int amount)
        {

            if (this.hunger + amount > MAX_HUNGER)
                this.hunger = MAX_HUNGER;

            this.hunger += amount;
        }

        public void charge(int amount)
        {

            if (this.mana + amount > MAX_MANA)
                this.mana = MAX_MANA;

            this.mana += amount;
        }

        public void removeKey(int amount)
        {

            this.keys -= amount;
        }

        public void damage(int amount)
        {

            this.health -= amount;
        }

        public void updateLevel()
        {

            int newlevel = (int)Math.Floor((decimal)(this.xp / LEVEL_INTERVAL));

            if (newlevel != level)
            {

                Program.addFeedback("level up!");
                Program.addFeedback("+ level {0}", newlevel);
            }

            this.level = newlevel;
        }

        public void addXP(long amount)
        {

            this.xp += amount;
        }

        public void removeXP(long amount)
        {

            if (this.xp - amount < 0)
                this.xp = 0;
            else
                this.xp -= amount;
        }

        public void heal(int amount)
        {

            if (this.health > 100)
                this.health = 100;

            this.health += amount;
        }

        public bool willExhaustPlayer(int amount)
        {
            double stanima_decrease = (STANIMA_FACTOR + Math.Abs(this.level / 5)) * amount;

            if (this.stanima - stanima_decrease < 0)
                return true;

            return false;
        }

        public double[] getLoses(int amount)
        {

            double[] loses = new double[2];
            loses[0] = (STANIMA_FACTOR + Math.Abs(this.level / 5)) * amount;
            loses[1] = (HUNGER_FACTOR + Math.Abs(this.level / 5)) * amount;

            return loses;
        }

        public int getKeys()
        {

            return this.keys;
        }

        public int getHealth()
        {

            return this.health;
        }

        public double getHunger()
        {

            return this.hunger;
        }

        public double getStanima()
        {

            return this.stanima;
        }

        public double getMana()
        {

            return this.mana;
        }

        public int getLevel()
        {

            return this.level;
        }

        public int getXPosition()
        {

            return this.position[0];
        }

        public int getYPosition()
        {

            return this.position[1];
        }

        public bool canOpenDoor()
        {

            if (this.keys <= 0)
                return false;

            return true;
        }

        public string getPlayerName()
        {

            return this.playerName;
        }

        public long getXP()
        {

            return this.xp;
        }
    }
}
