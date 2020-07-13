using System;
using System.Collections.Generic;

namespace BattleOfShips
{
    public class Player: IPlayer 
    {
        public string Name { get; set; }
        public Ship[] shipsOwned { get; set; }
        public Stack<string> hits { get; set; } 
        public Stack<string> misses { get; set; } 

        public Player(string name)
        {
            Name = name;
            shipsOwned = new Ship[5];
            hits = new Stack<string>();
            misses = new Stack<string>();
            SetFleetPosition();
        }
        private char SetRow()
        {
            Console.Write("Row (a-j): "); 
            var row = Console.ReadLine();
            char rowChar;
            while (!char.TryParse(row, out rowChar) || Array.IndexOf(Ship.PossibleRows, rowChar) == -1)
            {
                Console.WriteLine("A row value have to be a single letter from 'a' to 'j'!");
                Console.Write("Row (a-j): ");
                row = Console.ReadLine();
            }
            return rowChar;
        }
        private int SetColumn()
        {
            Console.Write("Column (1-10): ");
            var column = Console.ReadLine();
            int columnInt;
            while (!Int32.TryParse(column, out columnInt) || columnInt < 1 || columnInt > 10)
            {
                Console.WriteLine("A column value have to be a number from 1 to 10!");
                Console.Write("Column (1-10): ");
                column = Console.ReadLine();
            }
            return columnInt;
        }
        private char SetHorizontalOrVertical()
        {
            Console.Write("Horizontal (h) or vertical (v): ");
            var isHorizontalVar = Console.ReadLine();
            char isHorizontalChar;
            while (!char.TryParse(isHorizontalVar, out isHorizontalChar) || !(isHorizontalChar.Equals('h') || isHorizontalChar.Equals('v')))
            {
                Console.WriteLine("Type 'h' for horizontal or 'v' for vertical position!");
                Console.Write("Horizontal (h) or vertical (v): ");
                isHorizontalVar = Console.ReadLine();
            }
            return isHorizontalChar;
        }
        private bool SetShip(int i, Board b) //i is a current ships counter
        {
            int numOfMasts;
            Console.Clear();
            Console.WriteLine($"Map with positions of your ships, General {Name}.\n");
            b.DrawBoard(this);
            Console.WriteLine("\n" + (5 - i) + " ships left to distribute.");
            if (i < 2)
                numOfMasts = i + 2;
            else
                numOfMasts = i + 1;
            Console.WriteLine($"\nMove the {numOfMasts}-mast ship into position!");
            char row = SetRow();
            int column = SetColumn();
            char isHorizontal = SetHorizontalOrVertical();
            if (Ship.SetShipPosition(shipsOwned, numOfMasts, row, column - 1, isHorizontal.Equals('h'), out string[] cordinatesArr))
            {
                this.shipsOwned[i] = new Ship(cordinatesArr);
                Console.WriteLine($"The {numOfMasts}-mast ship is in position and ready to fight!\n");
                return true;
            }
            else
            {
                Console.WriteLine("An error occurred while entering the ship's coordinates. Press ENTER to place this ship again.");
                Console.ReadLine();
                return false;
            }
        }
        public void SetFleetPosition()
        {
            Console.WriteLine($"You have to move your fleet into positions, General {Name}!");
            Console.WriteLine("Arrange your ships. (Press ENTER)");
            Console.ReadLine();
            var b = new Board(1);
            for(int i = 0; i < 5; i++) 
            {
                if (!SetShip(i, b))
                    i--;
            } 
            Console.Clear();
            WriteAllShipsCoordinates();
            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();
        }
        public void WriteAllShipsCoordinates()
        {
            Console.WriteLine($"You have arranged all your ships, General {Name}.");
            Console.Write("Coordinates of your ships: ");
            foreach (Ship ship in shipsOwned)
            {
                if (ship != null)
                {
                    Console.Write("[{0}]", string.Join(", ", ship.Coordinates));
                    Console.Write("; ");
                }
            }
            Console.Clear();
            Console.WriteLine($"Map with positions of all your ships, General {Name}.\n");
            var b = new Board(1);
            b.DrawBoard(this);
        }
        public bool FireAMissile(IPlayer opponent)
        {
            Console.WriteLine($"Aim the missiles and choose your target, General {Name}.");
            while (true)
            {
                char row = SetRow();
                int column = SetColumn();
                string aim = row.ToString() + (column - 1);
                if (!hits.Contains(aim) && !misses.Contains(aim))
                {
                    if (opponent.BeenHit(aim))
                    {
                        Console.WriteLine($"Great shoot, General {Name}.");
                        hits.Push(aim);
                        return true;
                    }
                    Console.WriteLine($"You missed, General {Name}.");
                    misses.Push(aim);
                    return false;
                }
                Console.WriteLine("You have already shoot a missile at this aim! Choose your target again.");
            }
        }

        public bool BeenHit(string aim)
        {
            foreach (Ship ship in shipsOwned)
            {
                if (Array.IndexOf(ship.Coordinates, aim) != -1)
                {
                    ship.Lifes--;
                    ship.Injuries.Push(aim);
                    if(ship.IsDestroyed())
                    {
                        Console.WriteLine($"The {ship.Coordinates.Length}-masts ship is destroyed! \n");
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsDefeated() 
        {
            int destroyedShipsCounter = 0;
            foreach (Ship ship in shipsOwned)
            {
                if(ship.Lifes == 0)
                {
                    destroyedShipsCounter++;
                }
            }
            if(destroyedShipsCounter == 5)
            {
                return true;
            }
            return false;
        }


    }
}
