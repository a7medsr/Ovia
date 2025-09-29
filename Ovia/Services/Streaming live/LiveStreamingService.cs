public class LiveStreamingService
{
    private readonly HttpClient _httpClient;

    public LiveStreamingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //public async Task StartStreaming(string streamId)
    //{
    //    try
    //    {
    //        var requestUrl = $"https://dash.bunny.net/stream/{streamId}/api/start";
    //        var request = new HttpRequestMessage(HttpMethod.Put, requestUrl);

    //        var response = await _httpClient.SendAsync(request);
    //        response.EnsureSuccessStatusCode();
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        throw new Exception($"Failed to start live video streaming: {ex.Message}");
    //    }
    //}



    public async Task StartStreaming(string streamId)
    {
        try
        {
            var requestUrl = $"https://dash.bunny.net/stream/{streamId}/api/start";
            var response = await _httpClient.PostAsync(requestUrl, null);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to start live video streaming: {ex.Message}");
        }
    }



    public async Task StopStreaming(string streamId)
    {
        try
        {
       // https://dash.bunny.net/stream/215731/api
            var requestUrl = $"https://dash.bunny.net/stream/{streamId}/api/stop";
            var response = await _httpClient.PostAsync(requestUrl, null); // Use POST method instead
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to stop live video streaming: {ex.Message}");
        }
    }


}
