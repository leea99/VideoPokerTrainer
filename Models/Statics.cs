using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Statics
    {
        public enum Suit : byte
        {
            Clubs,
            Diamonds,
            Hearts,
            Spades
        }

        public enum Rank : byte
        {
            Two = 2,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }
    }
}
