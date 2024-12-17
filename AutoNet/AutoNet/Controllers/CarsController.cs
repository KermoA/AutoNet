using AutoNet.ApplicationServices.Services;
using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using AutoNet.Models.Cars;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoNet.Controllers
{
    public class CarsController : Controller
    {
        private readonly AutoNetContext _context;
        private readonly ICarsServices _carsServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileServices _fileServices;

        public CarsController
            (
            AutoNetContext context,
            ICarsServices carsServices,
            UserManager<ApplicationUser> userManager,
            IFileServices fileServices
            )
        {
            _context = context;
            _carsServices = carsServices;
            _userManager = userManager;
            _fileServices = fileServices;
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
        public async Task<IActionResult> Details(Guid id)
        {
            var car = await _carsServices.DetailAsync(id);

            if (car == null)
            {
                return View("Error");
            }

            var vm = new CarDetailsViewModel
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
                UserName = car.User.UserName ?? "No User",
                UserFirstName = car.User.FirstName ?? "No First Name",
                UserLastName = car.User.LastName ?? "No Last Name"
            };

            return View(vm);
        }

        public async Task<IActionResult> UserCars()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var userCars = _context.Cars
                .Where(car => car.UserId == currentUser.Id)
                .ToList();

            var vm = userCars.Select(car => new UserCarsViewModel
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Fuel = car.Fuel.ToString(),
                Mileage = car.Mileage,
                EngineDisplacement = car.EngineDisplacement,
                Power = car.Power,
                Transmission = car.Transmission.ToString(),
                UserName = currentUser.UserName,
                UserFirstName = currentUser.FirstName,
                UserLastName = currentUser.LastName
            }).ToList();

            return View(vm);
        }


        [HttpGet]
        public IActionResult Create()
        {
            CarCreateUpdateViewModel result = new();

            return View("CreateUpdate", result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateUpdateViewModel vm)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("CreateUpdate", vm);
            //}

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

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
                UpdatedAt = DateTime.Now,
                Files = vm.Files,
                Image = vm.Image
                    .Select(x => new FileToDatabaseDto
                    {
                        Id = x.ImageId,
                        ImageData = x.ImageData,
                        ImageTitle = x.ImageTitle,
                        CarId = vm.Id
                    }).ToArray()
            };

            var result = await _carsServices.Create(dto, currentUser.UserName);

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to create the car.");
                return View("CreateUpdate", vm);
            }

            return RedirectToAction(nameof(Index), vm);
        }


        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var userName = User.Identity.Name;
            var currentUser = await _userManager.FindByNameAsync(userName);

            if (currentUser == null)
            {
                return Unauthorized();
            }

			var car = await _carsServices.DetailAsync(id);

			if (car == null)
            {
                return NotFound();
            }

            if (car.UserId != currentUser.Id)
            {
                return Forbid();
            }

			var photos = await _context.FileToDatabases
				.Where(x => x.CarId == id)
				.Select(y => new CarImageViewModel
				{
					CarId = y.Id,
					ImageId = y.Id,
					ImageData = y.ImageData,
					ImageTitle = y.ImageTitle,
					Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
				}).ToArrayAsync();

            var vm = new CarCreateUpdateViewModel();

            vm.Id = car.Id;
            vm.Make = car.Make;
            vm.Model = car.Model;
            vm.Year = car.Year;
            vm.VIN = car.VIN;
            vm.Mileage = car.Mileage;
            vm.Power = car.Power;
            vm.EngineDisplacement = car.EngineDisplacement;
            vm.Fuel = car.Fuel.ToString();
            vm.Transmission = car.Transmission.ToString();
            vm.Drivetrain = car.Drivetrain.ToString();
            vm.InspectionMonth = car.InspectionMonth;
            vm.InspectionYear = car.InspectionYear;
            vm.Description = car.Description;
            vm.Image.AddRange(photos);
				
			

            return View("CreateUpdate", vm);
        }

		[HttpPost]
		public async Task<IActionResult> Update(CarCreateUpdateViewModel vm)
		{
			var userName = User.Identity.Name;
			var currentUser = await _userManager.FindByNameAsync(userName);

			if (currentUser == null)
			{
				return Unauthorized();
			}

			var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == vm.Id);

			if (car == null)
			{
				return NotFound();
			}

			if (car.UserId != currentUser.Id)
			{
				return Forbid();
			}

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
				CreatedAt = vm.CreatedAt,
				UpdatedAt = DateTime.UtcNow,
				Files = vm.Files
			};

			var result = await _carsServices.Update(dto);

			if (result == null)
			{
				return RedirectToAction("UserCars");
			}

			return RedirectToAction("UserCars");
		}


		[HttpPost]
		public async Task<IActionResult> Delete(Guid id)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
			{
				return Unauthorized();
			}

			try
			{
				var result = await _carsServices.Delete(id, currentUser.Id);

				if (result)
				{
					return RedirectToAction("UserCars");
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
