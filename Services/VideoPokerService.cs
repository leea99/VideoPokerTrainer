using IServices;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

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
    }
}
