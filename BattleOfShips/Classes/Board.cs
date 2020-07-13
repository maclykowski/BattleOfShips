using System;
using System.Linq;

namespace BattleOfShips
{
    class Board
    {
        public int NoOfHumanPlayers { get; }
        public Board(int noOfPlayers)
        {
            NoOfHumanPlayers = noOfPlayers;
        }

        private void DrawDestroyedCell()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("   ");
            Console.ResetColor();
        }
        private void DrawHitCell()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" X ");
            Console.ResetColor();
        }
        private void DrawUntouchedCell()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("   ");
            Console.ResetColor();
        }
        private void DrawSeaCell()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("   ");
            Console.ResetColor();
        }
        private void DrawShip(Ship ship, string cell)
        {
            if (ship.Lifes == 0)
            {
                DrawDestroyedCell();
            }
            else if (ship.Injuries.Contains(cell))
            {
                DrawHitCell();
            }
            else
            {
                if (this.NoOfHumanPlayers == 1)
                    DrawUntouchedCell();
                else
                    DrawSeaCell();
            }
        }
        private void DrawCell(IPlayer player, char row, int column)
        {
            bool alreadyDrawn = false;
            string cell = row + column.ToString();

            foreach (Ship ship in player.shipsOwned.Where(val => val != null))
            {
                if (Array.IndexOf(ship.Coordinates, cell) != -1)
                {
                    DrawShip(ship, cell);
                    alreadyDrawn = true;
                }
            }
            if (!alreadyDrawn)
            {
                DrawSeaCell();
            }
        }
        private void DrawRow(IPlayer player, char row)
        {
            Console.Write($" {row} ");
            for (int column = 0; column < 10; column++)
            {
                DrawCell(player, row, column);
            }
            Console.Write("\n");
        }
        public void DrawBoard(IPlayer player)
        {
            Console.WriteLine($"Your ships, General {player.Name}:");
            Console.WriteLine("    1  2  3  4  5  6  7  8  9  10 ");
            foreach (char row in Ship.PossibleRows)
            {
                DrawRow(player, row);
            }
        }
        public void DrawOpponentBoard(IPlayer player, IPlayer opponent) 
        {
            Console.WriteLine("\nChoose your next target:");
            Console.WriteLine("    1  2  3  4  5  6  7  8  9  10 ");

            foreach (char row in Ship.PossibleRows)
            {
                Console.Write($" {row} ");
                for (int column = 0; column < 10; column++)
                {
                    string cell = row + column.ToString();
                    if (player.hits.Contains(cell))
                    {
                        foreach (Ship ship in opponent.shipsOwned)
                        {
                            if (Array.IndexOf(ship.Coordinates, cell) != -1)
                            {
                                if (ship.Lifes == 0)
                                {
                                    DrawDestroyedCell();
                                }
                                else
                                {
                                    DrawHitCell();
                                }
                            }
                        }
                    }
                    else if (player.misses.Contains(cell))
                    {
                        DrawUntouchedCell();
                    }
                    else
                    {
                        DrawSeaCell();
                    }
                }
                Console.Write("\n");
            }
        }

    }
}
