using AutoNet.ApplicationServices.Services;
using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CarsTest
{
    public class CarsTest : TestBase
    {
        [Fact]
        public async Task ShouldNot_CreateCar_WhenUserNameIsNull()
        {
            // Arrange
            CarDto dto = MockCarData();
            var carsServices = Svc<ICarsServices>(); // Get CarsServices from DI
            var userManager = GetMockUserManager();
            var testUser = new ApplicationUser { UserName = null }; // User with a null username

            // Act
            var result = await carsServices.Create(dto, testUser.UserName);

            // Assert
            Assert.Null(result);
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

