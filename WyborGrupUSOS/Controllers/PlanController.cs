using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using WyborGrupUSOS.Models;

namespace WyborGrupUSOS.Controllers
{
    public class PlanController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> LoadPlan(UsosLink model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var queryString = HttpUtility.ParseQueryString(model.Link);
            queryString.Set("plan_format", "html");
            var link = queryString.ToString();

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(link);

            Stream stream = await result.Content.ReadAsStreamAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream);

            var classes = ExtractClassesFromHtmlView(doc).ToList();
            var plan = new Plan(classes);
            plan.StatusCode = result.StatusCode.ToString();
            
            return View("DisplayPlan", plan);
        }

        private IEnumerable<UniversityClass> ExtractClassesFromHtmlView(HtmlDocument planPage)
        {
            var table = planPage.DocumentNode.SelectSingleNode(
                @"//div[@id='layout-c22a']/div[@class='wrtext']/div[@class='wrtext']/div[@class='wrtext']/div/table[@cellspacing='0' and @cellpadding='0']"
                );

            //TODO: Read table row by row and detect which day it is
            var daysRow = table.FirstChild;
            var daysColumnSizes = ExtractDaysColumnSizes(daysRow).ToList();

            for (var i = 1; i < table.ChildNodes.Count; i++)
            {
                var tr = table.ChildNodes[i];
                if (!tr.HasChildNodes) continue; //TODO: Maybe ignore when InnerText == ""

                int columnCount = 0;
                foreach (var td in tr.ChildNodes)
                {
                    if (td.Attributes["colspan"] != null)
                    columnCount += Convert.ToInt32(td.Attributes["colspan"].Value);

                    if (IsClassData(td))
                    {
                        var universityClass = ExtractClassDataFromTableCell(td);
                        universityClass.DayOfWeek = DetectDayFromColumnCount(daysColumnSizes, columnCount);
                        yield return universityClass;
                    }
                }
            }
        }

        private DayOfWeek DetectDayFromColumnCount(IEnumerable<int> daysColumnSizes, int currentColumnCount)
        {
            int sum = 0, i = 1;
            foreach (var daysColumnSize in daysColumnSizes)
            {
                sum += daysColumnSize;
                if (currentColumnCount <= sum)
                    return (DayOfWeek) i;

                i++;
            }

            return DayOfWeek.Monday;
        }

        /// <summary>
        /// Each days column is fixed width. Knowing it will enable to parse day of the week of lectures
        /// </summary>
        /// <param name="daysRow"></param>
        /// <returns></returns>
        private IEnumerable<int> ExtractDaysColumnSizes(HtmlNode daysRow)
        {
            var children = daysRow.ChildNodes;
            children.RemoveAt(0);

            if (children.Count != 5)
                throw new FormatException("Expected 5 days of the weak");

            foreach (var th in children)
            {
                yield return Convert.ToInt32(th.Attributes["colspan"].Value);
            }
        }

        private UniversityClass ExtractClassDataFromTableCell(HtmlNode td)
        {
            var classTypeMap = new Dictionary<string, UniversityClass.ClassType>()
            {
                {"KON", UniversityClass.ClassType.Seminar},
                {"CW", UniversityClass.ClassType.Exercises},
                {"WYK", UniversityClass.ClassType.Lecture}
            };

            var details = td.SelectSingleNode(@"span[@class='note']").InnerText;

            var pattern = @"(?<time>\d{1,2}:\d{2}-\d{1,2}:\d{2}), (?<type>\w*) .*(?<groupNumber>\d)";
            var match = Regex.Match(details, pattern, RegexOptions.Compiled);

            var time = match.Groups["time"].Value;
            var type = match.Groups["type"].Value;
            var groupNumber = match.Groups["groupNumber"].Value;

            var timeParts = time.Split('-');
            var startTime = new Time(timeParts[0]);
            var endTime = new Time(timeParts[1]);

            var title = td.SelectSingleNode(@"div").InnerText;
            var className = title.Substring(0, title.LastIndexOf('(')).Trim();

            return new UniversityClass(className, classTypeMap[type], int.Parse(groupNumber), startTime, endTime);
        }

        private bool IsClassData(HtmlNode td)
        {
            var count = td.ChildNodes.Count;
            if (td.ChildNodes.Count != 2) return false;

            return td.SelectSingleNode(@"span[@class='note']") != null;
        }

        /// <summary>
        /// Extract all classes from <b>gif</b> view of Usos plan
        /// </summary>
        /// <param name="planPage">Default semester plan page</param>
        /// <param name="httpClient">Client used to download data</param>
        /// <returns>List of classes</returns>
        private IEnumerable<UniversityClass> ExtractClassesFromGifView(HtmlDocument planPage, HttpClient httpClient)
        {
            var classesLinks = planPage.DocumentNode.SelectNodes(@"//map").First().ChildNodes;
            
            // Create list of tasks with a query
            var tasks = from link in classesLinks select ExtractSingeClass(link.Attributes["href"].Value, httpClient);
            Task.WaitAll(tasks.ToArray());

            return from t in tasks select t.Result;
        }

        /// <summary>
        /// Loads single class from Usos class details page
        /// </summary>
        /// <param name="classPageLink">Link to class details</param>
        /// <param name="httpClient">Client used to download data</param>
        /// <returns></returns>
        private async Task<UniversityClass> ExtractSingeClass(string classPageLink, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(classPageLink);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new IOException("Unable to open class link");

            var stream = await response.Content.ReadAsStreamAsync();
            var document = new HtmlDocument();
            document.Load(stream);

            var contentDiv = document.DocumentNode.SelectSingleNode(@"//div[@class='wrtext']");
            var textNode = contentDiv.SelectSingleNode(@"h1");

            var className = textNode.SelectSingleNode(@"a").InnerText;

            var universityClass = new UniversityClass() {Name = className};

            return universityClass;
        }
    }
}