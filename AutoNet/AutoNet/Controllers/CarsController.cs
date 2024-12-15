using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using AutoNet.Models.Cars;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoNet.Controllers
{
	public class CarsController : Controller
	{
		private readonly AutoNetContext _context;
		private readonly ICarsServices _carsServices;
		
		public CarsController
			(
			AutoNetContext context,
			ICarsServices carsServices
			)
		{
			_context = context;
			_carsServices = carsServices;
		}

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars
                .Include(c => c.User)
                .ToListAsync();

            var result = cars.Select(car => new CarsIndexViewModel
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                VIN = car.VIN,
                Mileage = car.Mileage,
                Power = car.Power,
                EngineDisplacement = car.EngineDisplacement,
                Fuel = car.Fuel.ToString(),
                Transmission = car.Transmission.ToString(),
                Drivetrain = car.Drivetrain.ToString(),
                InspectionMonth = car.InspectionMonth,
                InspectionYear = car.InspectionYear,
                Description = car.Description,
                CreatedAt = car.CreatedAt,
                UpdatedAt = car.UpdatedAt,
                UserName = car.User != null ? car.User.UserName : "No User",
                UserFirstName = car.User != null ? car.User.FirstName : "No First Name",
                UserLastName = car.User != null ? car.User.LastName : "No Last Name"
            });

            return View(result);
        }


        [HttpGet]
		public IActionResult Create()
		{
			CarCreateUpdateViewModel result = new();

			return View("CreateUpdate", result);
		}

		[HttpPost]
		public async Task<IActionResult> Create (CarCreateUpdateViewModel vm)
		{
			var dto = new CarDto()
			{
				Id = vm.Id,
				Make = vm.Make,
				Model = vm.Model,
				Year = vm.Year,
				VIN = vm.VIN,
				Mileage = vm.Mileage,
				Power = vm.Power,
				EngineDisplacement = vm.EngineDisplacement,
				Fuel = vm.Fuel.ToString(),
				Transmission = vm.Transmission.ToString(),
				Drivetrain = vm.Drivetrain.ToString(),
				InspectionMonth = vm.InspectionMonth,
				InspectionYear = vm.InspectionYear,
				Description = vm.Description,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now

			};

            var userName = User.Identity.Name;

            var result = await _carsServices.Create(dto, userName);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), vm);
        }
	}
}
