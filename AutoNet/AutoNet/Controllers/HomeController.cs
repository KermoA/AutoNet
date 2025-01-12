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
            var latestCars = await _context.Cars
                .Include(car => car.Files)
                .OrderByDescending(car => car.CreatedAt)
                .Take(12)
                .Select(car => new LatestCarViewModel
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Price = car.Price,
                    DiscountPrice = car.DiscountPrice,
                    ImageData = car.Files.Any() ? car.Files.FirstOrDefault().ImageData : null
                })
                .ToListAsync();

            var viewModel = new CarSearchViewModel
            {
                Makes = await _carsServices.GetMakesAsync(),
                LatestCars = latestCars
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

        public async Task<IActionResult> SearchResults(int page = 1, int pageSize = 10, string sortOrder = null)
        {
            var resultsJson = HttpContext.Session.GetString("SearchResults");
            var results = string.IsNullOrEmpty(resultsJson) ? new List<Car>() : JsonConvert.DeserializeObject<List<Car>>(resultsJson);

			switch (sortOrder)
			{
				case "price_desc":
					results = results.OrderByDescending(car => car.Price).ToList();
					break;
				case "year_asc":
					results = results.OrderBy(car => car.Year).ToList();
					break;
				case "year_desc":
					results = results.OrderByDescending(car => car.Year).ToList();
					break;
				default:
					results = results.OrderBy(car => car.Price).ToList();
					break;
			}

			var pagedResults = results
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            

            foreach (var car in pagedResults)
            {
                car.Files = await _context.FileToDatabases
                    .Where(f => f.CarId == car.Id)
                    .ToListAsync();
            }

            var viewModel = new SearchResultsViewModel
            {
                Cars = pagedResults,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)results.Count / pageSize),
                PageSize = pageSize,
				SortOrder = sortOrder
			};

            return View(viewModel);
        }

        public IActionResult GetImage(Guid id)
        {
            var car = _context.Cars
                .Include(c => c.Files)
                .FirstOrDefault(c => c.Id == id);

            if (car != null && car.Files.Any() && car.Files.FirstOrDefault()?.ImageData != null)
            {
                return File(car.Files.FirstOrDefault()?.ImageData, "image/jpeg");
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
