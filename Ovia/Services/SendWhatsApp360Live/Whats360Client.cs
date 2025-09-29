using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ovia.DTO;
using Newtonsoft.Json;

namespace Ovia.Services.SendWhatsApp360Live
{
    public class Whats360Client
    {
        private readonly HttpClient _httpClient;

        public Whats360Client()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> SendMessage(SendWhatsDTO dto)
        {
            try
            {
                var url = "https://whatslive.online/api/user/v2/send_message";

                var body = new
                {
                    client_id = "momentum client id ",
                    mobile = dto.recipientPhoneNumber,
                    text = dto.message
                };

                var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiJiRDFQWWp2QXMwVEdHbDV4YUZLSldhOGpZZjhoNUh6UyIsInJvbGUiOiJ1c2VyIiwiaWF0IjoxNzEzMjE1MDczfQ.r4F8XY6vA08FpZSdKJQnuC_IrhMbsBjofYnqorkozHk"; // Replace with your actual API keys

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.PostAsJsonAsync(url, body);

                if (!response.IsSuccessStatusCode)
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return new OkObjectResult(responseData);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}











//using Microsoft.AspNetCore.Mvc;
//using Ovia.DTO;

//namespace Ovia.Services.SendWhatsApp360Live
//{
//    public class Whats360Client
//    {

//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;

//        public Whats360Client(string apiKey)
//        {
//            _httpClient = new HttpClient();
//            _apiKey = apiKey;
//        }


//        public async Task SendMessageAsync([FromForm] SendWhatsDTO dto)
//        {
//            var request = new HttpRequestMessage
//            {
//                Method = HttpMethod.Post,
//             //   RequestUri = new Uri("https://api.whats360.live/send-message"),
//                RequestUri = new Uri("https://v1.whats360.live/user/sent-text-message"),
//                Headers =
//            {
//                { "Authorization", $"Bearer {_apiKey}" }
//            },
//                Content = new FormUrlEncodedContent(new[]
//                {
//                new KeyValuePair<string, string>("recipient_phone_number", dto.recipientPhoneNumber),
//                new KeyValuePair<string, string>("message", dto.message)
//            })
//            };


//            // Send the HTTP request
//            var response = await _httpClient.SendAsync(request);

//            // Handle response here
//            if (response.IsSuccessStatusCode)
//            {
//                Console.WriteLine("Message sent successfully.");
//            }
//            else
//            {
//                Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
//            }
//        }




//    }
//}
