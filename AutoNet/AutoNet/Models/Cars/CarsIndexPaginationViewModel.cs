namespace AutoNet.Models.Cars
{
    public class CarsIndexPaginationViewModel
    {
        public List<CarsIndexViewModel> Cars { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }

}
