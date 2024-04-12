using System;
using ViewModels;
using static Models.Statics;

namespace ViewModels
{
    public class VideoPokerGameViewModel
    {
        public GameType? GameType { get; set; }
        public PayTableItem[]? PayTable { get; set; }
        public VideoPokerHandViewModel? HandViewModel { get; set; }
        public int? Balance { get; set; }
    }
}
