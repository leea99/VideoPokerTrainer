using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Models.Statics;

namespace Models
{
    public class Card
    {
        [JsonPropertyName("Suit")]
        public Suit Suit { get; set; }

        [JsonPropertyName("Rank")]
        public Rank Rank { get; set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}
