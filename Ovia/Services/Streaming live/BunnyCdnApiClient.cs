namespace Ovia.Services.Streaming_live
{
    public class BunnyCdnApiClient
    {
        private readonly HttpClient _httpClient;

        public BunnyCdnApiClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://bunnycdn.com/api/");
            _httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);
        }



        public async Task PurgeZoneCacheAsync(string zoneName, string url)
        {
            try
            {
                var requestUrl = $"purge?zoneName={zoneName}&url={url}";
                var response = await _httpClient.PostAsync(requestUrl, null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                // Handle request exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }




    }
}
