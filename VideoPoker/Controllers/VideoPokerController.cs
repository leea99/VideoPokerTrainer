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
            var gameSession = GetGameSession();
            if (gameSession == null)
            {
                gameSession = new VideoPokerGameViewModel();
            }
            if (gameSession.GameType == null)
            {
                gameSession.GameType = GameType.JacksOrBetter;
            }
            if (gameSession.PayTable == null)
            {
                gameSession.PayTable = _videoPokerService.GetPayTable(GameType.JacksOrBetter);
            }
            if (gameSession.HandViewModel == null)
            {
                gameSession.HandViewModel = new VideoPokerHandViewModel()
                {
                    CreditsWagered = _videoPokerService.GetLowestWagerAmount(gameSession.PayTable)
                };
            }
            if (gameSession.Balance == null)
            {
                gameSession.Balance = 5000;
            }
            SetGameSession(gameSession);
            return View(gameSession);
        }

        public IActionResult DealCards()
        {
            PayTableItem[]? payTable = GetPayTableFromSession();
            var deck = _videoPokerService.DealCards();
            int? creditsWager = GetWager();
            var videoPokerCards = new VideoPokerHandViewModel()
            {
                Card1 = deck.GetCardInDeck(),
                Card2 = deck.GetCardInDeck(),
                Card3 = deck.GetCardInDeck(),
                Card4 = deck.GetCardInDeck(),
                Card5 = deck.GetCardInDeck()
            };
            UpdateCreditBalance(creditsWager * -1);
            videoPokerCards.CreditsWagered = creditsWager;
            videoPokerCards.WinnerType = _videoPokerService.CheckJacksOrBetterWinners(videoPokerCards);
            HttpContext.Session.SetString("Deck", JsonConvert.SerializeObject(deck));
            var bestHolds = _videoPokerService.CalculateBestHolds(deck, videoPokerCards, payTable);
            var handData = UpdateHandData(videoPokerCards);
            handData.HoldInfo = bestHolds;
            return PartialView("~/Views/Shared/VideoPoker/_CardRow.cshtml", handData);
        }

        [HttpPost]
        public IActionResult DrawCards([FromBody] VideoPokerHandViewModel? heldCards)
        {
            PayTableItem[]? payTable = GetPayTableFromSession();
            int? wagerAmount = GetWager();
            var deckStr = HttpContext.Session.GetString("Deck");
            if (deckStr != null)
            {
                var deck = JsonConvert.DeserializeObject<Deck?>(deckStr);
                if (deck != null)
                {
                    var videoPokerCards = _videoPokerService.DrawCards(heldCards, deck);
                    videoPokerCards.CreditsWagered = wagerAmount;
                    videoPokerCards.WinnerType = _videoPokerService.CheckJacksOrBetterWinners(videoPokerCards);
                    videoPokerCards.CreditsWon = _videoPokerService.GetWonCredits(payTable, videoPokerCards.WinnerType, wagerAmount.GetValueOrDefault());
                    UpdateCreditBalance(videoPokerCards.CreditsWon);
                    return PartialView("~/Views/Shared/VideoPoker/_CardRow.cshtml", UpdateHandData(videoPokerCards));
                }
            }
            return RedirectToAction("DealCards");
        }

        private void UpdateCreditBalance(int? creditChange)
        {
            var gameData = GetGameSession();
            gameData = _videoPokerService.UpdateGameBalance(gameData, creditChange);
            SetGameSession(gameData);
        }

        [HttpPost]
        public IActionResult IncreaseWagerByOne()
        {
            int? wagerAmount = GetWager();
            var payTable = GetPayTableFromSession();
            wagerAmount = _videoPokerService.BetOne(payTable, wagerAmount.GetValueOrDefault());
            if (!wagerAmount.HasValue)
            {
                wagerAmount = _videoPokerService.GetLowestWagerAmount(payTable);
            }
            UpdateWager(wagerAmount);
            var newVm = new VideoPokerGameViewModel()
            {
                HandViewModel = new VideoPokerHandViewModel(wagerAmount.GetValueOrDefault()),
                PayTable = payTable
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
            UpdateWager(wagerAmount);
            var newVm = new VideoPokerGameViewModel()
            {
                HandViewModel = new VideoPokerHandViewModel(wagerAmount.GetValueOrDefault()),
                PayTable = payTable,
            };
            return PartialView("~/Views/Shared/VideoPoker/_PayTable.cshtml", newVm);
        }

        private void SetGameSession(VideoPokerGameViewModel? gameSession)
        {
            if (gameSession != null)
            {
                HttpContext.Session.SetString("GameSession", JsonConvert.SerializeObject(gameSession));
            }
        }

        private VideoPokerGameViewModel? GetGameSession()
        {
            VideoPokerGameViewModel? gameSession = null;
            var gameSessionStr = HttpContext.Session.GetString("GameSession");
            if (gameSessionStr != null)
            {
                gameSession = JsonConvert.DeserializeObject<VideoPokerGameViewModel>(gameSessionStr);
            }
            return gameSession;
        }

        private PayTableItem[]? GetPayTableFromSession()
        {
            PayTableItem[]? payTable = null;
            var gameSession = GetGameSession();
            if (gameSession != null)
            {
                payTable = gameSession.PayTable;
            }
            return payTable;
        }

        private int? GetWager()
        {
            int? wagerAmount = null;
            var gameSession = GetGameSession();
            if (gameSession != null && gameSession.HandViewModel != null)
            {
                wagerAmount = gameSession.HandViewModel.CreditsWagered;
            }
            return wagerAmount;
        }

        private void UpdateWager(int? wagerAmount)
        {
            var gameSession = GetGameSession();
            if (gameSession != null && gameSession.HandViewModel != null)
            {
                gameSession.HandViewModel.CreditsWagered = wagerAmount;
                SetGameSession(gameSession);
            }
        }

        private VideoPokerGameViewModel UpdateHandData(VideoPokerHandViewModel handVm)
        {
            var gameSession = GetGameSession();
            gameSession.HandViewModel = handVm;
            return gameSession;
        }
    }
}
