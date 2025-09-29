using System.ComponentModel.DataAnnotations;

namespace Ovia.DTO
{
    public class GetCustomerDataDTO
    {

        public int Id { get; set; }

        public string? Username { get; set; }
        
        public string Email { get; set; }

        public string? BackOfficeId { get; set; }
        public string? SponsorId { get; set; }

        public string? Mobile { get; set; }
        public string? whatsappmobile { get; set; }
        public string? NationalId { get; set; }
        public string? Gender { get; set; }
        public string? NameEn { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? StartDate { get; set; }

        public string? Role { get; set; }
        public string? PictureUrl { get; set; }
        public string? CountryName { get; set; }
        public bool? Binance_Account { get; set; }
        public bool? IsApproved_BinanceAccount { get; set; }
        public bool? AllowToCreate { get; set; }
      //  public string? Governrate { get; set; }
       // public int? TeamId { get; set; }
      //  public int? RanksID { get; set; }
    //    public int? RanksIDold { get; set; }

     //   public bool? ISEdited { get; set; }
      //  public DateTime? LeadExpiry { get; set; }
    //    public bool? ISEnglish { get; set; }
    //    public bool? ISDark { get; set; }
    //    public bool? ISEmailconfirmed { get; set; }


    }
}
