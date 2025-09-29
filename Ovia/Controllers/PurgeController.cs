using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ovia.Services.Streaming_live;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurgeController : ControllerBase
    {
        private readonly BunnyCdnApiClient _bunnyCdnClient;
        private readonly LiveStreamingService _liveStreamingService;

        public PurgeController(BunnyCdnApiClient bunnyCdnClient, LiveStreamingService liveStreamingService)
        {
            _bunnyCdnClient = bunnyCdnClient;
            _liveStreamingService = liveStreamingService;
        }

        [HttpPost]
        [Route("purge")]
        public async Task<IActionResult> PurgeContent(string url)
        {
            try
            {
                await _bunnyCdnClient.PurgeZoneCacheAsync("Europe (Falkenstein)", url);
                return Ok("Content purged successfully.");
            }
            catch (HttpRequestException ex)
            {
                // Handle request exception
                return StatusCode(500, $"Failed to purge content: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("startStreaming")]
        public async Task<IActionResult> StartStreaming(string streamId)
        {
            try
            {
                await _liveStreamingService.StartStreaming(streamId);
                return Ok("Live video streaming started successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to start live video streaming: {ex.Message}");
            }
        }



        [HttpPost]
        [Route("stopStreaming")]
        public async Task<IActionResult> StopStreaming(string streamId)
        {
            try
            {
                await _liveStreamingService.StopStreaming(streamId);
                return Ok("Live video streaming stopped successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to stop live video streaming: {ex.Message}");
            }
        }


    }
}
