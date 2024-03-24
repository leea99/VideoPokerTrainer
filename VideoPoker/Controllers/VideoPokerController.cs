using IServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using ViewModels;

namespace VideoPoker.Controllers
{
    public class VideoPokerController : Controller
    {
        private IVideoPokerService _videoPokerService { get; set; }
        public VideoPokerController(IVideoPokerService videoPokerService) 
        { 
            _videoPokerService = videoPokerService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult JacksOrBetter()
        {
            var test = _videoPokerService.DealCards();
            return View();
        }

        public IActionResult DealCards()
        {
            var deck = _videoPokerService.DealCards();
            var videoPokerCards = new VideoPokerCardsViewModel()
            {
                Card1 = deck.GetCardInDeck(),
                Card2 = deck.GetCardInDeck(),
                Card3 = deck.GetCardInDeck(),
                Card4 = deck.GetCardInDeck(),
                Card5 = deck.GetCardInDeck()
            };
            videoPokerCards.WinnerType = _videoPokerService.CheckJacksOrBetterWinners(videoPokerCards);
            HttpContext.Session.SetString("Deck", JsonConvert.SerializeObject(deck));
            return PartialView("~/Views/Shared/VideoPoker/_CardRow.cshtml", videoPokerCards);
        }

        [HttpPost]
        public IActionResult DrawCards([FromBody] VideoPokerCardsViewModel? heldCards)
        {
            var deckStr = HttpContext.Session.GetString("Deck");
            if (deckStr != null)
            {
                var deck = JsonConvert.DeserializeObject<Deck?>(deckStr);
                if (deck != null)
                {
                    var videoPokerCards = _videoPokerService.DrawCards(heldCards, deck);
                    videoPokerCards.WinnerType = _videoPokerService.CheckJacksOrBetterWinners(videoPokerCards);
                    return PartialView("~/Views/Shared/VideoPoker/_CardRow.cshtml", videoPokerCards);
                }
            }
            return RedirectToAction("DealCards");
        }
    }
}
