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
        public string? ImagePath { get; set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            ImagePath = GetImagePath(suit, rank);
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }

        public string GetShorthandStr()
        {
            string shortHandStr = Suit.ToString()[0].ToString();
            if (Rank <= Rank.Ten)
            {
                shortHandStr = ((byte)Rank).ToString() + shortHandStr;
            }
            else
            {
                shortHandStr = Rank.ToString()[0].ToString() + shortHandStr;
            }
            return shortHandStr;
        }

        public string GetImagePath(Suit suit, Rank rank)
        {
            string fileName;
            if (rank <= Rank.Ten)
            {
                fileName = suit.ToString().ToLower() + "_" + (int)rank + ".svg";
            }
            else
            {
                fileName = suit.ToString().ToLower() + "_" + rank.ToString().ToLower() + ".svg";
            }
            string path = Path.Combine("\\Images", "Cards", fileName);
            return path;
        }
    }
}
