using System;
using System.Collections.Generic;

namespace ModuleTask
{
    [Serializable]
    class Gambler
    {
        public enum Status
        {
            Activiti,
            Unactiviti
        }
        protected List<Card> hand = new List<Card>();
        public string Name { get; set; } //This is player's name
        public int points { get; set; } //This is player's points
        public Status status { get; set; } //This is player's status
        public int SeriesWin { get; set; } //This is player's wins count


        #region Operations with the player and his List<Card>hand
        /// <summary>
        /// Reterns the number of cards in the player's hand.
        /// </summary>
        /// <returns>Reterns the number of cards in the player's hand.</returns>
        public int countHand()
        {
            return hand.Count;
        }

        /// <summary>
        /// Reterns the number of cards in the player's hand.
        /// </summary>
        /// <returns>Reterns the player's points.</returns>
        public virtual int TakeCard(Card card, int weight)
        {
            hand.Add(card);
            points += weight;
            return points;
        }

        /// <summary>
        /// Remove cards in player's hand
        /// </summary>
        public void RemoveHand()
        {
            hand.Clear();
        }
        #endregion

        #region Display and visual

        /// <returns>Name - points - status; Hand;</returns>
        public override string ToString()
        {
            string cards = "";
            for (int i = 0; i < hand.Count; ++i)
            {
                cards += hand[i].ToString();
                if (i < hand.Count - 1) cards += ", ";
                else cards += "; ";
            }

            return Name + " - " + points + " - " + status + "; " + cards;
        }

        /// <summary>
        /// Displaying cards in the player's hand.
        /// </summary>
        public virtual void DisplayHand()
        {
            foreach (var item in hand)
            {
                Console.WriteLine($"Suit = {item.suit}, Value = {item.name}, Color = {item.color}");
            }
        }

        #endregion
    }
}
