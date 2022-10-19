using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class CRuleInfo
    {
       
        [Key]
        public int RuleID { get; set; }
        
        [Required]
        public string RuleName { get; set; }
       
        public string CreateDate { get; set; } = DateTime.Now.ToString();
        [Required]
        public int Target { get; set; }


        //Foreign Key column ServiceStatus
        // Foreign key 
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual ServiceStatus Status { get; set; }

    }
}
