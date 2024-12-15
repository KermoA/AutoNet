using AutoNet.Core.Domain;
using AutoNet.Core.Dto;

namespace AutoNet.Core.ServiceInterface
{
	public interface ICarsServices
	{
        Task<Car> Create(CarDto dto, string userName);

    }
}
