using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Statics;

namespace Models
{
    public class Deck
    {
        [JsonProperty]
        private List<Card> cards;
        [JsonProperty]
        private int cardIndex;

        public void InitializeDeck()
        {
            cards = new List<Card>();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }
            cardIndex = cards.Count - 1;
        }
        public void Shuffle()
        {
            Random rng = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        public void PrintDeck()
        {
            foreach (var card in cards)
            {
                Console.WriteLine(card);
            }
        }

        public Card? GetCardInDeck()
        {
            if (cardIndex > 0)
            {
                var card = cards[cardIndex];
                cards.RemoveAt(cardIndex);
                cardIndex--;
                return card;
            }
            return null;
        }

        public List<Card> GetCurrentDeck()
        {
            return cards;
        }
    }
}
