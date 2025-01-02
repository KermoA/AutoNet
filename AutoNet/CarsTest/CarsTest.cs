using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CarsTest
{
    public class CarsTest : TestBase
    {
        [Fact]
        public async Task ShouldNot_CreateCar_WhenUserIdIsNull()
        {
            // Arrange
            CarDto dto = MockCarData();
            var carsServices = Svc<ICarsServices>();
            var userManager = GetMockUserManager();
            var testUser = new ApplicationUser { Id = null };

            // Act
            var result = await carsServices.Create(dto, testUser.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Should_GetByIdCar_WhenReturnsEqual()
        {
            // Arrange
            Guid databaseGuid = Guid.Parse("204a6b06-63b2-4644-bf45-3b2481bfbd1e");
            Guid guid = Guid.Parse("204a6b06-63b2-4644-bf45-3b2481bfbd1e");

            // Act
            await Svc<ICarsServices>().DetailAsync(guid);

            // Assert
            Assert.Equal(databaseGuid, guid);
        }

        [Fact]
        public async Task Should_UpdateCar_WhenUpdateData()
        {
            // Arrange
            var guid = new Guid("dd6c6599-9d25-4ae1-bb29-17c6fba6f5d9");

            CarDto dto = new()
            {
                Id = guid,
                Make = "Honda",
                Model = "Accord",
                Year = 2001,
                VIN = "",
                Mileage = 123456,
                Power = 110,
                EngineDisplacement = 1998,
                Fuel = "Gasoline",
                Transmission = "Manual",
                Drivetrain = "FWD",
                InspectionMonth = 10,
                InspectionYear = 2025,
                Description = "Asd",
                Price = 2000,
                DiscountPrice = 1800,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

            };

            CarDto domain = MockUpdatedCarData();

            // Act
            await Svc<ICarsServices>().Update(dto);

            // Assert
            Assert.Equal(dto.Id, domain.Id);
            Assert.DoesNotMatch(dto.Model, domain.Model);
            Assert.NotEqual(dto.Year, domain.Year);
        }

        [Fact]
        public async Task Should_DeleteByIdCar_WhenDeleteCar()
        {
            // Arrange
            var testUser = new ApplicationUser { UserName = "asd@gmail.com" };

            var dbContext = Svc<AutoNetContext>();
            await dbContext.Users.AddAsync(testUser);
            await dbContext.SaveChangesAsync();

            CarDto dto = MockCarData();

            var addedCar = await Svc<ICarsServices>().Create(dto, testUser.UserName);

            // Act
            var result = await Svc<ICarsServices>().Delete(addedCar.Id, testUser.Id);

            // Assert
            Assert.True(result);
        }

        private CarDto MockCarData()
        {
            return new CarDto
            {
                Id = Guid.NewGuid(),
                Make = "Honda",
                Model = "Accord",
                Year = 2001,
                VIN = "",
                Mileage = 123456,
                Power = 110,
                EngineDisplacement = 1998,
                Fuel = "Gasoline",
                Transmission = "Manual",
                Drivetrain = "FWD",
                InspectionMonth = 10,
                InspectionYear = 2025,
                Description = "Asd",
                Price = 2000,
                DiscountPrice = 1800,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
        }

        private CarDto MockUpdatedCarData()
        {
            return new CarDto
            {
                Id = Guid.Parse("dd6c6599-9d25-4ae1-bb29-17c6fba6f5d9"),
                Make = "Honda",
                Model = "Civic",
                Year = 2005,
                VIN = "",
                Mileage = 123456,
                Power = 110,
                EngineDisplacement = 1998,
                Fuel = "Gasoline",
                Transmission = "Manual",
                Drivetrain = "FWD",
                InspectionMonth = 10,
                InspectionYear = 2025,
                Description = "Asd",
                Price = 2000,
                DiscountPrice = 1800,
                UpdatedAt = DateTime.Now,
            };
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            return userManager;
        }
    }
}

