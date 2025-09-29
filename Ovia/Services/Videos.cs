using Ovia.DTO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ovia.Services
{
    public class Videos
    {
        public OTPDTO GetVideoOTPById(string videoId)
        {
          
            RestClient client =
                new RestClient("https://dev.vdocipher.com/api/videos/"+videoId+"/otp");

            client
                .AddDefaultHeader("Authorization", "Apisecret Cf4y6Kkdl2GNQOen9vYwBCZYmepJL4TDFK4hHukbxxTn9kNiWW0ls2HRRcHB1zIC");
            
            var res = new OTPDTO();
            res = CallCustomerAPI.CallOutAPIWithAuth(client, new OTPDTO(), 0).Result;

            if (string.IsNullOrEmpty(res.otp) || string.IsNullOrEmpty(res.otp))
                return null;
            return res;
        }
    }
}
