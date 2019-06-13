using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WyborGrupUSOS.Models
{
    /// <summary>
    /// Groups classes with the same kind but at different times.
    /// Student hast to pick only one of them
    /// </summary>
    public class ClassGroup
    {
        public string Name { get; set; }
        public UniversityClass.ClassType Type { get; set; }
        public List<UniversityClass> Classes { get; set; }

        public ClassGroup(String name, UniversityClass.ClassType type, List<UniversityClass> classes)
        {
            Name = name;
            Type = type;

            Classes = classes;
        }
    }
}
