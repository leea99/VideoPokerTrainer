using IServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Services;
using System.Security.AccessControl;
using System.Text.Json.Serialization;
using ViewModels;
using static Models.Statics;

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
            PayTableItem[]? payTable = GetPayTableFromSession();
            int? creditsWager = HttpContext.Session.GetInt32("CreditsWager");
            HttpContext.Session.SetInt32("GameType", (int)GameType.JacksOrBetter);
            if (payTable == null)
            {
                payTable = _videoPokerService.GetPayTable(GameType.JacksOrBetter);
                HttpContext.Session.SetString("PayTable", JsonConvert.SerializeObject(payTable));
            }
            if (creditsWager == null)
            {
                creditsWager = _videoPokerService.GetLowestWagerAmount(payTable);
                HttpContext.Session.SetInt32("CreditsWager", creditsWager.GetValueOrDefault());
            }
            var vm = new VideoPokerGameViewModel()
            {
                CreditsWagered = creditsWager.GetValueOrDefault(),
                PayTable = payTable,
                Winnings = 0,
            };
            return View(vm);
        }

        public IActionResult DealCards()
        {
            var deck = _videoPokerService.DealCards();
            var videoPokerCards = new VideoPokerHandViewModel()
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
        public IActionResult DrawCards([FromBody] VideoPokerHandViewModel? heldCards)
        {
            PayTableItem[]? payTable = GetPayTableFromSession();
            var deckStr = HttpContext.Session.GetString("Deck");
            if (deckStr != null)
            {
                var deck = JsonConvert.DeserializeObject<Deck?>(deckStr);
                if (deck != null)
                {
                    var videoPokerCards = _videoPokerService.DrawCards(heldCards, deck);
                    videoPokerCards.WinnerType = _videoPokerService.CheckJacksOrBetterWinners(videoPokerCards);
                    videoPokerCards.CreditsWon = _videoPokerService.GetWonCredits(payTable, videoPokerCards.WinnerType, videoPokerCards.CreditsWagered);
                    return PartialView("~/Views/Shared/VideoPoker/_CardRow.cshtml", videoPokerCards);
                }
            }
            return RedirectToAction("DealCards");
        }

        [HttpPost]
        public IActionResult IncreaseWagerByOne()
        {
            int? wagerAmount = HttpContext.Session.GetInt32("CreditsWager");
            var payTable = GetPayTableFromSession();
            wagerAmount = _videoPokerService.BetOne(payTable, wagerAmount.GetValueOrDefault());
            if (!wagerAmount.HasValue)
            {
                wagerAmount = _videoPokerService.GetLowestWagerAmount(payTable);
            }
            HttpContext.Session.SetInt32("CreditsWager", wagerAmount.GetValueOrDefault());
            var newVm = new VideoPokerGameViewModel()
            {
                CreditsWagered = wagerAmount.GetValueOrDefault(),
                PayTable = payTable,
                Winnings = 0
            };
            return PartialView("~/Views/Shared/VideoPoker/_PayTable.cshtml", newVm);
        }

        [HttpPost]
        public IActionResult MaxWager()
        {
            var payTable = GetPayTableFromSession();
            int? wagerAmount = _videoPokerService.BetMax(payTable);
            if (!wagerAmount.HasValue)
            {
                wagerAmount = _videoPokerService.GetLowestWagerAmount(payTable);
            }
            HttpContext.Session.SetInt32("CreditsWager", wagerAmount.GetValueOrDefault());
            var newVm = new VideoPokerGameViewModel()
            {
                CreditsWagered = wagerAmount.GetValueOrDefault(),
                PayTable = payTable,
                Winnings = 0
            };
            return PartialView("~/Views/Shared/VideoPoker/_PayTable.cshtml", newVm);
        }

        private PayTableItem[]? GetPayTableFromSession()
        {
            PayTableItem[]? payTable = null;
            var payTableStr = HttpContext.Session.GetString("PayTable");
            if (payTableStr != null)
            {
                payTable = JsonConvert.DeserializeObject<PayTableItem[]>(payTableStr);
            }
            return payTable;
        }
    }
}
