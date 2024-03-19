using IServices;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class VideoPokerService : IVideoPokerService
    {
        public Deck DealCards()
        {
            var deck = new Deck();
            deck.Shuffle();
            return deck;
        }
    }
}
