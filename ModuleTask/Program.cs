using System;
using System.Collections.Generic;



namespace ModuleTask
{
    class Program
    {
        static CardDesk cardDesk = new CardDesk(CardDesk.Basic.basic);
        static Dictionary<Card.names, int> Points = new Dictionary<Card.names, int> {
                { Card.names.Ace, 11 },
                { Card.names.Jack, 2 },
                { Card.names.Quin, 3 },
                { Card.names.King, 4 }
            };
        static Game game = new Game(ref cardDesk, Points);

        /// <summary>
        /// globally Created instances of classes:
        /// CardDesk cardDesk - for managing a playing deck
        ///Dictionary<Card.names, int> Points - pairs connecting the card and its cost.
        ///Game game - for storing players and managing the game process
        ///We offer the user the choice to download the game or start a new one.
        ///If we start a new one, we initialize the instance of the class of the game, 
        ///creating new players and transferring the game deck.In the end of the game cycle, 
        ///it is suggested to start a new round:
        ///All players return to the table and hand out cards from the new deck.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "Black Jeck";
            Console.WriteLine("Do you wan't to load the game? (Yes - \"y/Y\", No - \"n/N\").");
            for (bool flag = false; !flag;)
            {
                switch (Console.ReadLine().ToUpper())
                {
                    case "Y":
                        Console.WriteLine("Enter the file name");
                        string fileName = Console.ReadLine();
                        try
                        {
                            game = game.readOfFile(fileName);
                            cardDesk = game.Desk;
                            cardDesk.MixDesk();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Entered incorrect name. Repeat please: ");
                            continue;
                        }
                        flag = true;
                        break;
                    case "N":
                        InitGame();
                        cardDesk.MixDesk();
                        game.StartPut(2); //put two cards
                        flag = true;
                        break;
                    default:
                        Console.WriteLine("Entered incorrect value. Repeat please: ");
                        break;
                }
            }
           
            while (true) //Gaming loop
            {
                ProcessGame();
                
                if (IsContonue())
                {
                    game.RemoveHands();
                    game.RemovePoints();
                    game.ActivGamers();
                    cardDesk = new CardDesk(CardDesk.Basic.basic);
                    cardDesk.MixDesk();
                    game.SetNewDesc(ref cardDesk);
                    game.StartPut(2); //put two cards
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Name - Win");
                    Console.WriteLine(game.WinnersList());
                    Console.WriteLine();
                    Console.WriteLine("Click \"Enter\" to exit");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Initializes the class Game, sets the number of players,
        /// dialog whis user and creates a table.
        /// </summary>
        public static void InitGame()
        {
            //Variables
            uint gamer = 0;
            uint bot = 0;

            //DIALOG WITH THE USER
            for (uint sum = 0; ; )
            {
                Console.WriteLine("How many bots play");
                for (; !UInt32.TryParse(Console.ReadLine(), out bot);)
                {
                    Console.WriteLine("Entered incorrect value. Repeat please: ");
                }

                Console.WriteLine("How many people play");
                for (; !UInt32.TryParse(Console.ReadLine(), out gamer);)
                {
                    Console.WriteLine("Entered incorrect value. Repeat please: ");
                }
                sum = gamer + bot;
                if (sum > (int)CardDesk.Basic.basic / 2)
                {
                    Console.WriteLine("Entered incorrect value.\nThe total number of player must be less than 18.\nPlease try again: ");
                    continue;
                }
                else break;
            }

            game.InitTable(bot, gamer);
        }

        /// <summary>
        /// This is the compleate cycle of one game process:
        /// Start and find the winner and show them.
        /// </summary>
        public static void ProcessGame()
        {
            game.Gaming();

            var wins = game.TakeWinner();
            if (wins.Count > 0)
                Console.WriteLine($"Win combo: {wins[0].points}");
            foreach (var item in wins)
            {
                Console.WriteLine($"{item.SeriesWin} Win {item.Name}:");
                item.DisplayHand();

                Console.WriteLine();
            }
            Console.WriteLine("Click \"Enter\" to continue");
            Console.ReadLine();
        }

        /// <summary>
        /// This is the final dialog with and user input.
        /// </summary>
        /// <returns>true if the player continues to game, and false if he refuses</returns>
        public static bool IsContonue()
        {
            Console.Clear();
            Console.WriteLine("Do you want to continue? (Yes - \"y/Y\", No(Exit) - \"n/N\").");
            for (;;)
            {
                switch (Console.ReadLine().ToUpper())
                {
                    case "Y":
                        return true;
                    case "N":
                        return false;
                    default:
                        Console.WriteLine("Entered incorrect value. Repeat please: ");
                        break;
                }
            }
        }
    }
}
