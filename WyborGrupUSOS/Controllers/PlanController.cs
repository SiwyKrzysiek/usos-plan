using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            Plan plan = new Plan { StatusCode = result.StatusCode.ToString()};

            Stream stream = await result.Content.ReadAsStreamAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream);

            ExtractClassesFromHtmlView(doc);
            
            var classes = ExtractClassesFromGifView(doc, httpClient);

            HtmlNodeCollection classesLinks = doc.DocumentNode.SelectNodes(@"//map").First().ChildNodes;
            plan.Dane = classesLinks;

            plan.Classes = classes.ToList();

            return View("DisplayPlan", plan);
        }

        private IEnumerable<UniversityClass> ExtractClassesFromHtmlView(HtmlDocument planPage)
        {
            var table = planPage.DocumentNode.SelectSingleNode(
                @"//div[@id='layout-c22a']/div[@class='wrtext']/div[@class='wrtext']/div[@class='wrtext']/div/table[@cellspacing='0' and @cellpadding='0']"
                );

            var dataNodes = table.SelectNodes(@"//td").Where(IsClassData);

            return null;
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