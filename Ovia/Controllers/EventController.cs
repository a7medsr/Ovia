using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services.SendEmails;
using Twilio.Jwt.AccessToken;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly MomEntity momDb;
        private readonly IMailingServices mailingServices;
        public EventController(MomEntity _momDb, IMailingServices _mailingServices)
        {
            momDb = _momDb;
            mailingServices = _mailingServices;
        }




        [HttpGet]
        [Route("GetEventTicketById")]
        public async Task<IActionResult> GetEventTicketById(string ticketId)
        {
            var ticket = await momDb.EventGuest.FirstOrDefaultAsync(t => t.TicketID == ticketId);
            if(ticket  == null) { return NotFound("Ticket not found"); }
            return Ok(ticket);  
        }


        [HttpGet]
        [Route("GetCustomerTicketByCustomerId")]
        public async Task<IActionResult> GetCustomerTicketByCustomerId(int customerId)
        {
            var tickets = await momDb.EventGuest.Where(t => t.CustomerID == customerId)
                .ToListAsync();
            if (tickets.Count == null) { return NotFound("Not found tickets for you"); }
            return Ok(tickets);
        }




        [HttpPost]
        [Route("BuyEvent")]
        public async Task<IActionResult> BuyEvent(BuyEventDTO dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isEmailFound = IsEmailFound(dto.Email);
            if (isEmailFound == true)
                return BadRequest("This email buy ticket before, please use another email");
            var token = GetToken(dto.Token);
            if (token == null)
                return BadRequest("Token not found");

            if(token.IsUsed == true)
                return BadRequest("Token is used before");
             if(token.Value != 2)
                return BadRequest("Token price not equal event price");


            string ticketid;
            EventGuest existingTicketID;

            do
            {
                ticketid = GenerateRandomTicketID(8);
                existingTicketID = await momDb.EventGuest
                    .FirstOrDefaultAsync(x => x.TicketID == ticketid);
            } while (existingTicketID != null);



            var mobile = GetWhatsAppNumber(dto.Mobile);
            ChangeTokenStatusToUsed(dto.Token);
            var eventData = new EventGuest
            {
                Id = 0,
                CustomerID = dto.CustomerID,
                CustomerEventID = dto.CustomerEventID,
                TicketID =ticketid,
                Name =dto.Name,
                Email =dto.Email,
                Mobile = mobile,
                Token = dto.Token,
                Value =token.Value,
            };

            momDb.EventGuest.Add(eventData);
            await momDb.SaveChangesAsync();

            await mailingServices.SendEventTicket(eventData.Email, eventData.Name, eventData.TicketID);

            return Ok(eventData);


        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private PayToken GetToken(string token)
        {
            var tokenValue = momDb.PayTokens.FirstOrDefault(t => t.Token == token);

            if (tokenValue != null)
                return tokenValue;
            return null;
        }
       
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool IsEmailFound(string Email)
        {
            var existingEmail = momDb.EventGuest.FirstOrDefault(t => t.Email == Email);

            if (existingEmail != null)
                return true;
            return false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private PayToken ChangeTokenStatusToUsed(string token)
        {
            var tokenValue = momDb.PayTokens.FirstOrDefault(t => t.Token == token);

            if (tokenValue != null)
            {
                tokenValue.IsUsed = true; // Changed '==' to '=' to assign true to IsUsed
                momDb.SaveChanges();
            }

            return null; // Returning null as per the method signature, but consider returning appropriate response or type
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private string GenerateRandomTicketID(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            var token = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return token;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private string GetWhatsAppNumber(string mobileNumber)
        {
            string whatsappnumber = mobileNumber;
            if (mobileNumber.StartsWith("+2"))
                whatsappnumber = mobileNumber;
            else if (mobileNumber.StartsWith("2"))
                whatsappnumber = "+" + mobileNumber;
            else if (mobileNumber.StartsWith("0"))
                whatsappnumber = "+2" + mobileNumber;

            return whatsappnumber;

        }



        









    }
}
