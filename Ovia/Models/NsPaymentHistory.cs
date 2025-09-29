namespace Ovia.Models
{
    public class NsPaymentHistory : Entity
    {
        public int Id { get; set; }
        public int? NS_ID { get; set; }
        public decimal? Amount { get; set; }
        public int? Transfered_To_ID { get; set; }
        public string Transaction_ID { get; set; }
        public DateTime? Transaction_Date { get; set; }
        public bool? NS_Is_Paid { get; set; }
        public DateTime? NS_Paid_Date { get; set; }
        public bool? Accountant_Is_Paid { get; set; }
        public DateTime? Accountant_Payment_Date { get; set; }

    }
}
