using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace AutoNet.ApplicationServices.Services
{
	public class CarsServices : ICarsServices
	{
		private readonly AutoNetContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileServices _fileServices;

        public CarsServices
			(
				AutoNetContext context,
                UserManager<ApplicationUser> userManager,
                IFileServices fileServices
            )
		{
			_context = context;
            _userManager = userManager;
            _fileServices = fileServices;
        }

        public async Task<Car> DetailAsync(Guid id)
        {
            var result = await _context.Cars
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            return result;
        }

        public async Task<Car> Create(CarDto dto, string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser == null)
            {
                throw new Exception($"User with username {userName} not found.");
            }

            Car car = new();

            car.Id = Guid.NewGuid();
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
            car.CreatedAt = DateTime.UtcNow;
            car.UpdatedAt = DateTime.UtcNow;
            car.UserId = currentUser.Id;
            car.Price = dto.Price;
            car.DiscountPrice = dto.DiscountPrice;

            if (dto.Files != null)
            {
                _fileServices.UploadFilesToDatabase(dto, car);
            }

            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();

            return car;
		}

		public async Task<Car> Update(CarDto dto)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (car == null)
            {
                return null;
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
            car.UpdatedAt = DateTime.UtcNow;
            car.Price = dto.Price;
            car.DiscountPrice = dto.DiscountPrice;

			if (dto.Files != null)
			{
				_fileServices.UploadFilesToDatabase(dto, car);
			}

			_context.Cars.Update(car);
            await _context.SaveChangesAsync();

            return car;
        }

        public async Task<bool> Delete (Guid carId, string userId)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(x => x.Id == carId && x.UserId == userId);

            if (car == null)
            {
                throw new Exception($"Car with ID {carId} not found or does not belong to the current user.");
            }

			var images = await _context.FileToDatabases
				.Where(x => x.CarId == carId)
				.Select(y => new FileToDatabaseDto
				{
					Id = y.Id,
					ImageTitle = y.ImageTitle,
					CarId = y.CarId
				}).ToArrayAsync();

			await _fileServices.RemoveImagesFromDatabase(images);
			_context.Cars.Remove(car);
			await _context.SaveChangesAsync();

			return true;
        }
    }
}
