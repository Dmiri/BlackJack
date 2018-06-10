using System;
using System.Collections.Generic;

namespace ModuleTask
{
    [Serializable]
    public class Card
    {
        public enum names { Joker, Ace = 1, two = 2, three, four, five, six, seven, eight, nine, ten, Jack, Quin, King };
        public enum suits { Club, Cross, Diamonds, Peak, Joker };
        public string color { get; set; }
        public suits suit { get; set; }
        public names name { get; set; }

        public Card(Card.suits _suit, Card.names _value, string _color)
        {
            this.suit = _suit;
            this.name = _value;
            this.color = _color;
        }

        public Card()
        {
            this.color = null;
            this.suit = Card.suits.Joker;
            this.name = names.Joker;
        }


        /// <returns>suit - name - color</returns>
        public override string ToString()
        {
            return suit + " - " + name + " - " + color;
        }


        public static bool operator !=(Card left, Card right)
        {
            if (left.suit != right.suit || left.name != right.name || left.color != right.color)
                return true;
            return false;
        }

        public static bool operator ==(Card left, Card right)
        {
            if (left.suit == right.suit && left.name == right.name && left.color == right.color)
                return true;
            return false;
        }

        /// <summary>
        /// Displaying cards parameter
        /// </summary>
        public virtual void Display()
        {
            Console.WriteLine($"Suit = {suit}, Value = {name}, Color = {color}");
        }

        public override bool Equals(object obj)
        {
            var card = obj as Card;
            return card != null &&
                   color == card.color &&
                   suit == card.suit &&
                   name == card.name;
        }
    }
}
