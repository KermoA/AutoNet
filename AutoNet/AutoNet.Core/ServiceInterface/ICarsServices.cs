using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AutoNet.Core.ServiceInterface
{
	public interface ICarsServices
	{
        Task<Car> DetailAsync(Guid id);
        Task<Car> Create(CarDto dto, string userName);
        Task<Car> Update(CarDto dto);
        Task<bool> Delete(Guid carId, string userId);

    }
}
