using IServices;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using static Models.Statics;

namespace Services
{
    public class VideoPokerService : IVideoPokerService
    {
        public Deck DealCards()
        {
            var deck = new Deck();
            deck.InitializeDeck();
            deck.Shuffle();
            return deck;
        }

        public VideoPokerCardsViewModel DrawCards(VideoPokerCardsViewModel heldCards, Deck deck)
        {
            deck.Shuffle();
            if (heldCards.Card1 == null)
            {
                heldCards.Card1 = deck.GetCardInDeck();
            }
            if (heldCards.Card2 == null)
            {
                heldCards.Card2 = deck.GetCardInDeck();
            }
            if (heldCards.Card3 == null)
            {
                heldCards.Card3 = deck.GetCardInDeck();
            }
            if (heldCards.Card4 == null)
            {
                heldCards.Card4 = deck.GetCardInDeck();
            }
            if (heldCards.Card5 == null)
            {
                heldCards.Card5 = deck.GetCardInDeck();
            }
            return heldCards;
        }

        public WinnerType CheckJacksOrBetterWinners(VideoPokerCardsViewModel cards)
        {
            var cardList = GetCardList(cards);
            var flushWinner = CheckFlush(cardList);
            var straightWinner = CheckStraight(cardList);
            if (flushWinner && straightWinner)
            {
                if (cardList.First().Rank != Rank.Ten)
                {
                    return WinnerType.StraightFlush;
                }
                else
                {
                    return WinnerType.RoyalFlush;
                }
            }
            else if (flushWinner)
            {
                return WinnerType.Flush;
            }
            else if (straightWinner)
            {
                return WinnerType.Straight;
            }
            return CheckXOfKind(cardList);
        }

        private List<Card> GetCardList(VideoPokerCardsViewModel cards)
        {
            var list = new List<Card>();
            if (cards != null)
            {
                if (cards.Card1 != null)
                {
                    list.Add(cards.Card1);
                }
                if (cards.Card2 != null)
                {
                    list.Add(cards.Card2);
                }
                if (cards.Card3 != null)
                {
                    list.Add(cards.Card3);
                }
                if (cards.Card4 != null)
                {
                    list.Add(cards.Card4);
                }
                if (cards.Card5 != null)
                {
                    list.Add(cards.Card5);
                }
            }
            return list.OrderBy(x => x.Rank).ToList();
        }

        private bool CheckFlush(List<Card> cards)
        {
            var suits = cards.GroupBy(x => x.Suit);
            if (suits.Count() == 1)
            {
                return true;
            }
            return false;
        }

        private bool CheckStraight(List<Card> cards)
        {
            int straightRank = (int)cards.First().Rank;
            foreach (var card in cards)
            {
                int cardRank = (int)card.Rank;
                if (cardRank != straightRank++)
                {
                    if (straightRank != 6 || card.Rank != Rank.Ace) //Handles A2345 straights
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        private WinnerType CheckXOfKind(List<Card> cards)
        {
            var rankSortedCards = cards.GroupBy(x => x.Rank).OrderByDescending(x => x.Count()).ToArray();
            if (rankSortedCards.Count() == 2)
            {
                if (rankSortedCards[0].Count() == 3)
                {
                    return WinnerType.FullHouse;
                }
                return WinnerType.FourKind;
            }
            else if (rankSortedCards[0].Count() == 3)
            {
                return WinnerType.ThreeKind;
            }
            else if (rankSortedCards[0].Count() == 2 && rankSortedCards[1].Count() == 2)
            {
                return WinnerType.TwoPair;
            }
            var lonePair = rankSortedCards.FirstOrDefault(x => x.Count() == 2);
            if (lonePair != null && lonePair.First().Rank >= Rank.Jack)
            {
                return WinnerType.JacksOrBetter;
            }
            return WinnerType.None;
        }

        public PayTableItem[]? GetPayTable(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.JacksOrBetter:
                    return new PayTableItem[]
                    {
                        new PayTableItem(WinnerType.RoyalFlush,  new Dictionary<int, int> { {1, 250}, {2, 500}, {3, 750}, {4, 1000}, {5, 4000} }),
                        new PayTableItem(WinnerType.StraightFlush,  new Dictionary<int, int> { {1, 50}, {2, 100}, {3, 150}, {4, 200}, {5, 250} }),
                        new PayTableItem(WinnerType.FourKind,  new Dictionary<int, int> { {1, 25}, {2, 50}, {3, 75}, {4, 100}, {5, 125} }),
                        new PayTableItem(WinnerType.FullHouse,  new Dictionary<int, int> { {1, 9}, {2, 18}, {3, 27}, {4, 36}, {5, 45} }),
                        new PayTableItem(WinnerType.Flush,  new Dictionary<int, int> { {1, 6}, {2, 12}, {3, 18}, {4, 24}, {5, 30} }),
                        new PayTableItem(WinnerType.Straight,  new Dictionary<int, int> { {1, 4}, {2, 8}, {3, 12}, {4, 16}, {5, 20} }),
                        new PayTableItem(WinnerType.ThreeKind,  new Dictionary<int, int> { {1, 3}, {2, 6}, {3, 9}, {4, 12}, {5, 15} }),
                        new PayTableItem(WinnerType.TwoPair,  new Dictionary<int, int> { {1, 2}, {2, 4}, {3, 6}, {4, 8}, {5, 10} }),
                        new PayTableItem(WinnerType.JacksOrBetter,  new Dictionary<int, int> { {1, 1}, {2, 2}, {3, 3}, {4, 4}, {5, 5} }),
                    };
            }
            return null;
        }
    }
}
