namespace AutoNet.Models.Cars
{
    public class CarDetailsViewModel
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
        public int? InspectionMonth { get; set; }
        public int? InspectionYear { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int? DiscountPrice { get; set; }

        public List<CarImageViewModel> Images { get; set; }
        = new List<CarImageViewModel>();
		public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }


        public string EngineDisplacementInLiters => $"{EngineDisplacement / 1000.0:F1}";
    }
}
