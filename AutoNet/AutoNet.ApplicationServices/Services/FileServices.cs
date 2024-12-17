using AutoNet.Core.Domain;
using AutoNet.Core.Dto;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Xml;

namespace AutoNet.ApplicationServices.Services
{
    public class FileServices : IFileServices
    {
        private readonly IHostEnvironment _webHost;
        private readonly AutoNetContext _context;

        public FileServices
            (
                IHostEnvironment webHost,
                AutoNetContext context
            )
        {
            _webHost = webHost;
            _context = context;
        }

        public void UploadFilesToDatabase(CarDto dto, Car domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var image in dto.Files)
                {
                    using (var target = new MemoryStream())
                    {
                        FileToDatabase files = new FileToDatabase()
                        {
                            Id = Guid.NewGuid(),
                            ImageTitle = image.FileName,
                            CarId = domain.Id
                        };

                        image.CopyTo(target);
                        files.ImageData = target.ToArray();

                        _context.FileToDatabases.Add(files);
                    }
                }
            }
        }

        public async Task<FileToDatabase> RemoveImageFromDatabase(FileToDatabaseDto dto)
		{
			var image = await _context.FileToDatabases
				.Where(x => x.Id == dto.Id)
				.FirstOrDefaultAsync();

			_context.FileToDatabases.Remove(image);
			await _context.SaveChangesAsync();

			return image;
		}

		public async Task<FileToDatabase> RemoveImagesFromDatabase(FileToDatabaseDto[] dtos)
		{
			foreach (var dto in dtos)
			{
				var image = await _context.FileToDatabases
					.Where(x => x.Id == dto.Id)
					.FirstOrDefaultAsync();

				_context.FileToDatabases.Remove(image);
				await _context.SaveChangesAsync();
			}

			return null;
		}

		public List<FileToDatabase> GetCarFiles()
		{
			return _context.FileToDatabases.ToList();
		}
	}
}
