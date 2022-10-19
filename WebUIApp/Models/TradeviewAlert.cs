using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class TradeviewAlert
    {
        [Key]
        public int Id { get; set; }
        public string alertmessage { get; set; }
        public DateTime createdate { get; set; } = DateTime.Now;
    }
}
