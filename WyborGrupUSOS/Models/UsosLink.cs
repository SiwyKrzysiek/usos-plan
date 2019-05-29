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
        [RegularExpression(@"https?:\/\/usosweb\..*\/.*", ErrorMessage = "Link musi wskazywać plan w serwisie Usos")]
        public string Link { get; set; }
    }
}
