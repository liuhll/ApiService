using System.Web.Mvc;

namespace Jueci.ApiService.Web.Controllers
{
    public class HomeController : ApiServiceControllerBase
    {
        public ActionResult Index()
        {
            // return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
            return Redirect(Request.Url.OriginalString + "swagger/ui/index");
        }
	}
}