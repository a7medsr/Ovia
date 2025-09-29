namespace Ovia.DTO
{
    public class Customer_DataIn_Hold_Tank_DTO
    {
        public int? customerAttributeId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? BackOfficeId { get; set; }
        public string? Status { get; set; }
        public bool? HasParent { get; set; }

        // Define the Children property
        public List<Customer_DataIn_Hold_Tank_DTO>? Children { get; set; }

    }
}
