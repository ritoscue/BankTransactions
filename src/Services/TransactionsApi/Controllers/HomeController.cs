using Microsoft.AspNetCore.Mvc;

namespace TransactionsApi.Controllers
{
    /// <summary>
    /// Main Controller
    /// </summary>
    public class HomeController  : Controller
    {
        /// <summary>
        /// This action load swagger Api
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}