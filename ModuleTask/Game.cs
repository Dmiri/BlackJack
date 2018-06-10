using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ModuleTask;
using System.Runtime.Serialization.Formatters.Binary;

namespace ModuleTask
{
    [Serializable]
    class Game
    {
        public Game(ref CardDesk cardDesk, Dictionary<Card.names, int> points)
        {
            this.Points = points;
            this.cardDesk = cardDesk;
        }

        private /*readonly*/ CardDesk cardDesk;
        private readonly Dictionary<Card.names, int> Points;
        private List<Gambler> gamers = new List<Gambler>();//must by private
        private List<Card> lookCards = new List<Card>();
        private int CountActiviti;
        private uint countPeoples;
        private uint countBbots;

        //Variables
        public uint CountPeoples => countPeoples;
        public uint CountBbots => countBbots;
        public CardDesk Desk => cardDesk;

        public int GamblerIt {get; set;}
        public List<Card> LookCards
        {
            get
            {
                return lookCards;
            }
            private set { }
        }

        #region Initialization and operation with the players
        public int CountGamers()
        {
            return gamers.Count;
        }

        public void RemoveHands()
        {
            foreach (var item in gamers)
            {
                item.RemoveHand();
            }
        }

        public void RemoveHand(Gambler player)
        {
           player.RemoveHand();
        }

        public void RemovePoints()
        {
            foreach (var item in gamers)
            {
                item.points = 0;
            }
        }

        public void RemovePoint(Gambler player)
        {
            player.points = 0;
        }

        public void InitTable(uint bot, uint people)
        {
            countBbots = bot;
            countPeoples = people;
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] rand = new byte[bot + people];
            for (int countBots = 0, countPeople = 0; countBots + countPeople < bot + people;)
            {
                rngCsp.GetBytes(rand);
                int isAdd = rand[0] % 2;
                if (countBots < bot && isAdd == 0)
                {
                    gamers.Add(new Bot());
                    ++countBots;
                }
                if (countPeople < people && isAdd == 1)
                {
                    gamers.Add(new Gambler());
                    ++countPeople;
                }
            }
        }

        public void PutGambler(Gambler gambler)
        {
            if (gambler is Bot) ++countBbots;
            else ++countPeoples;
            gamers.Add(gambler);
        }

        public void RemoveGambler(Gambler gambler)
        {
            if (gambler is Bot) --countBbots;
            else --countPeoples;
            gamers.Remove(gambler);
        }
    
        public int ActivGamers()
        {
            CountActiviti = gamers.Count;
            foreach (var item in gamers)
            {
                item.status = Gambler.Status.Activiti;
            }
            return CountActiviti;
        }

        private void ActivGamer(Gambler gambler)
        {
            if (gambler.status != Gambler.Status.Activiti)
            {
                gambler.status = Gambler.Status.Activiti;
                ++CountActiviti;
            }
        }

        private void InactivGamer(Gambler gambler)
        {
            if (gambler.status != Gambler.Status.Unactiviti)
            {
                gambler.status = Gambler.Status.Unactiviti;
                --CountActiviti;
            }
        }

        #endregion

        #region Initialization and operation with the cards and desc
        public void StartPut(uint count = 1)
        {
            for (int i = 0; i < gamers.Count; i++)
            {
                for (int it = 0; it < count; it++)
                {
                    var tempCard = cardDesk.TakeUpCard();
                    int point;
                    if (Points.TryGetValue(tempCard.name, out point)) ;
                    else point = (int)tempCard.name;

                    gamers[i].TakeCard(tempCard, point);
                }
            }
        }

        public void SetNewDesc(ref CardDesk desk)
        {
            cardDesk = desk;
        }

        #endregion


        #region Operations with the cards seen
        public void AddLookedCard(Card card)
        {
            lookCards.Add(card);
        }
        public void DisplayLookCard()
        {
            foreach (var item in lookCards)
            {
                Console.WriteLine($"Suit = {item.suit} Nane = {item.name} Color = {item.color}");
            }
        }
        #endregion

        #region Dialogue with player and display
        public bool ActionPlayer()
        {
            bool take = false;
            for (bool flag = true; flag;)
            {
                Console.WriteLine("Will you take a card? (Yes - \"y/Y\", No - \"n/N\").\n" +
                    "Save - \"s/S\", Quit - \"q/Q\"");
                string gamer = Console.ReadLine().ToUpper();

                switch (gamer)
                {
                    case "Y":
                        take = true;
                        break;
                    case "N":
                        take = false;
                        break;
                    case "S":
                        this.saveToFile($"Data.dat");
                        take = ActionPlayer();
                        break;
                    case "Q":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Entered incorrect value. Repeat please: ");
                        continue;
                        //throw new System.ArgumentException();
                }
                flag = false;
            }
            return take;
        }

        public string WinnersList()
        {
            string str = "";
            foreach (var item in gamers)
            {
                str += $"{item.Name} - {item.SeriesWin}\n";
            }
            return str;
        }
        #endregion

        #region Gaming process
        public void Gaming()
        {
            this.CountActiviti = ActivGamers();
            for (int gamblerIt = 0; gamblerIt < gamers.Count; gamblerIt++)
            {
                if (gamers[gamblerIt] is Bot && 
                    String.IsNullOrEmpty(gamers[gamblerIt].Name))
                    gamers[gamblerIt].Name = $"Bot {gamblerIt + 1}";
            }

            for (int count = 1; count > 0 && CountActiviti > 1;) //if there are more one acive gamer
            {
                count = 0;

                foreach (var item in gamers)
                {
                    if (item.points == 21) return;
                    if (item.countHand() == 2 &&
                        item.points == 22)
                        return;
                }
                for (int gamblerIt = 0; gamblerIt < gamers.Count; gamblerIt++)
                {
                    if (cardDesk.Length() == 0) return;

                    #region dialogue with the player
                    Console.Clear();
                    Console.WriteLine($"Ace - 11 or 1, Jack - 2, Quin - 3, King - 4");
                    string hendler = $"Wait for the next player {gamblerIt + 1}";

                    if (!(gamers[gamblerIt] is Bot) && 
                        String.IsNullOrEmpty(gamers[gamblerIt].Name))
                    {
                        Console.WriteLine("Enter the name of the player: ");
                        gamers[gamblerIt].Name = Console.ReadLine();
                    }
                    hendler += $" - {gamers[gamblerIt].Name}";
                    Console.WriteLine(hendler);

                    if (!(gamers[gamblerIt] is Bot))
                    {
                        Console.WriteLine("Click \"Enter\" to continue");
                        Console.ReadLine();
                    }

                    if (!(gamers[gamblerIt] is Bot)) gamers[gamblerIt].DisplayHand();

                    #endregion

                    if (gamers[gamblerIt].status == Gambler.Status.Activiti)
                    {
                        bool take = false;
                        int sumPoints = 0;
                        if (gamers[gamblerIt] is Bot)
                        {
                            take = ((Bot)gamers[gamblerIt]).RunBotMeditate();
                        }
                        else
                        {
                            take = ActionPlayer();//mast be run event handler for Gambler
                        }
                        if (take)
                        {
                            ++count;
                            int point = 0;
                            var tempCard = cardDesk.TakeUpCard();
                            if (tempCard.name == Card.names.Ace && gamers[gamblerIt].points >= 10)
                                point = 1;
                            else
                            {
                                if (Points.TryGetValue(tempCard.name, out point)) ;
                                else point = (int)tempCard.name;
                            }

                            if (!(gamers[gamblerIt] is Bot)) tempCard.Display();
                            sumPoints = gamers[gamblerIt].TakeCard(tempCard, point);
                        }
                        if (sumPoints == 21) return;
                        if (gamers[gamblerIt].points > 21)
                            InactivGamer(gamers[gamblerIt]);
                    }
                    if (!(gamers[gamblerIt] is Bot)) Console.WriteLine($"Points {gamers[gamblerIt].points}");
                    Console.WriteLine("Click \"Enter\" to continue");
                    Console.ReadLine();
                }
            }
        }
        #endregion

        #region Who his won?
        public List<Gambler> TakeWinner()
        {
            List<Gambler> wins = new List<Gambler>(); //List - if the winners are more than one.
            int WinPointTo = 0;
            int WinPointAfter = 100;//- it's magic value (becouse over 21)

            #region search for the wining combination
            if (gamers.Count == 1)
            {
                ++gamers[0].SeriesWin;
                return wins = gamers;
            }
            foreach (var item in gamers)//if 2 Ace
            {
                //search for the maximum point to 21 
                if (item.points > WinPointTo && item.status == Gambler.Status.Activiti)
                    WinPointTo = item.points;
                //search for the maximum point after 21 
                if (item.points < WinPointAfter && item.status == Gambler.Status.Unactiviti
                    ) WinPointAfter = item.points;
                if (item.countHand() == 2 &&
                    item.points == 22)
                {
                    ++item.SeriesWin;
                    wins.Add(item);
                }
            }
            if (wins.Count > 0 ) return wins;
            #endregion

            #region If there are is players with points less 22
            if (CountActiviti > 0)
            {
                foreach (var item in gamers)//Add player with win combo
                {
                    if (WinPointTo == item.points)
                    {
                        ++item.SeriesWin;
                        wins.Add(item);
                    }
                }
                if (wins.Count > 0) return wins;
            }
            #endregion

            #region If there are is players with poins after 22
            else
            {
                //Add winners to the list.
                foreach (var item in gamers)
                {
                    if (WinPointAfter == item.points)
                    {
                        ++item.SeriesWin;
                        wins.Add(item);
                    }
                }
            }
            #endregion

            return wins;
        }
        #endregion

        #region Save and load
        public void saveToFile(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, this);
                    Console.WriteLine($"Объект сериализован: {fileName}");
                }
            }
            catch (System.UnauthorizedAccessException e)//IOException
            {
                throw e;
            }
}

        public Game readOfFile(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {

                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    return (Game)formatter.Deserialize(fs);
                }
            }
            catch (System.UnauthorizedAccessException e)//IOException
            {
                throw e;
            }
            catch(System.Runtime.Serialization.SerializationException e)
            {
                throw e;
            }
        }
        #endregion
    }
}
