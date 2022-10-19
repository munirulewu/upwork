using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class CAlert
    {
        [Key]
        public int alertId { get; set; }
        public int NoOFTrigger { get; set; }
        public int NoOFAlert { get; set; }
        public string LastTrueDate { get; set; }
        public string CurrentPercent { get; set; }
        public string createdate { get; set; }

        // Foreign key 
        [Display(Name = "Rule")]
        public int RuleID { get; set; }

        [ForeignKey("RuleID")]
        public virtual CRuleInfo Rule { get; set; }


        // Foreign key 
        [Display(Name = "Message")]
        public int MessageID { get; set; }

        [ForeignKey("MessageID")]
        public virtual CMessage Message { get; set; }
        public CAlert()
        {
            this.createdate = DateTime.Now.ToString();
        }

    }
}
