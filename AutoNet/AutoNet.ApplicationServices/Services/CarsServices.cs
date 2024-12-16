using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Car> DetailAsync(Guid id)
        {
            var result = await _context.Cars
                .FirstOrDefaultAsync(x => x.Id == id );

            return result;
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

        public async Task<Car> Update(CarDto dto, string userId)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.UserId == userId);
            if (car == null)
            {
                throw new Exception($"Car with ID {dto.Id} not found or does not belong to the current user.");
            }

            car.Make = dto.Make;
            car.Model = dto.Model;
            car.Year = dto.Year;
            car.VIN = dto.VIN;
            car.Mileage = dto.Mileage;
            car.Power = dto.Power;
            car.EngineDisplacement = dto.EngineDisplacement;
            car.Fuel = Enum.TryParse(dto.Fuel, out FuelType fuelType) ? fuelType : FuelType.Gasoline;
            car.Transmission = Enum.TryParse(dto.Transmission, out TransmissionType transmissionType) ? transmissionType : TransmissionType.Manual;
            car.Drivetrain = Enum.TryParse(dto.Drivetrain, out DrivetrainType drivetrainType) ? drivetrainType : DrivetrainType.FWD;
            car.InspectionMonth = dto.InspectionMonth;
            car.InspectionYear = dto.InspectionYear;
            car.Description = dto.Description;
            car.UpdatedAt = DateTime.Now;

            _context.Cars.Update(car);

            await _context.SaveChangesAsync();

            return car;
        }
    }
}
