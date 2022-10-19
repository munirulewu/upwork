using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class APICredentials
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string APIKey { get; set; }
        [Required]
        public string APISecret { get; set; }
        public string APIStatus { get; set; }
    }
}
