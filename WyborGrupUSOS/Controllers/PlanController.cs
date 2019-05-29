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

            HttpClient hc = new HttpClient();
            HttpResponseMessage result = await hc.GetAsync(model.Link);
            Plan plan = new Plan { StatusCode = result.StatusCode.ToString()};

            Stream stream = await result.Content.ReadAsStreamAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream);

            HtmlNodeCollection classesLinks = doc.DocumentNode.SelectNodes(@"//map").First().ChildNodes;
            var classes = await ExtractClasses(doc);

            plan.Dane = classesLinks;
            plan.Classes = classes;

            return View("DisplayPlan", plan);
        }

        private async Task<List<UnivesityClass>> ExtractClasses(HtmlDocument planPage)
        {
            var classesLinks = planPage.DocumentNode.SelectNodes(@"//map").First().ChildNodes;
            var httpClient = new HttpClient();

            var results = new List<UnivesityClass>(classesLinks.Count);
            foreach (var link in classesLinks)
            {
                string url = link.Attributes["href"].Value;
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new IOException("Unable to open class link");

                var stream = await response.Content.ReadAsStreamAsync();
                var document = new HtmlDocument();
                document.Load(stream);

                var contentDiv = document.DocumentNode.SelectSingleNode(@"//div[@class='wrtext']");
                var textNode = contentDiv.SelectSingleNode(@"h1");

                var className = textNode.SelectSingleNode(@"a").InnerText;

                results.Add(new UnivesityClass() {Name = className});
            }

            return results;
        }
    }
}