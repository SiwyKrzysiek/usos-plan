using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> LoadPlan(WebsiteLink model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            HttpClient hc = new HttpClient();
            HttpResponseMessage result = await hc.GetAsync(model.Link);
            Plan plan = new Plan { StatusCode = result.StatusCode.ToString()};

            Stream stream = await result.Content.ReadAsStreamAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream);

            HtmlNodeCollection subjectLinks = doc.DocumentNode.SelectNodes(@"//map").First().ChildNodes;
            plan.Dane = subjectLinks;

            return View("DisplayPlan", plan);
        }
    }
}