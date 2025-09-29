namespace Ovia.Services.SendEmails
{
    public interface IMailingServices
    {
        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachment = null);
        Task SendPasswordResetEmail(string userEmail, string name, string newPassword);
        Task SendEmailToProfessor(string userEmail, string name, string referId, string password);
        Task SendWelcomeEmail(string userEmail, string username, string referId, string password);
        Task SendOTP(string userEmail, string username, string otp);
        Task SendEmailToSponsor(string userEmail, string username, string sponsorId, string childName, string childId);
        Task SendEmailSupportTicket(string email, string EventName,string FullName, string Email1, string phone, string Message);
        Task SendEmailToSponsorWhenHisChildBuyPackage(string userEmail, string username, string ChildReferId, decimal profit, string childName);
        Task sendEmailToCustomerWhenAdminConvertHisMoney(string userEmail, string username, decimal Comission, DateTime date);
        Task sendSubscribeEmail(string userEmail);  
        Task SendTicketSupportReply(string UserEmail , string Reply, IFormFile file);
        Task SendEventTicket(string UserEmail , string Name,string TicketNumber);


    }
}
