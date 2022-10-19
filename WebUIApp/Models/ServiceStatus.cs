using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUIApp.Models
{
    public class ServiceStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string ActiveStatus { get; set; }
    }
}
