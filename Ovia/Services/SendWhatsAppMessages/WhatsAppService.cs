using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Ovia.Services.SendWhatsAppMessages
{
    public class WhatsAppService
    {
        private readonly string _accountSid;
        private readonly string _authToken;

        public WhatsAppService(string accountSid, string authToken)
        {
            _accountSid = accountSid;
            _authToken = authToken;
        }

        public async Task<bool> SendMessage(string mobile, string message)
        {
            try
            {
                // Initialize Twilio client
                TwilioClient.Init(_accountSid, _authToken);

                // Use your Twilio number as the "From" number
                var fromNumber = new PhoneNumber("whatsapp:+13347216210");

                // Construct a new PhoneNumber instance for the recipient's WhatsApp number
                var toNumber = new PhoneNumber("whatsapp:" + mobile);

                // Create message options
                var messageOptions = new CreateMessageOptions(toNumber);

                // Set message options properties
                messageOptions.MessagingServiceSid = "SK009637f8758f52e411ba8e21f92f0066"; // If you're using a messaging service
                messageOptions.From = fromNumber;
              //  messageOptions.To = toNumber;
                messageOptions.Body = message;

                // Send message via Twilio
                var messageResult = await MessageResource.CreateAsync(messageOptions);

                Console.WriteLine($"Message sent: {messageResult.Sid}");

                // Return true if message was sent successfully
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending WhatsApp message: {ex.Message}");
                // Return false if there was an error sending the message
                return false;
            }
        }
    }
}
