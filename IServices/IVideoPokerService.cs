using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using static Models.Statics;

namespace IServices
{
    public interface IVideoPokerService
    {
        public Deck DealCards();
        VideoPokerCardsViewModel DrawCards(VideoPokerCardsViewModel heldCards, Deck deck);
        WinnerType CheckJacksOrBetterWinners(VideoPokerCardsViewModel cards);
        PayTableItem[]? GetPayTable(GameType gameType);
    }
}
