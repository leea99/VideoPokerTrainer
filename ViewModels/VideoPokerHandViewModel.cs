using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Statics;

namespace ViewModels
{
    public class VideoPokerHandViewModel
    {
        public WinnerType WinnerType { get; set; }
        public int? CreditsWagered { get; set; }
        public int CreditsWon { get; set; }
        public Card? Card1 { get; set; }
        public Card? Card2 { get; set; }
        public Card? Card3 { get; set; }
        public Card? Card4 { get; set; }
        public Card? Card5 { get; set; }

        public VideoPokerHandViewModel()
        {
            WinnerType = new WinnerType();
            CreditsWagered = 0;
            CreditsWon = 0;
            Card1 = null; 
            Card2 = null; 
            Card3 = null; 
            Card4 = null; 
            Card5 = null;
        }

        public VideoPokerHandViewModel(int? creditsWagered)
        {
            CreditsWagered = creditsWagered;
        }
    }
}
