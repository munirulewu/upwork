using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class CMessage
    {
        [Key]
        public int MessageID { get; set; }
       
        [Required]
        public string MessageName { get; set; }

        public string MessageCode { get; set; }
        public string CreateDate { get; set; }

        public CMessage()
        {
            this.CreateDate = DateTime.Now.ToString();
        }

    }
}
