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
        VideoPokerHandViewModel DrawCards(VideoPokerHandViewModel heldCards, Deck deck);
        WinnerType CheckJacksOrBetterWinners(VideoPokerHandViewModel cards);
        PayTableItem[]? GetPayTable(GameType gameType);
        int GetWonCredits(PayTableItem[]? payTable, WinnerType winnerType, int wager);
        int GetLowestWagerAmount(PayTableItem[]? payTable);
        int? BetOne(PayTableItem[]? payTable, int currentWager);
        public int? BetMax(PayTableItem[]? payTable);
    }
}
