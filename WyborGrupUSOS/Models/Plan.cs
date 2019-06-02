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

        public Dictionary<Tuple<string, UniversityClass.ClassType>, List<UniversityClass>> ClassGroups { get; set; }

        public Plan()
        {
        }

        public Plan(IEnumerable<UniversityClass> classes)
        {
            //TODO: Group classes with same type
            foreach (var universityClass in classes)
            {
                var signature = universityClass.GetSignature();

                if (!ClassGroups.ContainsKey(signature))
                {
                    ClassGroups.Add(signature, new List<UniversityClass>{universityClass});
                }
                else
                {
                    ClassGroups[signature].Add(universityClass);
                }
            }
        }
    }
}
