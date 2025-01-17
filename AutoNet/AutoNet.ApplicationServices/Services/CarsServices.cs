﻿using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Core.ViewModels;
using AutoNet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser == null)
            {
                throw new Exception($"User with username {userName} not found.");
            }

            Car car = new()
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
                UpdatedAt = DateTime.UtcNow,
                UserId = currentUser.Id,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice
            };

            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();

            if (dto.Files != null && dto.Files.Any())
            {
                _fileServices.UploadFilesToDatabase(dto, car);
            }

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

        public async Task<List<string>> GetMakesAsync()
        {
            return await _context.Cars.Select(c => c.Make).Distinct().ToListAsync();
        }

        public async Task<List<string>> GetModelsByMakeAsync(string make)
        {
            return await _context.Cars
                .Where(c => c.Make == make)
                .Select(c => c.Model)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Car>> SearchCarsAsync(CarSearchViewModel searchModel)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.SelectedMake))
                query = query.Where(c => c.Make == searchModel.SelectedMake);

            if (!string.IsNullOrEmpty(searchModel.SelectedModel))
                query = query.Where(c => c.Model == searchModel.SelectedModel);

            if (searchModel.YearFrom.HasValue)
                query = query.Where(c => c.Year >= searchModel.YearFrom);

            if (searchModel.YearTo.HasValue)
                query = query.Where(c => c.Year <= searchModel.YearTo);

            if (searchModel.PowerFrom.HasValue)
                query = query.Where(c => c.Power >= searchModel.PowerFrom);

            if (searchModel.PowerTo.HasValue)
                query = query.Where(c => c.Power <= searchModel.PowerTo);

            if (searchModel.PriceFrom.HasValue)
                query = query.Where(c => c.Price >= searchModel.PriceFrom);

            if (searchModel.PriceTo.HasValue)
                query = query.Where(c => c.Price <= searchModel.PriceTo);

            if (searchModel.MileageFrom.HasValue)
                query = query.Where(c => c.Mileage >= searchModel.MileageFrom);

            if (searchModel.MileageTo.HasValue)
                query = query.Where(c => c.Mileage <= searchModel.MileageTo);

            if (searchModel.Fuel.HasValue)
                query = query.Where(c => c.Fuel == searchModel.Fuel);

            if (searchModel.Transmission.HasValue)
                query = query.Where(c => c.Transmission == searchModel.Transmission);

            if (searchModel.Drivetrain.HasValue)
                query = query.Where(c => c.Drivetrain == searchModel.Drivetrain);

            return await query.ToListAsync();
        }
    }
}
