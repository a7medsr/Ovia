using System;
using System.Collections.Generic;

namespace Ovia.Models
{
    public partial class RequestsCards : Entity
    {
        public int Id { get; set; }

        public int? RequestTypeId { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsUsed { get; set; }
        public string CardId { get; set; }
        public string? AccNo { get; set; }
        public int? BankId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ResquestDate { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpCreateDate { get; set; }
        public bool? OtpActive { get; set; }




        public RequestType RequestType { get; set; }
        public RequestsBanks Bank { get; set; }

    }
}
