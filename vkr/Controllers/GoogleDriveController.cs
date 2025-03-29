using Microsoft.AspNetCore.Mvc;
using vkr.Services;

namespace vkr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleDriveController : ControllerBase
    {
        private readonly GoogleDriveService _driveService;

        public GoogleDriveController(GoogleDriveService driveService)
        {
            _driveService = driveService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] GoogleDriveUploadModel model)
        {

            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file uploaded");

            var tempFilePath = Path.GetTempFileName();
            await using (var stream = System.IO.File.Create(tempFilePath))
            {
                await model.File.CopyToAsync(stream);
            }
            var fileId = await _driveService.UploadFile(tempFilePath, model.FolderId);
            return Ok(new { FileId = fileId });
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileId)
        {

            var tempFilePath = Path.GetTempFileName();

            await _driveService.DownloadFile(fileId, tempFilePath);
            var fileStream = System.IO.File.OpenRead(tempFilePath);
            var fileName = Path.GetFileName(tempFilePath);

            Response.OnCompleted(() =>
            {
                fileStream.Dispose();
                System.IO.File.Delete(tempFilePath);
                return Task.CompletedTask;
            });

            return File(fileStream, "application/octet-stream", fileName);
        }
    }
    public class GoogleDriveUploadModel
    {
        public IFormFile File { get; set; }
        public string FolderId { get; set; }
    }
}
