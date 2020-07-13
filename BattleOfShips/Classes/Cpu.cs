using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleOfShips
{
    public class Cpu : IPlayer
    {
        public string Name { get; set; }
        public Ship[] shipsOwned { get; set; }
        public Stack<string> hits { get; set; }
        public Stack<string> misses { get; set; }

        public List<string> currentShipToDestroy;
        public Cpu()
        {
            Name = "Computer";
            shipsOwned = new Ship[5];
            hits = new Stack<string>();
            misses = new Stack<string>();
            SetFleetPosition();
            currentShipToDestroy = new List<string>();
            Console.Clear();
            Console.WriteLine("Ships managed by computer are ready to fight.");
            Console.WriteLine("Press ENTER");
            Console.ReadLine();
        }

        public void SetFleetPosition()
        {
            for (int i = 0; i < 5; i++)
            {
                int numOfMasts;
                if (i < 2)
                {
                   numOfMasts = i + 2;
                }
                else
                {
                    numOfMasts = i + 1;
                }
                Random rand = new Random();
                char rowRand = Ship.PossibleRows[rand.Next(10)];
                int columnRand = rand.Next(10);
                bool horizontalPositionRand = rand.NextDouble() >= 0.5;
                if (Ship.SetShipPosition(shipsOwned, numOfMasts, rowRand, columnRand, horizontalPositionRand, out string[] cordinatesArr))
                {
                    shipsOwned[i] = new Ship(cordinatesArr);
                }
                else
                {
                    i--;
                    continue;
                }
            }
        }

        public bool FireAMissile(IPlayer opponent) 
        {
            string aim;
            //Aim the missiles and choose your target
            while (true)
            {
                aim = ChooseTarget();
                if (!hits.Contains(aim) && !misses.Contains(aim))
                {
                    //Computer did not shoot a missile at this aim
                    break;
                }
            }
            if (opponent.BeenHit(aim))
            {
                hits.Push(aim);
                currentShipToDestroy.Add(aim);
                currentShipToDestroy.Sort();
                foreach (Ship ship in opponent.shipsOwned)
                {
                    if (ship.IsDestroyed() && currentShipToDestroy.Count > 1)
                    {
                        if (ship.Coordinates.Contains(currentShipToDestroy[0]))
                        {
                            int column = Int32.Parse(aim.Substring(1));
                            foreach (string shipPosition in ship.Coordinates)
                            {
                                char shipRow = Char.Parse(shipPosition.Substring(0, 1));
                                int shipColumn = Int32.Parse(shipPosition.Substring(1));
                                int shipRowIndex = Array.IndexOf(Ship.PossibleRows, shipRow);
                                if (shipColumn != 0 && !misses.Contains(shipRow.ToString() + (column - 1)) && !hits.Contains(shipRow.ToString() + (column - 1))) //adding misses around the ship, to avoid wrong future targets
                                {
                                    misses.Push(shipRow.ToString() + (column - 1));
                                }
                                if (shipColumn != 9 && !misses.Contains(shipRow.ToString() + (column + 1)) && !hits.Contains(shipRow.ToString() + (column + 1)))
                                {
                                    misses.Push(shipRow.ToString() + (column + 1));
                                }
                                if (shipRow != 'a')
                                {
                                    if (!misses.Contains(Ship.PossibleRows[(shipRowIndex - 1)].ToString() + column) && !hits.Contains(Ship.PossibleRows[(shipRowIndex - 1)].ToString() + column))
                                    {
                                        misses.Push(Ship.PossibleRows[(shipRowIndex - 1)].ToString() + column);
                                    }
                                }
                                if (shipRow != 'j')
                                {
                                    if (!misses.Contains(Ship.PossibleRows[(shipRowIndex + 1)].ToString() + column) && !hits.Contains(Ship.PossibleRows[(shipRowIndex + 1)].ToString() + column))
                                    {
                                        misses.Push(Ship.PossibleRows[(shipRowIndex + 1)].ToString() + column);
                                    }
                                }
                            }
                            currentShipToDestroy.Clear();
                        }
                    }
                }
                return true;
            }
            //Shoot missed
            misses.Push(aim);
            return false;
        }

        private string TargetFromFourClosestCells() //will try to find another part of ship in the four closest cells to the first hit
        {
            Random rand = new Random();
            while(true)
            {
                int shipToDestroyRow = Array.IndexOf(Ship.PossibleRows, Char.Parse(currentShipToDestroy[0].Substring(0, 1)));
                int shipToDestroyColumn = Int32.Parse(currentShipToDestroy[0].Substring(1));
                string aimLeftBorder = null,
                       aimUpperBorder = null,
                       aimRightBorder = null,
                       aimBottomBorder = null;
                if (shipToDestroyColumn != 0)
                {
                    aimLeftBorder = Ship.PossibleRows[shipToDestroyRow].ToString() + (shipToDestroyColumn - 1);
                }
                if (shipToDestroyRow != 0) //row != 'a'
                {
                    aimUpperBorder = Ship.PossibleRows[shipToDestroyRow - 1].ToString() + shipToDestroyColumn;
                }
                if (shipToDestroyColumn != 9)
                {
                    aimRightBorder = Ship.PossibleRows[shipToDestroyRow].ToString() + (shipToDestroyColumn + 1);
                }
                if (shipToDestroyRow != 9) //row != 'j'
                {
                    aimBottomBorder = Ship.PossibleRows[shipToDestroyRow + 1].ToString() + (shipToDestroyColumn);
                }
                string[] cellsToCheck = { aimLeftBorder, aimUpperBorder, aimRightBorder, aimBottomBorder };
                string target = cellsToCheck[rand.Next(4)];
                if (target != null)
                {
                    return target;
                }
            }
        }
        private string TargetHorizontalShip(int indexOfLastElement)
        {
            char row = Char.Parse(currentShipToDestroy[indexOfLastElement].Substring(0, 1));
            int column = Int32.Parse(currentShipToDestroy[indexOfLastElement].Substring(1)) + 1;
            if (misses.Contains(row.ToString() + column) || column > 9)
            {
                column = Int32.Parse(currentShipToDestroy[0].Substring(1)) - 1;
            }
            return row + column.ToString();
        }
        private string TargetVerticalShip(int indexOfLastElement)
        {
            char row;
            char[] rows = Ship.PossibleRows;
            char rowTemp = Char.Parse(currentShipToDestroy[indexOfLastElement].Substring(0, 1));
            int column = Int32.Parse(currentShipToDestroy[indexOfLastElement].Substring(1));
            if (Array.IndexOf(rows, rowTemp) + 1 < rows.Length)
            {
                if (!misses.Contains(rows[Array.IndexOf(rows, rowTemp) + 1].ToString() + column))
                {
                    row = rows[Array.IndexOf(rows, rowTemp) + 1];
                }
                else
                {
                    row = rows[Array.IndexOf(rows, Char.Parse(currentShipToDestroy[0].Substring(0, 1))) - 1];
                }
            }
            else
            {
                row = rows[Array.IndexOf(rows, Char.Parse(currentShipToDestroy[0].Substring(0, 1))) - 1];
            }
            return row + column.ToString();
        }
        private string TargetCurrentlyAttackedShip()
        {
            char[] rows = Ship.PossibleRows;
            currentShipToDestroy.Sort();
            int indexOfLastElement = currentShipToDestroy.Count - 1;
            bool isCurrentShipHorizontal = false;
            if (currentShipToDestroy[0].Substring(0, 1) == currentShipToDestroy[indexOfLastElement].Substring(0, 1))
                isCurrentShipHorizontal = true;
            if (isCurrentShipHorizontal)
            {
                return TargetHorizontalShip(indexOfLastElement);
            }
            else
            { 
                return TargetVerticalShip(indexOfLastElement);
            }
        }
        public string ChooseTarget()
        {
            Random rand = new Random();
            if (currentShipToDestroy.Count == 0) //choose random target if there is no ship currently attacked 
            {
                char row = Ship.PossibleRows[rand.Next(10)];
                int column = rand.Next(10);
                return row + column.ToString();
            }
            else if (currentShipToDestroy.Count == 1) //if ship that is under attack got only one hit 
            {
                return TargetFromFourClosestCells(); 
            }
            else //if ship that is under attack got more than 1 hits
            {
                return TargetCurrentlyAttackedShip();
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
                    if (ship.IsDestroyed())
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
                if (ship.Lifes == 0)
                {
                    destroyedShipsCounter++;
                }
            }
            if (destroyedShipsCounter == 5)
            {
                return true;
            }
            return false;
        }
    }
}
