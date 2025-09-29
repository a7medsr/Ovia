namespace Ovia.DTO
{
    public class AddProfitDTO
    {
        public int  SponsorId { get; set; }
        public decimal  Bonus_Comission { get; set; }
        public bool  IsPaid { get; set; }
        public int  PointProcessId { get; set; }
        public int  ProcessTypeId { get; set; }

    }
}
