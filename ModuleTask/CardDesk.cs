using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ModuleTask
{
    [Serializable]
    public class CardDesk
    {
        private List<Card> desk = new List<Card>();
        public enum Basic { basic = 36, standart = 52, full = 54 };

        public CardDesk(Basic basicCards)
        {
            desk = GenericSortCard(basicCards);
        }

        public CardDesk(List<Card> loadDesk)
        {
            //desk = LoadDesk(loadDesk); // - it's gag
        }

        public Card this[int key] => (Card)(desk[key]);

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < desk.Count; ++i)
            {
                str += desk[i].ToString();
                if (i < desk.Count - 1) str += ", ";
                else str += "; ";
            }
            return str;
        }

        public List<Card> GenericSortCard(Basic basicCards = Basic.standart)
        {
            uint cards = (uint)basicCards;//-2-Jokers
            List<Card> list = new List<Card>();
            //create cards
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= (int)Card.names.King; j++)
                {
                    if (basicCards == Basic.basic &&
                        (j > (int)Card.names.Ace && j < (int)Card.names.six)) continue;

                    string color = (i == (int)Card.suits.Club || i == (int)Card.suits.Diamonds) ? "Red" : "Black";
                    list.Add(new Card((Card.suits)i, (Card.names)(j), color));
                }
            }
            if (basicCards == Basic.full)
            {
                list[(int)basicCards - 2] = new Card(Card.suits.Joker, Card.names.Joker, "Red");
                list[(int)basicCards - 1] = new Card(Card.suits.Joker, Card.names.Joker, "Black");
            }
            return list;
        }

        public int Length() => this.desk.Count;

        public void MixDesk(int count = 560)
        {
            List<Card> outCards = new List<Card>();
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] rand = new byte[2];

            for (int i = 0; i < 100; i++)
            {
                var firstIt = rand[1] % desk.Count;
                rngCsp.GetBytes(rand);
                int secondIt = rand[1] % desk.Count();

                var temp = desk[firstIt];
                desk[firstIt] = desk[secondIt];
                desk[secondIt] = temp;
            }
        }

        public Card TakeRandomCard()
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte [] rand = new byte [1];
            rngCsp.GetBytes(rand);
            int it = rand[0] % desk.Count();
            Card card = desk[it];
            desk.RemoveAt(it);
            return card;
        }

        public Card TakeDownCard()
        {
            if (desk.Count == 0) return null;
            Card card = desk[0];
            desk.RemoveAt(0);
            return card;
        }

        public Card TakeUpCard()
        {
            if (desk.Count == 0) return null;
            Card card = desk[desk.Count-1];
            desk.RemoveAt(desk.Count - 1);
            return card;
        }

        public List<int> findNameCards(List<Card> cards, Card.names name)
        {
            List<int> outList = new List<int>();
            for (int i = 0; i < cards.Count; ++i)
            {
                if (cards[i].name == name)
                    outList.Add(i);
            }
            return outList;
        }

        /// <summary>
        /// Write to file "suit - name - color" for desk
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public void writeCardsToFile(string fileName)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
                {
                    foreach (var item in this.desk)
                    {
                        writer.WriteLine($"{item.suit} - {item.name} - {item.color}");
                    }
                }
            }
            catch (System.UnauthorizedAccessException e)//IOException
            {
                throw e;
            }
        }

        public List<Card> ReadCardsOfFile(string fileName)
        {
            System.IO.StreamReader reader = null;
            string pattern = @"(\w*)\s*-\s(\w*|\d*)\s*-\s(\w*)";
            List<Card> cards = new List<Card>();
            try
            {
                reader = new System.IO.StreamReader(fileName);
                string line = reader.ReadToEnd();
                string[] lines = line.Split('\n');
                foreach (var item in lines)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        foreach (Match m in Regex.Matches(item, pattern))
                        {
                            Card.names name = (Card.names)Enum.Parse(typeof(Card.names), m.Groups[2].Value);
                            Card.suits suit = (Card.suits)Enum.Parse(typeof(Card.suits), m.Groups[1].Value);
                            cards.Add(new Card(suit, name, m.Groups[3].Value));
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error reader file");
                throw;
            }
            finally
            {
                reader.Close();
            }
            return cards;
        }

    }

}
