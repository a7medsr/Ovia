using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ovia.DTO;
using Ovia.Models;
using Ovia.Services.SendEmails;
using System.ComponentModel.DataAnnotations;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly MomEntity momDb;
        private readonly IMailingServices mailing;
        public SubscribeController(MomEntity _momDb, IMailingServices _mailing)
        {
            momDb = _momDb;
            mailing = _mailing;
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }


        [HttpPost("SubscribeUser")]
        public async Task<IActionResult> SubscribeUser(string email, string phone)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                return BadRequest("Invalid email format. Please provide a valid email address.");
            }

            if (string.IsNullOrEmpty(phone))
            {
                return BadRequest("Phone number cannot be empty.");
            }

            if (phone.Length < 11)
            {
                return BadRequest("Phone number must be at least 11 digits long.");
            }

           var existMail = await momDb.Subscribe.FirstOrDefaultAsync(s => s.Email == email);
            if(existMail != null)
            {
                return BadRequest("You subscribed before");
            }

           
            var subscribe = new Subscribe
            {
                Id = 0,
                Email = email,
                Phone = phone,
            };

            momDb.Subscribe.Add(subscribe);
            await momDb.SaveChangesAsync();

            await mailing.sendSubscribeEmail(email);

            return Ok("you Subscribed Successfully");

        }



        }
    }
