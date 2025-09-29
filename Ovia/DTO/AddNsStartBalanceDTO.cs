namespace Ovia.DTO
{
    public class AddNsStartBalanceDTO
    {
        public int AdminId { get; set; }
        public int? NsId { get; set; }   //userId
        public string? Type { get; set; }
        public decimal? DailyStartingBalance { get; set; }
    }
}
