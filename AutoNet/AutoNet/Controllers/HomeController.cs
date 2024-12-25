using AutoNet.Core.Domain;
using AutoNet.Core.ServiceInterface;
using AutoNet.Core.ViewModels;
using AutoNet.Data;
using AutoNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AutoNet.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ICarsServices _carsServices;
        private readonly AutoNetContext _context;

		public HomeController(ILogger<HomeController> logger, ICarsServices carsServices, AutoNetContext context)
		{
			_logger = logger;
            _carsServices = carsServices;
            _context = context;
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

            HttpContext.Session.SetString("SearchResults", JsonConvert.SerializeObject(results));

            return RedirectToAction("SearchResults");
        }

        public async Task<IActionResult> SearchResults()
        {
            var resultsJson = HttpContext.Session.GetString("SearchResults");
            var results = string.IsNullOrEmpty(resultsJson) ? new List<Car>() : JsonConvert.DeserializeObject<List<Car>>(resultsJson);

            foreach (var car in results)
            {
                car.Files = await _context.FileToDatabases
                    .Where(f => f.CarId == car.Id)
                    .ToListAsync();
            }

            return View(results);
        }
        public IActionResult GetImage(Guid id)
        {
            var image = _context.FileToDatabases.FirstOrDefault(f => f.Id == id);
            if (image != null)
            {
                return File(image.ImageData, "image/jpeg");
            }
            return NotFound();
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
