using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WyborGrupUSOS.Models
{
    public class WebsiteLink
    {
        [Required]
        public string Link { get; set; }
    }
}
