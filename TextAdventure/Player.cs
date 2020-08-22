using System;
using System.Collections.Generic;

namespace TextAdventure
{
    public class Player
    {

        public enum Items : int
        {

            NULL,
            HEALTH_POTION,
            APPLE,
            MANA_POTION,
            KEY,
            HONEY,
            BERRY,
            PEAR,
            PLUM,
            GOLDEN_APPLE,
            WOOD,
            NICE_SOUP,
            WOOD_CHIPPINGS,
            TWIG,
            PURE_MANA,
            PURE_OXYGEN,
            EGGPLANT,
            NICE_SALAD,
            LIQUID_STANIMA,
            LIQUID_EXPERIENCE,
            XP_VIAL,
            STANIMA_POTION
        }

        public enum YielType : int
        {

            STANIMA,
            HEALTH,
            MANA,
            XP,
            HUNGER
        }

        private Dictionary<Items, int> inventory = new Dictionary<Items, int>
        {

        };

        public static readonly Dictionary<Items, Dictionary<YielType, int>> itemYields = new Dictionary<Items, Dictionary<YielType, int>>
        {
            {
                Items.HEALTH_POTION, new Dictionary<YielType, int>()
                {
                    {YielType.HEALTH, 50}
                }
            },
            {
                Items.MANA_POTION, new Dictionary<YielType, int>()
                {
                    {YielType.MANA, 100}
                }
            },
            {
                Items.STANIMA_POTION, new Dictionary<YielType, int>()
                {
                    {YielType.STANIMA, 500}
                }
            },
            {
                Items.XP_VIAL, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 125}
                }
            },
            {
                Items.NICE_SOUP, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 125},
                    {YielType.STANIMA, 355},
                    {YielType.HUNGER, 555},
                    {YielType.HEALTH, 50},
                }
            },
            {
                Items.NICE_SALAD, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 250},
                    {YielType.STANIMA, 500},
                    {YielType.HUNGER, 1064},
                    {YielType.HEALTH, 100},
                }
            },
            {
                Items.BERRY, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 5},
                    {YielType.STANIMA, 20},
                    {YielType.HUNGER, 50},
                }
            },
            {
                Items.APPLE, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 10},
                    {YielType.STANIMA, 40},
                    {YielType.HUNGER, 20},
                }
            },
            {
                Items.PLUM, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 25},
                    {YielType.STANIMA, 60},
                    {YielType.HUNGER, 20},
                }
            },
            {
                Items.HONEY, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 30},
                    {YielType.STANIMA, 80},
                    {YielType.HUNGER, 40},
                }
            },
            {
                Items.EGGPLANT, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 30},
                    {YielType.STANIMA, 120},
                    {YielType.HUNGER, 80},
                }
            },
            {
                Items.GOLDEN_APPLE, new Dictionary<YielType, int>()
                {
                    {YielType.XP, 500},
                    {YielType.STANIMA, 500},
                    {YielType.HUNGER, 500},
                    {YielType.HEALTH, 100},
                }
            },
        };

        public static int CAMERA_ZOOM_FACTOR = 0;
        public static int CAMERA_WIDTH = (int)(Program.windowWidth) - (Program.windowWidth / 5) - 18;
        public static int CAMERA_HEIGHT = (int)(Program.windowHeight) - 2;
        public static int HUDX = CAMERA_WIDTH + 2;
        public static int HUDY = Program.windowHeight - 11;
        public static int HUDW = (int)(CAMERA_WIDTH / 2.5);

        private int[] position = new int[2];
        private static int[] last_position = new int[2];

        public const int MAX_MOVE_DISTANCE = 32;
        public const int MAX_HARVEST_DISTANCE = 8;
        public const int LEVEL_INTERVAL = 250;
        public const int MAX_HUNGER = 2000;
        public const int MAX_HEALTH = 201;
        public const int DEFAULT_HEALTH = 100;
        public const int DEFAULT_MOVES = 5;
        public const int MAX_STANIMA = 1000;
        public const int MAX_MANA = 100;
        public const int HUDH = 10;
        public const int HUDPADDING = 2;
        public const double STANIMA_FACTOR = 1.10;
        public const double STANIMA_INCREASE = 20;
        public const double HUNGER_FACTOR = 2.4;
        public const double HEALING_FACTOR = 0.75;

        private int health;
        private long xp;
        private int level;
        private int moves = DEFAULT_MOVES;
        private double hunger;
        private double stanima;
        private double mana;
        private string ame;
        private Crafting crafting;

        private List<int> discovereies;

        public static int[] LastPosition { get => last_position; set => last_position = value; }
        public long Xp { get => xp; }
        public int Level { get => level; }
        public int Health { get => health; }
        public int[] Position { get => position; }
        public double Hunger { get => hunger; }
        public double Stanima { get => stanima; }
        public double Mana { get => mana; }
        public string PlayerName { get => ame; set => ame = value; }
        public int Moves { get => moves; set => moves = value; }
        public Crafting Crafting { get => crafting; }
        public List<int> Discovereies { get => discovereies; }

        public Player(string ame)
        {

            this.ame = ame;
            this.crafting = new Crafting();
        }

        public void update()
        {

            CAMERA_WIDTH = (int)(Program.windowWidth) - (Program.windowWidth / 5) - 18;
            CAMERA_HEIGHT = (int)(Program.windowHeight);
            HUDX = CAMERA_WIDTH + 2;
            HUDY = Program.windowHeight - 11;
            updateLevel();
        }

        public void reset()
        {

            this.health = DEFAULT_HEALTH;
            this.hunger = MAX_HUNGER;
            this.stanima = MAX_STANIMA;
            this.mana = MAX_MANA;
        }

        public void drawDiscoveries(ref World world)
        {

            int currentx = Console.CursorLeft;
            int currenty = Console.CursorTop;
            int scale;
            if (Player.CAMERA_ZOOM_FACTOR > 1)
                scale = Player.CAMERA_ZOOM_FACTOR;
            else
                scale = 1;

            if (discovereies == null || discovereies.Count == 0)
                return;

            Program.setColour(ConsoleColor.White);

            foreach (int key in discovereies)
            {

                object[] discovery = world.getDiscovery(key);

                int startx = (int)discovery[0];
                int starty = (int)discovery[1];
                int xcounter = 0;
                int ycounter = 0;
                int positionx = -1;
                int positiony = -1;

                for (int y = Position[1] - (((Player.CAMERA_HEIGHT - Program.HUD_BUFFER_ZONE)) / 2 + (1 / scale)) * scale; y < Position[1] + (((Player.CAMERA_HEIGHT - Program.HUD_BUFFER_ZONE)) / 2 + (1 / scale)) * scale; y++)
                {

                    if (ycounter > Program.windowHeight)
                        continue;

                    if (Player.CAMERA_ZOOM_FACTOR > 1)
                        if (y % Player.CAMERA_ZOOM_FACTOR == 0)
                            if (y != Position[1] || y != starty)
                                continue;

                    if (positionx == -1)
                        for (int x = Position[0] - (((Player.CAMERA_WIDTH - Program.HUD_BUFFER_ZONE)) / 2 + (1 / scale)) * scale; x < Position[0] + (((Player.CAMERA_WIDTH - Program.HUD_BUFFER_ZONE)) / 2 + (1 / scale)) * scale; x++)
                        {

                            if (Player.CAMERA_ZOOM_FACTOR > 1)
                                if (x % Player.CAMERA_ZOOM_FACTOR == 0)
                                    if (x != Position[1] || x != startx )
                                        continue;

                            if (xcounter > Program.windowWidth)
                                continue;

                            if (y < 0 || x < 0)
                                continue;

                            if (startx == x)
                            {
                                positionx = xcounter;
                                break;
                            }


                            xcounter++;
                        }

                    if (starty == y)
                    {
                        positiony = ycounter;
                        break;
                    }

                    ycounter++;
                }

                if (positionx != -1 && positiony != -1)
                {

                    string name = world.getName(discovery);

                    if (positionx - name.Length / 2 > 0)
                        positionx = positionx - name.Length / 2;

                    Console.SetCursorPosition(positionx, positiony - 2);
                    Program.write(name);
                }
            }

            Console.SetCursorPosition(currentx, currenty);
        }

        public void drawHud(string header = "[details]")
        {

            int currentx = Console.CursorLeft;
            int currenty = Console.CursorTop;

            Program.setColour(ConsoleColor.White);

            for (int y = 0; y < HUDH; y++)
                for (int x = 0; x < HUDW; x++)
                {

                    if (HUDX + x < Console.WindowWidth && HUDY + y < Console.WindowHeight)
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

        public void processTurn(bool in_base = true)
        {

            double hunger_reduction = HUNGER_FACTOR - Math.Abs(this.level / 10);
            double stanima_increase = STANIMA_INCREASE + Math.Abs(this.level / 10);
            double healing_factor = HEALING_FACTOR + Math.Abs(this.level / 10);


            if (!in_base)
            {
                if (this.hunger - hunger_reduction <= 0.0)
                {
                    this.hunger = 0.0;
                    this.health -= 1;
                }
                else
                    this.hunger -= hunger_reduction;
            }
            else
            {

                stanima_increase = stanima_increase * 2;
                healing_factor = healing_factor * 2;
            }


            if (this.stanima + stanima_increase > MAX_STANIMA)
                this.stanima = MAX_STANIMA;
            else
                this.stanima += stanima_increase;

            if (this.health + healing_factor > MAX_HEALTH)
                this.health = MAX_HEALTH;
            else
                this.health += (int)Math.Floor(healing_factor);

            int[] array =
            {
                (int)stanima_increase,
                0 - (int)hunger_reduction,
                (int)healing_factor
            };

            Program.addChange(array);

            updateLevel();
        }

        public void decreaseStanima(int amount)
        {


            double stanima_decrease = (STANIMA_FACTOR + Math.Abs(this.level / 5)) * amount;

            if (this.stanima - stanima_decrease < 0)
                this.stanima = 0;

            this.stanima -= stanima_decrease;

            Program.addChange("stanima", 0 - (int)stanima_decrease);
        }

        public void decreaseHunger(int amount)
        {

            double hunger_decrease = (HUNGER_FACTOR + Math.Abs(this.level / 5)) * amount;

            if (this.hunger - hunger_decrease < 0)
                this.hunger = 0;

            this.hunger -= hunger_decrease;

            Program.addChange("hunger", 0 - (int)hunger_decrease);
        }

        public void setPosition(int x, int y)
        {

            int[] current_position = new int[2];
            current_position[0] = this.position[0];
            current_position[1] = this.position[1];

            Player.LastPosition = current_position;
            this.position[0] = x;
            this.position[1] = y;
        }

        public void feed(int amount)
        {

            Program.addChange("hunger", amount);

            if (this.hunger + amount > MAX_HUNGER)
                this.hunger = MAX_HUNGER;

            this.hunger += amount;
        }

        public void charge(int amount)
        {

            if (this.mana + amount > MAX_MANA)
                this.mana = MAX_MANA;

            Program.addChange("mana", amount);

            this.mana += amount;
        }

        public void damage(int amount)
        {


            Program.addChange("health", amount);

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

            Program.addChange("xp", (int)amount);
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

            Program.addChange("health", amount);

            if (this.health > 100)
                this.health = 100;

            this.health += amount;
        }

        public void replenish(float amount)
        {

            Program.addChange("stanima", (int)amount);

            if (this.stanima + amount > MAX_STANIMA)
                this.stanima = MAX_STANIMA;

            this.stanima += amount;
        }

        public void printInventory()
        {

            var keys = this.inventory.Keys;
            Program.addFeedback("[inventory] ");

            foreach (Items item in keys)
                Program.addFeedback("{0} x{1}", Enum.GetName(typeof(Items), item), this.inventory[item]);
        }

        public void addItem(Items item, int amount = 1)
        {

            if (this.inventory.ContainsKey(item))
                this.inventory[item] = this.inventory[item] + amount;
            else
                this.inventory[item] = amount;
        }

        public void removeItem(Items item, bool whole_item = false, int amount = 1)
        {

            if (this.inventory.ContainsKey(item))
            {

                if (whole_item)
                    this.inventory.Remove(item);
                else
                {

                    if (this.inventory[item] - amount > 0)
                        this.inventory[item] = this.inventory[item] - amount;
                    else
                        this.inventory.Remove(item);
                }
            }
        }

        public void addDiscovery(int key)
        {

            if (discovereies == null)
                discovereies = new List<int>();

            discovereies.Add(key);
        }

        public Items getItemFromName(string item_name)
        {


            var names = Enum.GetNames(typeof(Items));
            var key = 0;

            foreach (string name in names)
            {
                if (name.ToLower() == item_name.ToLower())
                    return (Items)key;

                key++;
            }

            return Items.NULL;
        }

        public double[] getLoses(int amount)
        {

            double[] loses = new double[2];
            loses[0] = (STANIMA_FACTOR + Math.Abs(this.level / 5)) * amount;
            loses[1] = (HUNGER_FACTOR + Math.Abs(this.level / 5)) * amount;

            return loses;
        }

        public int getItemQuantity(Items item)
        {

            return (this.inventory[item]);
        }

        public bool hasLastPosition()
        {

            if (LastPosition[0] == 0 && LastPosition[1] == 0)
                return false;

            return true;
        }

        public bool willExhaustPlayer(int amount)
        {
            double stanima_decrease = (STANIMA_FACTOR + Math.Abs(this.level / 5)) * amount;

            if (this.stanima - stanima_decrease < 0)
                return true;

            return false;
        }

        public bool itemExists(string item_name)
        {

            var names = Enum.GetNames(typeof(Items));

            foreach (string name in names)
                if (name.ToLower() == item_name.ToLower())
                    return true;

            return false;
        }

        public bool newUseItem(Items item)
        {

            if (!itemYields.ContainsKey(item))
                return false;
            else
            {

                var yield = itemYields[item];

                foreach(KeyValuePair<YielType, int> keyValuePair in yield)
                {

                    switch(keyValuePair.Key)
                    {
                        case YielType.STANIMA:
                            charge(keyValuePair.Value);
                            Program.addFeedback("+ S {0}", keyValuePair.Value);
                            break;
                        case YielType.HEALTH:
                            heal(keyValuePair.Value);
                            Program.addFeedback("+ H {0}", keyValuePair.Value);
                            break;
                        case YielType.HUNGER:
                            feed(keyValuePair.Value);
                            Program.addFeedback("+ F {0}", keyValuePair.Value);
                            break;
                        case YielType.XP:
                            addXP(keyValuePair.Value);
                            Program.addFeedback("+ xp {0}", keyValuePair.Value);
                            break;
                        default:
                            return false;
                    }
                }

                return true;
            }
        }

        //TODO: Update to use a Dictionary<Player.Items, int>
        public bool useItem(Items item)
        {

            switch (item)
            {
                default:
                    Program.addFeedback("you can't use this, it may need crafting");
                    return false;
                case Items.MANA_POTION:
                    Program.addFeedback("+ M 50");
                    this.charge(50);
                    return true;
                case Items.STANIMA_POTION:
                    Program.addFeedback("+ S 500");
                    this.charge(500);
                    return true;
                case Items.XP_VIAL:
                    Program.addFeedback("+ xp 125");
                    this.addXP(125);
                    return true;
                case Items.HEALTH_POTION:
                    Program.addFeedback("+ H 50");
                    this.heal(50);
                    return true;
                case Items.NICE_SALAD:
                    Program.addFeedback("+ S 1000");
                    Program.addFeedback("+ F 1000");
                    Program.addFeedback("+ H 100");
                    this.feed(1000);
                    this.replenish(1000);
                    this.heal(100);
                    return true;
                case Items.NICE_SOUP:
                    Program.addFeedback("+ S 500");
                    Program.addFeedback("+ F 1000");
                    Program.addFeedback("+ H 100");
                    this.feed(500);
                    this.replenish(500);
                    this.heal(50);
                    return true;
                case Items.GOLDEN_APPLE:
                    Program.addFeedback("+ S 250");
                    Program.addFeedback("+ F 500");
                    Program.addFeedback("+ H 100");
                    Program.addFeedback("+ xp 100");
                    this.feed(500);
                    this.replenish(250);
                    this.heal(100);
                    this.addXP(100);
                    return true;
                case Items.BERRY:
                    Program.addFeedback("+ S 5");
                    Program.addFeedback("+ F 5");
                    Program.addFeedback("+ xp 2");
                    this.replenish(5);
                    this.feed(10);
                    this.addXP(2);
                    return true;
                case Items.PEAR:
                case Items.PLUM:
                case Items.HONEY:
                case Items.APPLE:
                    Program.addFeedback("+ S 10");
                    Program.addFeedback("+ F 20");
                    Program.addFeedback("+ xp 4");
                    this.feed(5);
                    this.replenish(10);
                    this.addXP(4);
                    return true;
                case Items.KEY:
                    Program.addFeedback("'use the unlock command' the key whispers");
                    return false;
            }
        }

        public bool hasItem(Items item)
        {

            if (this.inventory.Count == 0)
                return false;

            return (this.inventory.ContainsKey(item));
        }

        public bool canOpenDoor()
        {

            return (this.hasItem(Items.KEY));
        }
    }
}
