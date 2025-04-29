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
        public async Task<IActionResult> UploadFile(string localFilePath)
        {
            return Ok (await _driveService.UploadFile(localFilePath));
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string fileName, string localFilePath)
        {

            await _driveService.DownloadFile(fileName, localFilePath);

            // Возвращаем файл пользователю (если нужно сразу скачать в браузере)
            var fileBytes = await System.IO.File.ReadAllBytesAsync(localFilePath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(localFilePath));
        }
        [HttpGet("files")]
        public async Task<IActionResult> GetFileNamesInFolder()
        {
            try
            {
                var fileNames = await _driveService.GetFileNamesInFolder();
                return Ok(fileNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
    }
    public class GoogleDriveUploadModel
    {
        public IFormFile File { get; set; }
        public string FolderId { get; set; }
    }
}
