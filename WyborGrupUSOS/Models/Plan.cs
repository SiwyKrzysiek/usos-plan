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
        public List<UniversityClass> Classes { get; set; }
        public List<ClassGroup> ClassGroups { get; set; }

        //public Dictionary<Tuple<string, UniversityClass.ClassType>, List<UniversityClass>> ClassGroupsDictionary { get; set; }

        public Plan()
        {
        }

        public Plan(IEnumerable<UniversityClass> classes)
        {
            Classes = classes.ToList();
            var classGroupsDictionary = new Dictionary<Tuple<string, UniversityClass.ClassType>, List<UniversityClass>>();

            foreach (var universityClass in Classes)
            {
                var signature = universityClass.GetSignature();

                if (!classGroupsDictionary.ContainsKey(signature))
                {
                    classGroupsDictionary.Add(signature, new List<UniversityClass>{universityClass});
                }
                else
                {
                    classGroupsDictionary[signature].Add(universityClass);
                }
            }

            ClassGroups = (from g in classGroupsDictionary select new ClassGroup(g.Key.Item1, g.Key.Item2, g.Value)).ToList();
        }
    }
}
