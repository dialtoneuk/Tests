using System;
using System.Collections.Generic;
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

        public readonly Dictionary<string, Action<object[]>> commands;
        public readonly Dictionary<States, Action<object[]>> draw;

        //default state
        protected States currentState = States.UNKNOWN;
        protected InputType inputType = InputType.NORMAL;

        //actual command string
        private string command;
        private int frames = 0;

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
                        this.currentState = States.GAME;
                    }
                },
                { "menu",
                    command =>
                    {
                        this.currentState = States.MENU;
                    }
                },
                { "move",
                    command =>
                    {
                          
                    }
                },
                { "input_type",
                    command =>
                    {
                        InputType type;
                        if(command.Length>1)
                            type = (InputType)command[1];
                        else
                            type = InputType.NORMAL;

                        inputType = type;
                        Console.WriteLine("changed input type to {0}", type);
                    }
                }
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
                    }
                },
            };
        }

        //starts the thing
        public void Start()
        {

            while(currentState!=States.EXIT)
            {
                //clear screen
                Console.Clear();

                //Process command
                ProcessCommand(this.command);

                //Draw
                Draw();

                //we read the command
                this.command = ReadLine(inputType);

                //frames counter
                frames++;
            }
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
                Console.WriteLine("! invalid state {0} !", currentState);
            else
                draw[currentState](Parse(this.command));
        }

        //calls a command lambda function
        public bool ProcessCommand(string _command)
        {

            object[] command = Parse(_command);

            if(inputType == InputType.NORMAL)
            {
                if (command.Length == 0)
                    return false;

                if (!commands.ContainsKey((string)command[0]))
                    Console.WriteLine("! invalid command {0} !", command[0]);
                else
                {
                    commands[(string)command[0]](command);
                    return true;
                }
            }
    
            return false;
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

                    if (int.TryParse(objects[1], out int result))
                        array[1] = result;

                    for (int i = 2; i < objects.Length; i++)
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

                    if (int.TryParse(parse, out int result))
                        array[counter] = result;

                    counter++;
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
