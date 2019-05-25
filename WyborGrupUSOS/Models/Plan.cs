using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WyborGrupUSOS.Models
{
    public class Plan
    {
        public string StatusCode { get; set; }
        public HtmlNodeCollection Dane { get; set; }
        public List<UnivesityClass> Classes { get; set; }
    }
}
