using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LydsTextAdventure
{
    public class Game
    {

        //an array of all the possible states
        public enum States : int
        {
            UNKNOWN,
            MENU,
            GAME,
            EXIT
        }

        public enum InputType : int
        {
            NORMAL,
            MOVEMENT_MOVE,
            SHORT_HAND
        }

        //default state
        protected States currentState = States.UNKNOWN;
        //default input type
        protected InputType inputType = InputType.NORMAL;
        protected World currentWorld;
        protected readonly Dictionary<string, Action<object[]>> commands;
        protected readonly Dictionary<States, Action<object[]>> draw;

        //actual command string
        private string command;
        private int frames = 0;

        public static Dictionary<string, string[]> abbreviations = new Dictionary<string, string[]>
        {
            {"exit",new string[]{
                "e",
            }},
            {"play",new string[]{
                "p",
                "n"
            }},
            {"load",new string[]{
                "l",
            }},
            {"input_type",new string[]{
                "i",
            }}
        };

        /**
         *  The main logic for the commands and drawing is programmed here inside lambda functions,
         *  the element which is passed is the parsed command the user has entered.
         *  
         *  The parsed command is broken down into an object[]. depending 
         *  
         *  This is as follows
         *      object[0] = full named argument,
         *      object[1-?] = parameters which could be either a string or an int. Depends on the logic you program 
         *                    and what you are expecting. Just remember to check types when using this object.
         */
        public Game()
        {

            //TODO: maybe load music and stuff here?

            this.commands = new Dictionary<string, Action<object[]>>
            {
                { "exit",
                    command =>
                    {
                        this.currentState = States.EXIT;
                    }
                },
                { "play",
                    command =>
                    {

                        string filename = "default";

                        //roso easier unbox check
                        if(command.Length>1)
                            if(typeof(int)!=command[1].GetType())
                                filename = (string)command[1];

                        if(this.currentWorld==null)
                        {
                            this.currentWorld = World.NewWorld(filename);
                            this.currentWorld.AddCommands(this);
                        }

                        this.currentWorld.GenerateWorld();
                        this.currentWorld.Save();
                        this.currentState = States.GAME;
                    }
                },
                { "load",
                    command =>
                    {
                        string filename = "default";
                        if(command.Length>1)
                            if(typeof(int)!=command[1].GetType())
                                filename = (string)command[1];

                        if(this.currentWorld==null)
                        {

                            this.currentWorld = World.LoadWorld(filename);

                            if(this.currentWorld==null)
                                Debug.WriteLine("world does not exist {0}", (object)filename);
                            else
                            {
                                this.currentWorld.AddCommands(this);
                                this.currentState = States.GAME;
                            }
                        }
                    }
                },
                { "default_settings",
                    command =>
                    {

                        File.WriteAllText("user_settings.json", File.ReadAllText("default_settings.json"));
                        Program.RefreshSettings();

                        Debug.WriteLine("settings reset");
                    }
                },
                { "menu",
                    command =>
                    {
                        this.currentState = States.MENU;
                    }
                },
                { "input_type",
                    command =>
                    {

                        InputType type;
                        if(command.Length>1&&typeof(int)==command[1].GetType())
                            type = (InputType)command[1];
                        else
                            type = InputType.NORMAL;

                        inputType = type;

                        Program.Settings.SettingsValues["input_type"] = (int)type;
                        Debug.WriteLine("changed input type to {0}", type);
                    }
                },
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
                { "settings",
                    command =>
                    {

                        foreach(KeyValuePair<string,object> obj in Program.Settings.SettingsValues)
                            Console.WriteLine("{0} {1}", obj.Key, obj.Value);
                    }
    
                }, 
                { "change",
                    command =>
                    {

                        if(command.Length<3)
                            return;

                        if(!Program.Settings.SettingsValues.ContainsKey((string)command[1]))
                            return;

                        if(command[2].GetType() == typeof(int))
                            Program.Settings.SettingsValues[command[1].ToString()] = command[2];
                        else
                            Program.Settings.SettingsValues[(string)command[1]] = command[2];

                        foreach(KeyValuePair<string,object> obj in Program.Settings.SettingsValues)
                            Console.WriteLine("{0} {1}", obj.Key, obj.Value);
                    }
                },
            };

            this.draw = new Dictionary<States, Action<object[]>>
            {
                {
                    States.MENU,
                    command =>
                    {

                        Console.WriteLine("fames: {0} / state: {1}", frames, currentState);
                    }
                },
                {
                    States.GAME,
                    command =>
                    {
                        Console.WriteLine("fames: {0} / state: {1}", frames, currentState);

                        if(this.currentWorld!=null)
                            this.currentWorld.DrawWorld();
                    }
                },
            };
        }

        //starts the thing
        public void Start()
        {

            Console.Title = "Lyds Text Adventure";

            inputType = (InputType)Program.Settings.GetIntOrZero("input_type");

            while(currentState!=States.EXIT)
            {
                //clear screen
                Console.Clear();

                //Process command
                ProcessCommand(this.command);

                //Draw
                Draw();


                if(currentState!=States.EXIT)
                    //we read the command
                    this.command = ReadLine(inputType);

                //frames counter
                frames++;
            }

            if (Program.Settings.IsChecked("auto_save"))
                if (this.currentWorld != null)
                    this.currentWorld.Save();
        }

        public void AddCommand(string command, Action<object[]> action)
        {

            this.commands[command] = action;
        }

        //just sets the current state to exit which stops the loop
        public void Stop()
        {
            currentState = States.EXIT;
        }

        //calls the lambda function related to the current state
        public void Draw()
        {

            if (!draw.ContainsKey(currentState))
                Debug.WriteLine("! invalid state {0} !", currentState);
            else
                draw[currentState](Parse(this.command));
        }

        //calls a command lambda function
        public bool ProcessCommand(string _command)
        {

            object[] command = Parse(_command);

            if (command.Length == 0)
                return false;

            if(inputType == InputType.NORMAL)
            {

                if (!commands.ContainsKey((string)command[0]))
                    Debug.WriteLine("! invalid command {0} !", command[0]);
                else
                {
                    commands[(string)command[0]](command);
                    return true;
                }
            }

            if(inputType == InputType.SHORT_HAND)
            {

                string actualCommand = GetCommandFromShorthand(command);

                if (actualCommand == null)
                    Debug.WriteLine("! invalid shorthand command {0} !", command[0]);
                else
                {
                    if (!commands.ContainsKey(actualCommand))
                        Debug.WriteLine("! invalid command {0} !", actualCommand);
                    else
                    {

                        commands[actualCommand](command);
                        return true;
                    }
                }
            }
    
            return false;
        }

        private string GetCommandFromShorthand(object[] command)
        {

            foreach (KeyValuePair<string, string[]> keyValue in abbreviations)
            {

                foreach (string shorthand in keyValue.Value)
                    if(command.Length!=0)
                        if (command[0].ToString().StartsWith(shorthand[0]))
                            return keyValue.Key;
            }

            return null;
        }

        //parses a command in various different ways
        public object[] Parse(string values)
        {

            if (values == null)
                return new object[0];

            //will split up the string by a space
            if(inputType == InputType.NORMAL)
            {

                string[] objects = values.Trim().Split(' ');

                if (objects.Length <= 1)
                    return (object[])objects;
                else
                {

                    object[] array = new object[objects.Length];
                    array[0] = objects[0];

                    for (int i = 1; i < objects.Length; i++)
                        if (Regex.IsMatch((string)objects[i], @"^\d+$"))
                            if (int.TryParse((string)objects[i], out int result))
                                array[i] = result;
                            else
                                array[i] = objects[i];
                        else
                            array[i] = objects[i];

                    return array;
                }
            }
            
            //Will split up by movement
            if(inputType == InputType.MOVEMENT_MOVE)
            {
                object[] array = new object[1];
                array[0] = values.Trim();
                return array;
            }

            //Will split up by Shorthand
            if (inputType == InputType.SHORT_HAND)
            {

                string[] numbers = Regex.Split(values, @"\D+");
                object[] array = new object[numbers.Length + 1];
                int counter = 1;
                array[0] = values[0];

                foreach (string parse in numbers)
                {

                    if (parse == ""||parse==null)
                        continue;

                    if (int.TryParse(parse, out int result))
                    {

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                        if (result != null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                        {
                            array[counter] = result;
                            counter++;
                        }
                    }
                    else
                        throw new ApplicationException();
                }

                return array;
            }

            throw new Exception(String.Format("invalid inputType {0}", inputType));
        }

        //reads a line from the console and a key depending on input type.
        public string ReadLine(InputType inputType)
        {

            string line;
            if (inputType == InputType.NORMAL || inputType == InputType.SHORT_HAND)
            {

                Console.Write("> ");
                repeat:
                line = Console.ReadLine();

                if (line.Length == 0 || line == "")
                    goto repeat;
            }
            else
                line = Console.ReadKey().ToString();

            return line;
        }
    }
}
