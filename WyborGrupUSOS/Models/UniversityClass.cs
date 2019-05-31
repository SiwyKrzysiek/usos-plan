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
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }

        public enum ClassType
        {
            Lecture,
            Exercises,
            Laboratories,
            Seminar,
            Project
        }

        public UniversityClass()
        {
        }

        public UniversityClass(string name, ClassType type, int? groupNumber, Time start = null, Time end = null)
        {
            Name = name;
            Type = type;
            GroupNumber = IsSplitToGroups(type) ? groupNumber : null;
        }

        /// <summary>
        /// Informs if given class type is single for whole year or divided to groups
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public bool IsSplitToGroups(ClassType classType)
        {
            return !(classType == ClassType.Lecture || classType == ClassType.Seminar);
        }
    }
}
