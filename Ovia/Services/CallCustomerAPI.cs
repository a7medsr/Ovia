using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ovia.Services
{
  public static  class CallCustomerAPI
    {
        public static HttpResponseMessage CallAPI<T>(string URL, T data)
        {

        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            var JsonData = JsonConvert.SerializeObject(data);
            //  data response.
            HttpResponseMessage response = client.PostAsync(URL,new StringContent (JsonData,Encoding.UTF8, "application/json")).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
          
            client.Dispose();
            return response;
        }
        public async static Task<T> CallOutAPIWithAuth<T>(RestClient client, T result, int method )
        {
          

            var request = new RestRequest((Method)method);

            var response = client.Execute<T>(request);
            if (response.IsSuccessful)
            {
              
                result =response.Data;  

            }





            return result;
        }
    

    }
}
