using Microsoft.AspNetCore.Mvc;

namespace GasCalc.Controllers
{
    public class CarController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
