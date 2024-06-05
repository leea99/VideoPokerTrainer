using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Statics;

namespace ViewModels
{
    public class HoldInfo
    {
        public List<Card> HeldCards { get; set; }
        public string HoldShorthand { get; set; }
        public ConcurrentDictionary<WinnerType, int>? Outcomes { get; set; }
        public double ExpectedCredits { get; set; }
    }
}
