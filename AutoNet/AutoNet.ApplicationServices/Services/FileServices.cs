using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AutoNet.ApplicationServices.Services
{
    public class FileServices : IFileServices
    {
        private readonly IHostEnvironment _webHost;
        private readonly AutoNetContext _context;

        public FileServices(IHostEnvironment webHost, AutoNetContext context)
        {
            _webHost = webHost;
            _context = context;
        }

        public void UploadFilesToDatabase(CarDto dto, Car domain)
        {
            if (dto?.Files == null || dto.Files.Count == 0)
            {
                return;
            }

            foreach (var image in dto.Files)
            {
                if (image != null && image.Length > 0)
                {
                    using (var target = new MemoryStream())
                    {
                        image.CopyTo(target);

                        var fileToDatabase = new FileToDatabase
                        {
                            Id = Guid.NewGuid(),
                            ImageTitle = !string.IsNullOrWhiteSpace(image.FileName) ? image.FileName : "Untitled",
                            ImageData = target.ToArray(),
                            CarId = domain.Id
                        };

                        _context.FileToDatabases.Add(fileToDatabase);
                    }
                }
            }

            _context.SaveChanges();
        }

        public async Task<FileToDatabase> RemoveImageFromDatabase(FileToDatabaseDto dto)
        {
            if (dto == null || dto.Id == Guid.Empty)
            {
                return null;
            }

            var image = await _context.FileToDatabases
                .Where(x => x.Id == dto.Id)
                .FirstOrDefaultAsync();

            if (image != null)
            {
                _context.FileToDatabases.Remove(image);
                await _context.SaveChangesAsync();
            }

            return image;
        }

        public async Task<List<FileToDatabase>> RemoveImagesFromDatabase(FileToDatabaseDto[] dtos)
        {
            if (dtos == null || dtos.Length == 0)
            {
                return new List<FileToDatabase>();
            }

            var imagesToRemove = new List<FileToDatabase>();

            foreach (var dto in dtos)
            {
                var image = await _context.FileToDatabases
                    .Where(x => x.Id == dto.Id)
                    .FirstOrDefaultAsync();

                if (image != null)
                {
                    imagesToRemove.Add(image);
                }
            }

            if (imagesToRemove.Any())
            {
                _context.FileToDatabases.RemoveRange(imagesToRemove);
                await _context.SaveChangesAsync();
            }

            return imagesToRemove;
        }

        public List<FileToDatabase> GetCarFiles()
        {
            return _context.FileToDatabases.ToList() ?? new List<FileToDatabase>();
        }
    }
}
