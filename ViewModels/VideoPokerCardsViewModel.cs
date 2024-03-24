using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Statics;

namespace ViewModels
{
    public class VideoPokerCardsViewModel
    {
        public WinnerType WinnerType { get; set; }
        public Card? Card1 { get; set; }
        public Card? Card2 { get; set; }
        public Card? Card3 { get; set; }
        public Card? Card4 { get; set; }
        public Card? Card5 { get; set; }
    }
}
