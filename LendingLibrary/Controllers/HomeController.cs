using Markdig;
using System.Web.Mvc;

namespace LendingLibrary.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var readme = System.IO.File.ReadAllText(Server.MapPath("~/Content/README.md"));
            ViewBag.Readme = Markdown.ToHtml(readme);

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}