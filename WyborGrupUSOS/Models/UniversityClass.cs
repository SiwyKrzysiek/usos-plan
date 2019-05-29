using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WyborGrupUSOS.Models
{
    public class UniversityClass
    {
        public string Name { get; set; }
        public ClassType Type { get; set; }
        public int? GroupNumber { get; set; }

        public enum ClassType
        {
            Lecture,
            Exercises,
            Laboratories,
            Seminar,
            Project
        }
    }
}
