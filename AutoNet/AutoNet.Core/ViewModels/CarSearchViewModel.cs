using AutoNet.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace AutoNet.Core.ViewModels
{
    public class CarSearchViewModel
    {
        public List<string> Makes { get; set; } = new List<string>();
        public List<string> Models { get; set; } = new List<string>();
        public string? SelectedMake { get; set; }
        public string? SelectedModel { get; set; }

        [Range(1886, 2100)]
        public int? YearFrom { get; set; }

        [Range(1886, 2100)]
        public int? YearTo { get; set; }

        public int? PowerFrom { get; set; }
        public int? PowerTo { get; set; }

        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }

        public int? MileageFrom { get; set; }
        public int? MileageTo { get; set; }

        public FuelType? Fuel { get; set; }
        public TransmissionType? Transmission { get; set; }
        public DrivetrainType? Drivetrain { get; set; }

        public List<FuelType> Fuels => Enum.GetValues(typeof(FuelType)).Cast<FuelType>().ToList();
        public List<TransmissionType> Transmissions => Enum.GetValues(typeof(TransmissionType)).Cast<TransmissionType>().ToList();
        public List<DrivetrainType> Drivetrains => Enum.GetValues(typeof(DrivetrainType)).Cast<DrivetrainType>().ToList();

        public List<LatestCarViewModel> LatestCars { get; set; } = new List<LatestCarViewModel>();
    }

    public class LatestCarViewModel
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Price { get; set; }
        public int? DiscountPrice { get; set; }
        public byte[] ImageData { get; set; }
    }

    public class SearchResultsViewModel
    {
        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
		public string SortOrder { get; set; }
	}
}
