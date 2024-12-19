﻿using Microsoft.AspNetCore.Http;

namespace AutoNet.Core.Dto
{
	public class CarDto
	{
		public Guid? Id { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public int Year { get; set; }
		public string VIN { get; set; }
		public int Mileage { get; set; }
		public int Power { get; set; }
		public int EngineDisplacement { get; set; }
		public string Fuel { get; set; }
		public string Transmission { get; set; }
		public string Drivetrain { get; set; }
		public int InspectionMonth { get; set; }
		public int InspectionYear { get; set; }
		public string Description { get; set; }
        public int Price { get; set; }
        public int? DiscountPrice { get; set; }

        public List<IFormFile> Files { get; set; }
        public IEnumerable<FileToDatabaseDto> Image { get; set; }
            = new List<FileToDatabaseDto>();

        public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }


		public string EngineDisplacementInLiters => $"{EngineDisplacement / 1000.0:F1} L";
	}
}
