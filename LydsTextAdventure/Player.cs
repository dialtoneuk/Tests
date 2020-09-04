using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LydsTextAdventure
{

    [Serializable]
    public class Player : Command
    {

        protected int Health = 0;
        protected int Stanima = 0;
        protected int Calories = 0;
        protected int XP = 0;

        public int[] Position = new int[2];

        protected string Name;

        public enum Directions : int
        {

            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        public Player(string Name)
        {

            this.Name = Name;
        }

        public void AddCommands(Game game)
        {
     
            game.AddCommand("player_details", command =>
            {

                Console.WriteLine("{0} {1} {2} {3} [{4}/{5}]", Name, XP, Health, Calories, Position[0], Position[1]);
            });

            game.AddCommand("left", command =>
            {

                if (!Game.CommandIsInt(command[1]))
                    return;

                MovePlayer(Directions.LEFT, (int)command[1]);
            });

            game.AddCommand("right", command =>
            {

                if (!Game.CommandIsInt(command[1]))
                    return;

                MovePlayer(Directions.RIGHT, (int)command[1]);
            });

            game.AddCommand("up", command =>
            {

                if (!Game.CommandIsInt(command[1]))
                    return;

                MovePlayer(Directions.UP, (int)command[1]);
            });

            game.AddCommand("down", command =>
            {

                if (!Game.CommandIsInt(command[1]))
                    return;

                MovePlayer(Directions.DOWN, (int)command[1]);
            });

            //Abbreviations
            Game.Abbreviations.Add("left", new string[]{
                "a",
                "west"
            });

            Game.Abbreviations.Add("right", new string[]{
                "d",
                "east"
            });

            Game.Abbreviations.Add("up", new string[]{
                "w",
                "north"
            });

            Game.Abbreviations.Add("down", new string[]{
                "s",
                "south"
            });
        }

        public void DrawPlayer()
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("P");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void MovePlayer(Directions direction, int amount)
        {
           
            switch(direction)
            {
                case Directions.LEFT:
                    Position[0] -= amount;
                    break;
                case Directions.RIGHT:
                    Position[0] += amount;
                    break;
                case Directions.UP:
                    Position[1] -= amount;
                    break;
                case Directions.DOWN:
                    Position[1] += amount;
                    break;
            }
        }

        public int GetX()
        {

            return (Position[0]);
        }

        public int GetY()
        {

            return (Position[1]);
        }

        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            string filename = String.Format("players/{0}.player", Name);

            if (!Directory.Exists("players/"))
                Directory.CreateDirectory("players/");

            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, this);
            stream.Close();

            Debug.WriteLine("saved player {0}", (object)Name);
        }

        public static Player Load(string Name)
        {

            IFormatter formatter = new BinaryFormatter();
            string filename = String.Format("players/{0}.player", Name);

            if (!Directory.Exists("players/"))
                throw new ApplicationException();

            if (!File.Exists(filename))
                return null;

            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            Player player = (Player)formatter.Deserialize(stream);

            stream.Close();

            Debug.WriteLine("loaded player {0}", (object)Name);

            return player;
        }
    }
}
