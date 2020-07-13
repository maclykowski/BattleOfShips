using System;

namespace BattleOfShips
{
    public class Game              
    {
        public int NoOfPlayers;
        private void SetGameType()
        {
            Console.Write("To fight against Computer - type '1', to fight a 2 player battle - type '2': ");
            var noOfPlayers = Console.ReadLine(); 
            int noOfPlayersInt;
            while (!Int32.TryParse(noOfPlayers, out noOfPlayersInt) || noOfPlayersInt < 1 || noOfPlayersInt > 2)
            {
                Console.WriteLine("Error! - type '1' or '2'!");
                Console.Write("To fight against Computer - type '1', to fight a 2 player battle - type '2': ");
                noOfPlayers = Console.ReadLine();
            } 
            NoOfPlayers = noOfPlayersInt;
        }
        private string SetPlayerName()
        {
            Console.WriteLine("Enter your name, General.");
            Console.Write("My name is: ");
            string playerName = Console.ReadLine();
            while (String.IsNullOrWhiteSpace(playerName)) 
            {
                Console.Clear();
                Console.WriteLine("Error!");
                Console.WriteLine("Enter your name, General.");
                Console.Write("My name is: ");
                playerName = Console.ReadLine();
            }
            return playerName;
        }
        private void HandleSinglePlayerGame()
        {
            var p1 = new Player(SetPlayerName()); 
            var p2 = new Cpu(); 
            Console.Clear();
            Play1PlayerGame(p1, p2);
        }
        private void HandleMultiPlayerGame()
        {
            Console.Clear();
            Console.WriteLine("PLAYER 1");
            string player1Name = SetPlayerName();
            Console.Clear();
            Console.WriteLine("PLAYER 2");
            string player2Name = SetPlayerName();
            Console.Clear();
            Console.WriteLine($"Press ENTER to arrange your ships, General {player1Name}. General {player2Name} do not peek!");
            Console.ReadLine();
            Console.Clear();
            var p1 = new Player(player1Name);
            Console.WriteLine("\nPress ENTER to let your opponent arrange the ships.");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"Press ENTER to arrange your ships, General {player2Name}. General {player1Name} do not peek!"); 
            Console.ReadLine();
            Console.Clear();
            var p2 = new Player(player2Name);
            Console.WriteLine($"\nPress ENTER to start the battle of ships!");
            Console.ReadLine();
            Console.Clear();
            Play2PlayerGame(p1, p2);
        }
        public void InitializeGame()
        {
            Console.WriteLine("It's time to begin the BATTLE OF SHIPS!");
            SetGameType(); //sets NoOfPlayers (humans)
            if (NoOfPlayers == 1)
            {
                HandleSinglePlayerGame();
            }
            else if (NoOfPlayers == 2) 
            {
                HandleMultiPlayerGame();
            }
        }

        public void ShowSinglePlayerBattleMap(Player player, Cpu opponent)
        {
            var b = new Board(1);
            Console.WriteLine($"General {player.Name}, it is your turn.");
            b.DrawBoard(player);
            b.DrawOpponentBoard(player, opponent);
        }
        public void ShowMultiPlayerBattleMap(Player player, Player opponent)
        {
            var b = new Board(2);
            Console.WriteLine($"General {player.Name}, it is your turn.");
            b.DrawBoard(player);
            b.DrawOpponentBoard(player, opponent);
        }

        public void Play1PlayerGame(Player player, Cpu opponent)
        {
            while(true) 
            { 
                Console.Clear();
                ShowSinglePlayerBattleMap(player, opponent);
                while (player.FireAMissile(opponent))
                {
                    if (opponent.IsDefeated())
                    {
                        Console.WriteLine($"General {player.Name} won the battle.");
                        Console.WriteLine("Press ENTER to EXIT game.");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    Console.WriteLine("Press ENTER to shoot another missile.");
                    Console.ReadLine();
                    Console.Clear();
                    ShowSinglePlayerBattleMap(player, opponent);
                }
                Console.WriteLine("Press ENTER to let your opponent do the job.");
                Console.ReadLine();
                while (opponent.FireAMissile(player))
                {
                    if (player.IsDefeated())
                    {
                        Console.WriteLine($"\nComputer General won the battle.");
                        Console.WriteLine("Press ENTER to EXIT game.");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    Console.Clear();
                    Console.WriteLine("Computer General hit you.");
                    Console.WriteLine($"Look at your losses.");
                    var b = new Board(1);
                    b.DrawBoard(player);
                    Console.WriteLine("Press ENTER to let Computer General do another shot.");
                    Console.ReadLine();
                }
                Console.WriteLine("Computer General misses. Press ENTER to aim the missiles against him.");
                Console.ReadLine();
            }
        }
        public void Play2PlayerGame(Player player, Player opponent)
        {
            Console.Clear();
            ShowMultiPlayerBattleMap(player, opponent);
            while (player.FireAMissile(opponent))
            {
                if (opponent.IsDefeated())
                {
                    Console.WriteLine($"\nGeneral {player.Name} won the battle.");
                    Console.WriteLine("Press ENTER to EXIT game.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                Console.WriteLine("Press ENTER to shoot another missile.");
                Console.ReadLine();
                Console.Clear();
                ShowMultiPlayerBattleMap(player, opponent);
            }
            Console.WriteLine("Press ENTER to let your opponent do the job.");
            Console.ReadLine();
            Play2PlayerGame(opponent, player);
        }
    }
}
