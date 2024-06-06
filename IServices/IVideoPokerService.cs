using Models;
using System;
using System.Collections.Concurrent;
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
        Deck DealCards();
        VideoPokerHandViewModel DrawCards(VideoPokerHandViewModel heldCards, Deck deck);
        WinnerType CheckJacksOrBetterWinners(VideoPokerHandViewModel cards);
        List<Card> GetCardList(VideoPokerHandViewModel cards);
        PayTableItem[]? GetPayTable(GameType gameType);
        int GetWonCredits(PayTableItem[]? payTable, WinnerType winnerType, int wager);
        int GetLowestWagerAmount(PayTableItem[]? payTable);
        int? BetOne(PayTableItem[]? payTable, int currentWager);
        int? BetMax(PayTableItem[]? payTable);
        VideoPokerGameViewModel? UpdateGameBalance(VideoPokerGameViewModel? gameData, int? creditChange);
        List<HoldInfo> CalculateBestHolds(Deck? deck, VideoPokerHandViewModel hand, PayTableItem[]? payTable);
        public List<Card?> HandToList(VideoPokerHandViewModel hand);
        HoldInfo GetHoldInfo(Deck? deck, VideoPokerHandViewModel hand, PayTableItem[]? payTable, List<Card> c, out ConcurrentDictionary<WinnerType, int> outcomeTotals, out double totalPayout);
        string GetHandShort(List<Card> heldCards);
    }
}
