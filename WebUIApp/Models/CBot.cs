using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class CBot
    {
        [Key]
        public int id { get; set; }
        [Required]      
        public string botid { get; set; }
        public string botname { get; set; }
        public int baseordersize { get; set; }
        [Required]
        public string Pairs { get; set; }
        [Required]
        public int delay { get; set; }
        public string createdate { get; set; } = DateTime.Now.ToString();

        // Foreign Key Cloumn Buy Sell
        [Display(Name = "buySell")]
        public int BuyselId { get; set; }

        [ForeignKey("BuyselId")]
        public virtual BuySell buySell { get; set; } 

        //Foreign Key column Message
        // Foreign key 
        [Display(Name = "Message")]
        public int MessageID { get; set; }

        [ForeignKey("MessageID")]
        public virtual CMessage Message { get; set; } 


        //Foreign Key column ServiceStatus
        // Foreign key 
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual ServiceStatus Status { get; set; } 
        public CBot()
        {
            this.createdate = DateTime.Now.ToString();
        }
    }


}
