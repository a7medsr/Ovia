namespace Ovia.DTO
{
    public class AddBackageDTO
    {

        public int PackageTypeId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? File { get; set; }
        public decimal? SponsorDistributorToDistributor { get; set; }
        public decimal? BusinessValue { get; set; }



    }
}
