using System;

namespace NQueens_Problem
{
    class Queen
    {

        public static int queenCount = 12;
        public static int solutionsFound = 0;
        private static Queen queenAbove;
        private readonly int line;
        private readonly int row;
        private Queen parent;

        public Queen(int line, int row, Queen parent)
        {

            this.line = line;
            this.row = row;
            this.parent = parent;
        }

        public void WalkThrough()
        {

            if (line == queenCount)
            {

                solutionsFound++;
                printSolution(this);
                return;
            }

            for (var r = 0; r < queenCount; r++)
            {
                queenAbove = this;
                while (queenAbove.row >= 0 && r != queenAbove.row
                       && r - queenAbove.row != line + 1 - queenAbove.line
                       && queenAbove.row - r != line + 1 - queenAbove.line)
                    queenAbove = queenAbove.parent;
                if (queenAbove.line == 0)
                    new Queen(line + 1, r, this).WalkThrough();
            }
        }

        private static void printSolution(Queen queen)
        {

            char[,] board = new char[queenCount + 1, queenCount + 1];

            while (queen.row >= 0)
            {
                queen = queen.parent;

                if (queen.row >= 0)
                    board[queen.line, queen.row] = (char)('a' + queen.row);
            }

            Console.WriteLine();
            for (int x = 0; x < queenCount; x++)
            {
                for (int y = 0; y < queenCount; y++)
                {

                    if (board[x, y] == 0)
                        Console.Write(" " + "n0" + " ");
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" " + board[x, y] + x + " ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        }
    }
}
