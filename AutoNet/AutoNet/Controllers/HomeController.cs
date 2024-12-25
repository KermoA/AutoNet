using AutoNet.Core.Domain;
using AutoNet.Core.ServiceInterface;
using AutoNet.Core.ViewModels;
using AutoNet.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AutoNet.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ICarsServices _carsServices;

		public HomeController(ILogger<HomeController> logger, ICarsServices carsServices)
		{
			_logger = logger;
            _carsServices = carsServices;
		}

        public async Task<IActionResult> Index()
        {
            var viewModel = new CarSearchViewModel
            {
                Makes = await _carsServices.GetMakesAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Search(CarSearchViewModel searchModel)
        {
            var results = await _carsServices.SearchCarsAsync(searchModel);

            // Store the results in the session (instead of TempData)
            HttpContext.Session.SetString("SearchResults", JsonConvert.SerializeObject(results));

            return RedirectToAction("SearchResults");
        }

        public IActionResult SearchResults()
        {
            // Retrieve the results from the session
            var resultsJson = HttpContext.Session.GetString("SearchResults");
            var results = string.IsNullOrEmpty(resultsJson) ? new List<Car>() : JsonConvert.DeserializeObject<List<Car>>(resultsJson);

            return View(results);
        }

        public async Task<JsonResult> GetModels(string make)
        {
            var models = await _carsServices.GetModelsByMakeAsync(make);
            return Json(models);
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
	}
}
