using System;
using System.Diagnostics;

namespace NQueens_Problem
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("NQueens Problem Solver written by Lyds");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("- enter how many queens on your board/how big your board is. Large numbers can take a very long time.");
        top:
            Console.WriteLine("- memory allocated {0}bytes", (MemoryUsage() / 1024f) / 1024);
            Console.Write("type something: ");
            string line = Console.ReadLine();
            Console.WriteLine();
            if (int.TryParse(line, out int queens))
            {

                if (queens % 2 == 1)
                {
                    Console.WriteLine("even numbers are better");
                    goto top;
                }

                Queen.queenCount = Math.Abs(queens);
                new Queen(0, Int32.MinValue, null).walkThrough();

                Console.WriteLine();
                Console.WriteLine("finished!");
                Console.WriteLine("{0} queens per board", Queen.queenCount);
                Console.WriteLine("found {0} solutions", Queen.solutionsFound);
                Queen.solutionsFound = 0;
                goto top;
            }
            else
            {
                if (line.ToLower() == "clear")
                {
                    Console.Clear();
                    goto top;
                }
                else if (line.ToLower() != "exit")
                {
                    Console.WriteLine("enter numbers");
                    goto top;
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
