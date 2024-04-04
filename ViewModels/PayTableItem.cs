using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Statics;

namespace ViewModels
{
    public class PayTableItem
    {
        public WinnerType WinnerType { get; set; }
        public Dictionary<int, int>? Payouts { get; set; }
        public PayTableItem(WinnerType winnerType, Dictionary<int, int> payouts)
        {
            WinnerType = winnerType;
            Payouts = payouts;
        }
    }
}
