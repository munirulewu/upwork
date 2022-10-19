using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class BuySell
    {
        [Key]
        public int BuyselId { get; set; }
        public string BuySellStatus { get; set; }
    }
}
