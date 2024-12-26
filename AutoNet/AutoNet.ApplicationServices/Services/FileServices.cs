using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            if (dto?.Files != null && dto.Files.Count > 0)
            {
                foreach (var image in dto.Files)
                {
                    if (image != null)
                    {
                        using (var target = new MemoryStream())
                        {
                            var files = new FileToDatabase()
                            {
                                Id = Guid.NewGuid(),
                                ImageTitle = image.FileName ?? "Untitled",
                                CarId = domain.Id
                            };

                            image.CopyTo(target);
                            files.ImageData = target.ToArray();

                            _context.FileToDatabases.Add(files);
                        }
                    }
                }
                _context.SaveChanges();
            }
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
