using AutoNet.Core.Domain;
using AutoNet.Core.Dto;

namespace AutoNet.Core.ServiceInterface
{
    public interface IFileServices
    {
        void UploadFilesToDatabase(CarDto dto, Car domain);
        Task<FileToDatabase> RemoveImageFromDatabase(FileToDatabaseDto dto);
        Task<List<FileToDatabase>> RemoveImagesFromDatabase(FileToDatabaseDto[] dtos);
        List<FileToDatabase> GetCarFiles();


    }
}
