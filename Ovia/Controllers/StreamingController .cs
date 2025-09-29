using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ovia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        private Process _ffmpegProcess;

        [HttpPost("start")]
        public IActionResult StartStreaming()
        {
            if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
            {
                return BadRequest("Live streaming is already running.");
            }

            string ffmpegCommand = "ffmpeg -i input_video.mp4 -c:v libx264 -preset ultrafast -tune zerolatency -f flv rtmp://your_streaming_server/live/stream_key";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegCommand,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _ffmpegProcess = Process.Start(startInfo);
            _ffmpegProcess.WaitForExit();

            return Ok("Live streaming started");
        }

        [HttpPost("stop")]
        public IActionResult StopStreaming()
        {
            if (_ffmpegProcess == null || _ffmpegProcess.HasExited)
            {
                return BadRequest("No live streaming is currently running.");
            }

            // Stop FFmpeg process
            _ffmpegProcess.Kill();
            _ffmpegProcess.WaitForExit();

            return Ok("Live streaming stopped");
        }
    }
}
