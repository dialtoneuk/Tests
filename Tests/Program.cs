using System;
using System.Diagnostics;

namespace Tests
{
    class Program
    {

        protected static string line = "bepis";
        protected static bool auto = false;
        private static int auto_steps = 1;


        static void Main(string[] args)
        {

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Prime Number Finder written by Lyds");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("- type auto to automatically find prime numbers. clear to clean screen");

            while (line != "")
            {

                int a = 0;

                if (int.TryParse(line, out int n))
                {


                    for (int i = 1; i <= n; i++)
                        if (n % i == 0)
                        {
                            a++;
                        }

                    if (a == 2)
                        Console.WriteLine("[{0}] is a prime number!", n);
                    else if (!auto)
                        Console.WriteLine("[{0}] is not a prime number!", n);
                }
                else
                {

                    if (line == "auto")
                    {
                        Console.WriteLine("please enter how many numbers ahead to check");

                        int.TryParse(Console.ReadLine(), out int parsedNumber);

                        if (parsedNumber == 0)
                            parsedNumber++;

                        auto_steps = parsedNumber;
                        auto = !auto;
                    }
                    else
                        if (line == "clear")
                        Console.Clear();
                }

                if (auto)
                {
                    if (auto_steps > 0)
                    {
                        int newNumber = n + 1;
                        line = newNumber.ToString();
                        auto_steps--;
                    }
                    else
                    {
                        auto = !auto;
                    }
                }
                else
                {
                    Console.WriteLine("- memory allocated {0}bytes", (MemoryUsage() / 1024f) / 1024);
                    Console.Write("type something: ");
                    line = Console.ReadLine();
                }
            }
        }

        static long MemoryUsage()
        {

            Process proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64;
        }
    }
}
