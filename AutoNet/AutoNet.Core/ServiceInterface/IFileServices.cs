using AutoNet.Core.Domain;
using AutoNet.Core.Dto;

namespace AutoNet.Core.ServiceInterface
{
    public interface IFileServices
    {
        void UploadFilesToDatabase(CarDto dto, Car domain);
        List<FileToDatabase> GetCarFiles();
        Task<FileToDatabase> RemoveImageFromDatabase(FileToDatabaseDto dto);
		Task<FileToDatabase> RemoveImagesFromDatabase(FileToDatabaseDto[] dtos);

	}
}
