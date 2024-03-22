using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace IServices
{
    public interface IVideoPokerService
    {
        public Deck DealCards();
        VideoPokerCardsViewModel DrawCards(VideoPokerCardsViewModel heldCards, Deck deck);
    }
}
