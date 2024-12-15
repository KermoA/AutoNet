using System.ComponentModel.DataAnnotations;

namespace AutoNet.Core.Domain
{
	public class Car
	{
		public Guid Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Make { get; set; }

		[Required]
		[StringLength(50)]
		public string Model { get; set; }

		[Range(1886, 2100)]
		public int Year { get; set; }

		[StringLength(17)]
		public string VIN { get; set; }

		[Range(0, int.MaxValue)]
		public int Mileage { get; set; }

		[Range(0, int.MaxValue)]
		public int Power { get; set; }

		[Range(50, 10000)]
		public int EngineDisplacement { get; set; }

		public string EngineDisplacementInLiters => $"{EngineDisplacement / 1000.0:F1} L";

		public FuelType Fuel { get; set; }
		public TransmissionType Transmission { get; set; }
		public DrivetrainType Drivetrain { get; set; }

		[Range(1, 12)]
		public int InspectionMonth { get; set; }

		[Range(1900, 2100)]
		public int InspectionYear { get; set; }
		public string InspectionValidFormatted => $"{InspectionMonth:D2}/{InspectionYear}";

		[StringLength(1000)]
		public string Description { get; set; }

		public Guid? UserId { get; set; }
		public ApplicationUser User { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}

	public enum FuelType { Gasoline, Diesel, Electric, Hybrid, LPG, CNG }
	public enum TransmissionType { Manual, Automatic, SemiAutomatic }
	public enum DrivetrainType { FWD, RWD, AWD, FourWD }
}
