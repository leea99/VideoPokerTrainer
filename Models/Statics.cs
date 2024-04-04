using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

        public enum WinnerType : byte
        {
            None = 0,
            [Display(Name = "Jacks or Better")]
            JacksOrBetter,
            [Display(Name = "Two Pair")]
            TwoPair,
            [Display(Name = "Three of a Kind")]
            ThreeKind,
            Straight,
            Flush,
            [Display(Name = "Full House")]
            FullHouse,
            [Display(Name = "Four of a Kind")]
            FourKind,
            [Display(Name = "Straight Flush")]
            StraightFlush,
            [Display(Name = "Royal Flush")]
            RoyalFlush
        }

        public enum GameType : byte
        {
            JacksOrBetter
        }
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var displayAttribute = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                                        .FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
