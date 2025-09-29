namespace Ovia.DTO
{
    public class BuyPackageDTO
    {
        public List<string> Tokens { get; set; }
        public List<int> PackagesIds { get; set; }
        public int customerAttributeId { get; set; } 
    }
}
