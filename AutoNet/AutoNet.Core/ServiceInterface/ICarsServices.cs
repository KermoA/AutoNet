using AutoNet.Core.Domain;
using AutoNet.Core.Dto;

namespace AutoNet.Core.ServiceInterface
{
	public interface ICarsServices
	{
        Task<Car> DetailAsync(Guid id);
        Task<Car> Create(CarDto dto, string userName);
        Task<Car> Update(CarDto dto, string userName);
        Task<bool> Delete(Guid carId, string userId);

    }
}
