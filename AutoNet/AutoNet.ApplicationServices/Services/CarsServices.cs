using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.AspNetCore.Identity;

namespace AutoNet.ApplicationServices.Services
{
	public class CarsServices : ICarsServices
	{
		private readonly AutoNetContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarsServices
			(
				AutoNetContext context,
                UserManager<ApplicationUser> userManager
            )
		{
			_context = context;
            _userManager = userManager;
        }

        public async Task<Car> Create(CarDto dto, string userName)
        {
            Car car = new Car
            {
                Id = Guid.NewGuid(),
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                VIN = dto.VIN,
                Mileage = dto.Mileage,
                Power = dto.Power,
                EngineDisplacement = dto.EngineDisplacement,
                Fuel = Enum.TryParse(dto.Fuel, out FuelType fuelType) ? fuelType : FuelType.Gasoline,
                Transmission = Enum.TryParse(dto.Transmission, out TransmissionType transmissionType) ? transmissionType : TransmissionType.Manual,
                Drivetrain = Enum.TryParse(dto.Drivetrain, out DrivetrainType drivetrainType) ? drivetrainType : DrivetrainType.FWD,
                InspectionMonth = dto.InspectionMonth,
                InspectionYear = dto.InspectionYear,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser != null)
            {
                car.UserId = currentUser.Id;
            }
            else
            {
                throw new Exception($"User with username {userName} not found.");
            }

            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();

            return car;
        }
    }
}
