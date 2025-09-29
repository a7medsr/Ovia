namespace Ovia.Models
{
    public partial class NsStartingBalance
    {
        public int Id { get; set; }
        public int? NsId { get; set; }   //userId
        public string?   Type { get; set; }
        public decimal? DailyStartingBalance { get; set; }
        public decimal? paid { get; set; }

        public decimal? Remain { get; set; }
        public bool? IsActive { get; set; }
    }

}
