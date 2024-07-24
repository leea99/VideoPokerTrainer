using IServices;
using Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
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

        public VideoPokerHandViewModel DrawCards(VideoPokerHandViewModel heldCards, Deck deck)
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

        public WinnerType CheckJacksOrBetterWinners(VideoPokerHandViewModel cards)
        {
            var cardList = GetCardList(cards).OrderBy(x => x.Rank).ToList();
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

        public List<Card> GetCardList(VideoPokerHandViewModel cards)
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
            return list;
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

        public int GetWonCredits(PayTableItem[]? payTable, WinnerType winnerType, int wager)
        {
            int creditsWon = 0;
            if (payTable != null)
            {
                var payTableEntryMatch = payTable.FirstOrDefault(x => x.WinnerType == winnerType);
                if (payTableEntryMatch != null && payTableEntryMatch.Payouts != null)
                {
                    creditsWon = payTableEntryMatch.Payouts.FirstOrDefault(x => x.Key == wager).Value;
                }
            }
            return creditsWon;
        }

        public int GetLowestWagerAmount(PayTableItem[]? payTable)
        {
            int wagerAmount = 0;
            if (payTable != null)
            {
                var firstEntry = payTable.FirstOrDefault();
                if (firstEntry != null && firstEntry.Payouts != null)
                {
                    wagerAmount = firstEntry.Payouts.FirstOrDefault().Key;
                }
            }
            return wagerAmount;
        }

        public int? BetOne(PayTableItem[]? payTable, int currentWager)
        {
            if (payTable != null)
            {
                var firstEntry = payTable.FirstOrDefault();
                if (firstEntry != null && firstEntry.Payouts != null)
                {
                    List<int> sortedKeys = firstEntry.Payouts.Keys.ToList();
                    sortedKeys.Sort();

                    for (int i = 0; i < sortedKeys.Count; i++)
                    {
                        if (sortedKeys[i] == currentWager)
                        {
                            if (i == sortedKeys.Count - 1)
                            {
                                return sortedKeys[0];
                            }
                            return sortedKeys[++i];
                        }
                    }
                }
            }
            return null;
        }

        public int? BetMax(PayTableItem[]? payTable)
        {
            if (payTable != null)
            {
                var firstEntry = payTable.FirstOrDefault();
                if (firstEntry != null && firstEntry.Payouts != null)
                {
                    List<int> sortedKeys = firstEntry.Payouts.Keys.ToList();
                    sortedKeys.Sort();

                    return sortedKeys.Last();
                }
            }
            return null;
        }

        public VideoPokerGameViewModel? UpdateGameBalance(VideoPokerGameViewModel? gameData, int? creditChange)
        {
            if (gameData == null)
            {
                return null;
            }
            else
            {
                if (creditChange != null)
                {
                    gameData.Balance += creditChange;
                }
            }
            return gameData;
        }

        public List<HoldInfo> CalculateBestHolds(Deck? deck, VideoPokerHandViewModel hand, PayTableItem[]? payTable)
        {
            List<Card?> handCards = HandToList(hand);
            var combo = new List<List<Card>>();
            GenerateCombinations(deck, hand, handCards, 0, new List<Card>(), combo);
            var expectedCred = new ConcurrentBag<HoldInfo>();
            Parallel.ForEach(combo, c =>
            {
                ConcurrentDictionary<WinnerType, int> outcomeTotals;
                double totalPayout;
                GetHoldInfo(deck, hand, payTable, c, out outcomeTotals, out totalPayout);
                expectedCred.Add(GetHoldInfo(deck, hand, payTable, c, out outcomeTotals, out totalPayout));
            });
            return expectedCred.OrderByDescending(x => x.ExpectedCredits).ToList();
        }

        public List<Card?> HandToList(VideoPokerHandViewModel hand)
        {
            var cardList = new List<Card>();
            if (hand.Card1 != null)
            {
                cardList.Add(hand.Card1);
            }
            if (hand.Card2 != null)
            {
                cardList.Add(hand.Card2);
            }
            if (hand.Card3 != null)
            {
                cardList.Add(hand.Card3);
            }
            if (hand.Card4 != null)
            {
                cardList.Add(hand.Card4);
            }
            if (hand.Card5 != null)
            {
                cardList.Add(hand.Card5);
            }
            return cardList;
        }

        public HoldInfo GetHoldInfo(Deck? deck, VideoPokerHandViewModel hand, PayTableItem[]? payTable, List<Card> c, out ConcurrentDictionary<WinnerType, int> outcomeTotals, out double totalPayout)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            var localHand = new VideoPokerHandViewModel()
            {
                Card1 = hand.Card1,
                Card2 = hand.Card2,
                Card3 = hand.Card3,
                Card4 = hand.Card4,
                Card5 = hand.Card5,
                CreditsWagered = hand.CreditsWagered
            };
            DeckOperations(deck, localHand);
            outcomeTotals = InitializeComboDictionary();
            GetCombinationsRecursive(deck.GetCurrentDeck(), localHand, c, 0, 5, outcomeTotals);
            totalPayout = 0;
            int totalOutcomes = outcomeTotals.Values.Sum();
            foreach (var result in outcomeTotals)
            {
                double prob = (double)result.Value / (double)totalOutcomes;
                totalPayout += GetWonCredits(payTable, result.Key, hand.CreditsWagered.GetValueOrDefault()) * prob;
            }
            stopwatch.Stop();
            return new HoldInfo()
            {
                HeldCards = c,
                HoldShorthand = GetHandShort(c),
                Outcomes = outcomeTotals,
                ExpectedCredits = totalPayout
            };
        }

        private void GenerateCombinations(Deck? deck, VideoPokerHandViewModel hand, List<Card> cards, int index, List<Card> current, List<List<Card>> results)
        {
            if (index == cards.Count)
            {
                results.Add(new List<Card>(current));
                //GetCombinationsRecursive(deck.GetCurrentDeck(), hand, current, 0, 5, InitializeComboDictionary());
                return;
            }

            // Exclude the current card and move to the next
            GenerateCombinations(deck, hand, cards, index + 1, current, results);

            // Include the current card and move to the next
            current.Add(cards[index]);
            GenerateCombinations(deck, hand, cards, index + 1, current, results);

            // Backtrack and remove the last added card
            current.RemoveAt(current.Count - 1);
        }

        private ConcurrentDictionary<WinnerType, int> InitializeComboDictionary()
        {
            return new ConcurrentDictionary<WinnerType, int>(new[]
            {
                new KeyValuePair<WinnerType, int>(WinnerType.None, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.JacksOrBetter, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.TwoPair, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.ThreeKind, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.Straight, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.Flush, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.FullHouse, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.FourKind, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.StraightFlush, 0),
                new KeyValuePair<WinnerType, int>(WinnerType.RoyalFlush, 0),
            });
        }

        private void GetCombinationsRecursive(List<Card> deck, VideoPokerHandViewModel hand, List<Card> currentCombination, int start, int combinationSize, ConcurrentDictionary<WinnerType, int> results)
        {
            if (currentCombination.Count == combinationSize)
            {
                hand.Card1 = currentCombination[0];
                hand.Card2 = currentCombination[1];
                hand.Card3 = currentCombination[2];
                hand.Card4 = currentCombination[3];
                hand.Card5 = currentCombination[4];
                var winType = CheckJacksOrBetterWinners(hand);
                results.AddOrUpdate(winType, 1, (key, oldValue) => oldValue + 1);
                return;
            }

            for (int i = start; i <= deck.Count - (combinationSize - currentCombination.Count); i++)
            {
                currentCombination.Add(deck[i]);
                GetCombinationsRecursive(deck, hand, currentCombination, i + 1, combinationSize, results);
                currentCombination.RemoveAt(currentCombination.Count - 1);
            }
        }

        public string GetHandShort(List<Card> heldCards)
        {
            if (heldCards.Count == 0)
            {
                return "Discard All";
            }
            string shorthandStr = "";
            foreach (var card in heldCards.Where(x => x != null))
            {
                shorthandStr += card.GetShorthandStr();
            }
            return shorthandStr;
        }

        private void DeckOperations(Deck? deck, VideoPokerHandViewModel hand)
        {
            if (deck != null)
            {
                var outcomeTotals = InitializeComboDictionary();
                var grouped = deck.GetRankGroups();
                var heldCards = HandToList(hand);
                var slots = deck.GetFullDeckSize() - deck.GetCurrentDeckSize() - heldCards.Count;
                int fourKind = 0;
                foreach (var group in grouped)
                {
                    var heldMatch = heldCards.Where(x => x.Rank == group.Key).Count();
                    var totalRank = heldMatch + group.Count();
                    if (totalRank + heldMatch >= 4 && slots + heldMatch >= 4)
                    {
                        //Need to account for throwing away a part of a pair.
                        fourKind += Combination(deck.GetCurrentDeckSize() - group.Count(), slots - group.Count());
                    }
                }
            }
        }

        private int Combination(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;

            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result = result * (n - i + 1) / i;
            }

            return result;
        }
    }
}

