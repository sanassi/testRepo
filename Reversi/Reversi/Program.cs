using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi
{
    internal class Program
    {
        private const char Player1Piece = 'X';
        private const ConsoleColor Player1Color = ConsoleColor.Blue;
        private const char Player2Piece = '0';
        private const ConsoleColor Player2Color = ConsoleColor.Red;
        private const char EmptyPiece = '.';
        
        
        public static void Main()
        {
            StartGame();
        }

        static char[,] InitBoard()
        {
            Console.Write("Enter Board Dimension: ");
            int size = Convert.ToInt32(Console.ReadLine());
            char[,] grid = new char[size,size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    grid[i, j] = EmptyPiece;
                }
            }

            grid[3, 3] = Player1Piece;
            grid[4, 4] = Player1Piece;
            grid[4, 3] = Player2Piece;
            grid[3, 4] = Player2Piece;

            return grid;
        }
        static void PrintBoard(char[,] grid)
        {
            Console.Clear();
            Console.WriteLine("        Black & White");
            Console.Write("    ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            for (int e = 0; e < grid.GetLength(0); e++)
            {
                Console.Write((char)(65 + e) + "  ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            
            Console.Write('\n');
            
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(i + 1 + "   ");
                Console.ForegroundColor = ConsoleColor.White;
                
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i, j] == Player1Piece)
                    {
                        Console.ForegroundColor = Player1Color;
                        Console.Write(Player1Piece + "  ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (grid[i, j] == Player2Piece)
                    {
                        Console.ForegroundColor = Player2Color;
                        Console.Write(Player2Piece + "  ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (grid[i, j] == EmptyPiece)
                    {
                        Console.Write(EmptyPiece + "  ");
                    }
                }
                Console.Write('\n');
            }
        }
        static void Flip(char[,] grid, List<int> coo, bool player1Turn)
        {
            char playerPiece;

            if (player1Turn)
            {
                playerPiece = Player1Piece;
            }
            else
            {
                playerPiece = Player2Piece;
            }

            int i = 0;
            for (int j = 1; j < coo.Count; j+=2, i+=2)
            {
                grid[coo[i], coo[j]] = playerPiece;
            }
        }
        static List<int> Neighbours(char[,] grid, int x, int y, bool player1Turn)
        {
            List<int> neighbours = new List<int> {1,1,1,0,1,-1,0,1,0,-1,-1,1,-1,0,-1,-1};
            List<int> coordinatesToFlip = new List<int>();
            List<int> save = new List<int>();

            char playerPiece;

            if (player1Turn)
            {
                playerPiece = Player2Piece;
            }
            else
            {
                playerPiece = Player1Piece;
            }

            for (int i = 0, j = 1; j < neighbours.Count; i += 2, j += 2)
            {
                int a = x;
                int b = y;
                while (a + neighbours[i] < grid.GetLength(0)
                       && b + neighbours [j] < grid.GetLength(0)
                       && a + neighbours[i] > 0
                       && b + neighbours[j] > 0)
                {
                    if (grid[a + neighbours[i], b + neighbours[j]] == playerPiece)
                    {
                        save.Add(a + neighbours[i]);
                        save.Add(b + neighbours[j]);
                        a += neighbours[i];
                        b += neighbours[j];
                    }
                    else
                    {
                        break;
                    }
                }

                if (a + neighbours[i] < grid.GetLength(0)
                    && b + neighbours [j] < grid.GetLength(0)
                    && a + neighbours[i] > 0
                    && b + neighbours[j] > 0
                    && grid[a + neighbours[i], b + neighbours[j]] == EmptyPiece)
                {
                    save = new List<int>();
                }
                else
                {
                    coordinatesToFlip = coordinatesToFlip.Concat(save).ToList();
                }
            }

            return coordinatesToFlip;
        }
        static bool Pass(char[,] grid, bool player1Turn)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (Neighbours(grid, i, j, player1Turn).Count != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        static void Play(char[,] grid, bool player1Turn) 
        {
            char playerPiece;
            string action;
            Console.Write("         Player ");
            if (player1Turn)
            {
                Console.ForegroundColor = Player1Color;
                playerPiece = Player1Piece;
                Console.Write(playerPiece);
            }
            else
            {
                Console.ForegroundColor = Player2Color;
                playerPiece = Player2Piece;
                Console.Write(playerPiece);
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('\n');
            Count(grid);
            
            Console.Write("Commands :\n    P: Place Pawn\n    A: Abandon\n    H: Help\nEnter Command: ");
            action = Console.ReadLine();

            if ((action == "P" || action == "H") && Pass(grid, player1Turn) == false)
            {
                if (action == "H")
                {
                    Help(grid, player1Turn);
                }
                Console.Write("   \nEnter Coordinates: ");
                string coord = Console.ReadLine();

                while (coord == null)
                {
                    coord = Console.ReadLine();
                }

                int col = coord[0] - 65;

                int line = Convert.ToInt32(Char.GetNumericValue(coord[1])) - 1;

                while (IsValid(line, col, grid, player1Turn) == false)
                {
                    Console.Write("Enter Coordinates: ");
                    coord = Console.ReadLine();

                    col = coord[0] - 65;

                    line = Convert.ToInt32(Char.GetNumericValue(coord[1])) - 1;
                }

                grid[line, col] = playerPiece;
            
                if (Neighbours(grid, line, col, player1Turn).Count != 0)
                {
                    Flip(grid, Neighbours(grid, line, col, player1Turn), player1Turn);
                }
            }
            
            if (Pass(grid,player1Turn))
            {
                Console.WriteLine("can't play.");
            }
        }
        static bool IsValid(int x, int y, char[,] grid, bool player1Turn)
        {
            if (x >= 0 & y >= 0 & x < grid.GetLength(0) & y < grid.GetLength(0) & Neighbours(grid, x, y, player1Turn).Count != 0)
            {
                return true;
            }

            return false;
        }
        static void Help(char[,] grid, bool player1Turn)
        {
            List<int> help = new List<int>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i,j] == EmptyPiece && Neighbours(grid, i, j, player1Turn).Count != 0)
                    {
                        help.Add(j);
                        help.Add(i + 1);
                    }
                }
            }

            if (help.Count != 0)
            {
                Console.Write("Help. You can play at: ");
            }

            for (int i = 0, j = 1; j < help.Count; i += 2, j += 2)
            {
                Console.Write("{0}{1} ",(char) (help[i] + 65), help[j]);
            }
        }
        static bool Cleared(char[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i, j] == EmptyPiece)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        static char Winner(char[,] grid)
        {
            int player1 = 0;
            int player2 = 0;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i, j] == Player1Piece)
                    {
                        player1++;
                    }

                    if (grid[i, j] == Player2Piece)
                    {
                        player2++;
                    }
                }
            }

            if (player1 > player2)
            {
                return Player1Piece;
            }

            if (player2 > player1)
            {
                return Player2Piece;
            }

            return EmptyPiece;
        }
        static bool Count(char[,] grid)
        {
            int player1 = 0;
            int player2 = 0;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i, j] == Player1Piece)
                    {
                        player1++;
                    }

                    if (grid[i, j] == Player2Piece)
                    {
                        player2++;
                    }
                }
            }
            
            Console.WriteLine(" Player {0}: {1}  Player {2}: {3}\n__________________________", Player1Piece, player1, Player2Piece, player2);

            if (player1 == 0 || player2 == 0)
            {
                Console.WriteLine("The Winner Is {0}!", Winner(grid));
                return true;
            }
            
            return false;
        }
        static bool MutualBlock(char[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i, j] == EmptyPiece &&
                        (Neighbours(grid, i, j, true).Count != 0 || Neighbours(grid, i, j, false).Count != 0))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        static bool End(char[,] grid)
        {
            if (Cleared(grid))
            {
                Console.Write("Board Is Full.");
                Console.Write("The Winner Is {0}", Winner(grid));
                return true;
            }

            if (Count(grid))
            {
                PrintBoard(grid);
                return Count(grid);
            }

            if (MutualBlock(grid))
            {
                PrintBoard(grid);
                Console.Write("         No Winner.");
                return true;
            }

            return false;
        }
        static bool Swap(bool player1Turn)
        {
            if (player1Turn)
            {
                return false;
            }

            return true;
        }
        static void StartGame()
        {
            char[,] board = InitBoard();

            bool player1Turn = true;

            while (End(board) == false)
            {
                PrintBoard(board);
                Play(board, player1Turn);
                player1Turn = Swap(player1Turn);
            }
            
        }

        static void WeakAi(char[,] grid, bool player1Turn)
        {
            List<int> playable = new List<int>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[i,j] == EmptyPiece && Neighbours(grid, i, j, player1Turn).Count != 0)
                    {
                        playable.Add(j);
                        playable.Add(i + 1);
                    }
                }
            }
            
            Random rand = new Random();
        }
    }
}