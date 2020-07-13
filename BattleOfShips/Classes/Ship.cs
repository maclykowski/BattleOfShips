using System;
using System.Collections.Generic;

namespace BattleOfShips
{
    public class Ship
    {
        private int _lifes; //lifes == numberOfMasts
        public int Lifes
        {
            get => _lifes;
            set => _lifes = value; 
        }
        private string[] _coordinates; 
        public string[] Coordinates
        {
            get => _coordinates;
            set { _coordinates = value; }
        }
        private Stack<string> _injuries = new Stack<string>(); 
        public Stack<string> Injuries
        {
            get => _injuries;
            set { _injuries = value; }
        }
        
        private static char[] _possibleRows = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
        public static char[] PossibleRows { get => _possibleRows; }


        public Ship(string[] coordinates)
        {
            Lifes = coordinates.Length;
            Coordinates = new string[Lifes];
            coordinates.CopyTo(Coordinates, 0);
        }

        public bool IsDestroyed()
        {
            if (Lifes == 0)
            {
                return true;
            }
            return false;
        }

        private static bool IsNewShipNotOverTheBoard(int numberOfMasts, char row, int column, bool isHorizontal, string[] coordinates)
        {
            int indexOfRow = Array.IndexOf(Ship.PossibleRows, Char.ToLower(row));
            if (indexOfRow == -1 || column < 0 || column > 9)
            {
                Console.WriteLine("You can't place a ship over the board.");
                return false;
            }
            else if (indexOfRow != -1 || column >= 0 || column <= 9)
            {
                int coordinatesIndex = 0;
                if (isHorizontal)
                {
                    if (column + numberOfMasts - 1 < 10)
                    {
                        while (coordinatesIndex < numberOfMasts)
                        {
                            coordinates[coordinatesIndex] = Ship.PossibleRows.GetValue(indexOfRow) + column.ToString();
                            coordinatesIndex++;
                            column++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Part of horizontaly placed ship is over the board.");
                        return false;
                    }
                }
                else
                {
                    int currentIndexOfRow = indexOfRow;
                    while (currentIndexOfRow <= indexOfRow + numberOfMasts - 1)
                    {
                        if (currentIndexOfRow < 10)
                        {
                            coordinates[coordinatesIndex] = Ship.PossibleRows.GetValue(currentIndexOfRow) + column.ToString();
                        }
                        else
                        {
                            Console.WriteLine("Part of verticaly placed ship is over the board.");
                            return false;
                        }
                        coordinatesIndex++;
                        currentIndexOfRow++;
                    }
                }
            }
            return true;
        }
        private static bool IsNewShipNotOverlapingCurrentShips(Ship[] ownedShips, string[] coordinates)
        {
            foreach (Ship ship in ownedShips)
            {
                if (ship != null)
                {
                    foreach (string shipPosition in ship.Coordinates)
                    {
                        if (Array.IndexOf(coordinates, shipPosition) != -1)
                        {
                            Console.WriteLine("You already have a ship on this position.");
                            return false;
                        }
                    }
                    foreach (string shipPosition in ship.Coordinates)
                    {
                        char shipRow = Char.Parse(shipPosition.Substring(0, 1));
                        int shipColumn = Int32.Parse(shipPosition.Substring(1));
                        int shipRowIndex = Array.IndexOf(PossibleRows, shipRow);
                        if ((shipColumn != 0 && Array.IndexOf(coordinates, shipRow.ToString() + (shipColumn - 1)) != -1)) //check free space near left broadside
                        {
                            //RestrictedArea:
                            Console.WriteLine("Too close to left broadside");
                            return false;
                        }
                        if ((shipColumn != 9 && Array.IndexOf(coordinates, shipRow.ToString() + (shipColumn + 1)) != -1)) //check free space near right broadside
                        {
                            //RestrictedArea:
                            Console.WriteLine("Too close to right broadside");
                            return false;
                        }
                        if ((shipRow != 'a' && Array.IndexOf(coordinates, PossibleRows[shipRowIndex - 1].ToString() + shipColumn) != -1)) //check free space near upper broadside
                        {
                            //RestrictedArea:
                            Console.WriteLine("Too close to upper broadside");
                            return false;
                        }
                        if ((shipRow != 'j' && Array.IndexOf(coordinates, PossibleRows[shipRowIndex + 1].ToString() + shipColumn) != -1)) //check free space near upper broadside
                        {
                            //RestrictedArea:
                            Console.WriteLine("Too close to down broadside");
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public static bool ValidateNewShipPosition(Ship[] ownedShips, int numberOfMasts, char row, int column, bool isHorizontal, string[] coordinates)
        {
            return (IsNewShipNotOverTheBoard(numberOfMasts, row, column, isHorizontal, coordinates) && IsNewShipNotOverlapingCurrentShips(ownedShips, coordinates));
        }
        public static bool SetShipPosition(Ship[] ownedShips, int numberOfMasts, char row, int column, bool isHorizontal, out string[] coordinates)
        {
            coordinates = new string[numberOfMasts];
            if (numberOfMasts < 2 || numberOfMasts > 5) //ship is too small or too big
            {
                Console.WriteLine("Your fleet can have only from 2- to 5-masts ships.");
                return false;
            }
            return Ship.ValidateNewShipPosition(ownedShips, numberOfMasts, row, column, isHorizontal, coordinates);
        }

    }
}
