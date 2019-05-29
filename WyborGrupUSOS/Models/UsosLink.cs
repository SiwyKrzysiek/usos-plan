using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WyborGrupUSOS.Models
{
    public class UsosLink
    {
        [Required(ErrorMessage = "Link do Usosa jest wymagany")]
        public string Link { get; set; }
    }
}
