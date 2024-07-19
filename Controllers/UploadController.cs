using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Video_Catalogue_Challenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly string _mediaFolderPath;
        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
            _mediaFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media");
            if (!Directory.Exists(_mediaFolderPath))
            {
                Directory.CreateDirectory(_mediaFolderPath);
            }
        }
        // HTTP Post to handle chunk upload
        [HttpPost("chunk")]
        public async Task<IActionResult> UploadChunk([FromForm] IFormFile file, [FromForm] int chunkIndex, [FromForm] int totalChunks, [FromForm] long totalSize)
        {
            try
            {
                _logger.LogInformation("Received chunk {ChunkIndex}/{TotalChunks} for file {FileName}", chunkIndex + 1, totalChunks, file.FileName);
                
                // Check if file is valid

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Invalid file received.");
                    return BadRequest(new { message = "Invalid file." });
                }

                // Check for total size

                if (totalSize > 200 * 1024 * 1024) // Check total size of the file
                {
                    _logger.LogWarning("File size exceeds the 200 MB limit.");
                    return BadRequest(new { message = "File size exceeds the 200 MB limit." });
                }

                // Check if the file extension is .mp4
                if (Path.GetExtension(file.FileName).ToLower() != ".mp4")
                {
                    _logger.LogWarning("Invalid file type: {FileName}", file.FileName);
                    return BadRequest(new { message = "Only MP4 files are allowed." });
                }

                // temporary file path to store chunk
                var tempFilePath = Path.Combine(_mediaFolderPath, file.FileName + ".tmp");

                // Apend or create the temp files 
                using (var stream = new FileStream(tempFilePath, chunkIndex == 0 ? FileMode.Create : FileMode.Append))
                {
                    await file.CopyToAsync(stream);
                }

                // Checking if final then renaming and adding to the media folder
                if (chunkIndex == totalChunks - 1)
                {
                    var finalFilePath = Path.Combine(_mediaFolderPath, file.FileName);
                    System.IO.File.Move(tempFilePath, finalFilePath);
                    _logger.LogInformation("File uploaded successfully: {FileName}", file.FileName);
                }

                _logger.LogInformation("File {FileName} uploaded successfully", file.FileName);
                return Ok(new { message = "Chunk uploaded successfully" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, "File upload failed. Please try again.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "File upload failed. Please try again" });
            }
        }
    }
}
