namespace Ovia.DTO
{
    public class CashbackvisablityDTO
    {
        public DateTime datefrom { get; set; }
        public DateTime dateto { get; set; }
        public string day { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public string? Notes { get; set; }
    }
}
