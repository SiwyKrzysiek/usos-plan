using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WyborGrupUSOS.Models
{
    public class Plan
    {
        private List<UniversityClass> _classes;
        public string StatusCode { get; set; }

        public List<UniversityClass> Classes
        {
            get => _classes;
            set => LoadClasses(value);
        }

        /// <summary>
        /// Classes that have only one group
        /// </summary>
        public List<UniversityClass> SingularClasses { get; private set; }
        /// <summary>
        /// Groups that have more than one member
        /// </summary>
        public List<ClassGroup> ClassGroups { get; private set; }

        public Plan(IEnumerable<UniversityClass> classes)
        {
            SingularClasses = new List<UniversityClass>();
            ClassGroups = new List<ClassGroup>();
            LoadClasses(classes);
        }

        private Dictionary<Tuple<string, UniversityClass.ClassType>, List<UniversityClass>> CreateInitialGrouping(
            IEnumerable<UniversityClass> classes)
        {
            var classGroupsDictionary = new Dictionary<Tuple<string, UniversityClass.ClassType>, List<UniversityClass>>();

            foreach (var universityClass in Classes)
            {
                var signature = universityClass.GetSignature();

                if (!classGroupsDictionary.ContainsKey(signature))
                {
                    classGroupsDictionary.Add(signature, new List<UniversityClass> { universityClass });
                }
                else
                {
                    classGroupsDictionary[signature].Add(universityClass);
                }
            }

            return classGroupsDictionary;
        }

        private void LoadClasses(IEnumerable<UniversityClass> classes)
        {
            _classes = classes.ToList();
            var classGroupsDictionary = CreateInitialGrouping(_classes);

            foreach (var group in classGroupsDictionary)
            {
                if (group.Value.Count == 1)
                    SingularClasses.Add(group.Value.First());
                else
                {
                    var name = group.Value.First().Name;
                    var type = group.Value.First().Type;
                    ClassGroups.Add(new ClassGroup(name, type, group.Value));
                }
            }
        }
    }
}
