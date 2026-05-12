using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.Models;
using System.Diagnostics;

namespace Proyecto_Final.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
            // esto es para tomar solo 3 reseñas recientes
            var reseñas = ResenasController.lista
                            .TakeLast(3)
                            .Reverse()
                            .ToList();

            return View(reseñas);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        public IActionResult SobreNosotros()
        {
            return View();
        }

    }
}
