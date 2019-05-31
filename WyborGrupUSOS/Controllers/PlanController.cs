using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(model.Link);
            Plan plan = new Plan { StatusCode = result.StatusCode.ToString()};

            Stream stream = await result.Content.ReadAsStreamAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream);

            HtmlNodeCollection classesLinks = doc.DocumentNode.SelectNodes(@"//map").First().ChildNodes;
            var classes = ExtractClasses(doc, httpClient);

            plan.Dane = classesLinks;
            plan.Classes = classes.ToList();

            return View("DisplayPlan", plan);
        }

        /// <summary>
        /// Extract all classes from main view of Usos plan
        /// </summary>
        /// <param name="planPage">Default semester plan page</param>
        /// <param name="httpClient">Client used to download data</param>
        /// <returns>List of classes</returns>
        private IEnumerable<UniversityClass> ExtractClasses(HtmlDocument planPage, HttpClient httpClient)
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