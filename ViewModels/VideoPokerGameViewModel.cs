using System;
using ViewModels;

namespace ViewModels
{
    public class VideoPokerGameViewModel
    {
        public int CreditsWagered { get; set; }
        public PayTableItem[]? PayTable { get; set; }
        public int Winnings { get; set; }
    }
}
