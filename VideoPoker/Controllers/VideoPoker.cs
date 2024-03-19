using IServices;
using Microsoft.AspNetCore.Mvc;

namespace VideoPoker.Controllers
{
    public class VideoPoker : Controller
    {
        private IVideoPokerService _videoPokerService { get; set; }
        public VideoPoker(IVideoPokerService videoPokerService) 
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
    }
}
