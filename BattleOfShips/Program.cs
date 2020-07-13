using System;

namespace BattleOfShips
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.InitializeGame();
            Console.ReadLine();
        }
    }
}
