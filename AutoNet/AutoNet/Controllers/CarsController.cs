using AutoNet.Data;
using AutoNet.Models.Cars;
using Microsoft.AspNetCore.Mvc;

namespace AutoNet.Controllers
{
	public class CarsController : Controller
	{
		private readonly AutoNetContext _context;
		
		public CarsController
			(
			AutoNetContext context
			)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var result = _context.Cars
				.Select(x => new CarsIndexViewModel
				{
					Id = x.Id,
					Make = x.Make,
					Model = x.Model,
					Year = x.Year,
					VIN = x.VIN,
					Mileage = x.Mileage,
					Power = x.Power,
					EngineDisplacement = x.EngineDisplacement,
					Fuel = x.Fuel.ToString(),
					Transmission = x.Transmission.ToString(),
					Drivetrain = x.Drivetrain.ToString(),
					InspectionMonth = x.InspectionMonth,
					InspectionYear = x.InspectionYear,
					Description = x.Description,
					UserName = x.User.UserName,
					UserFirstName = x.User.FirstName,
					UserLastName = x.User.LastName,
					CreatedAt = DateTime.Now,
					UpdatedAt = DateTime.Now
				});

			return View(result);
		}
	}
}
