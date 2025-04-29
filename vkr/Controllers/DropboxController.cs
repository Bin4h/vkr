using Microsoft.AspNetCore.Mvc;
using vkr.Services;
using System.Threading.Tasks;

namespace vkr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DropboxController : ControllerBase
    {
        private readonly DropboxService _dropboxService;

        public DropboxController(DropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(string localPath, string filePath)
        {
            var result = await _dropboxService.UploadFile(localPath, filePath);
            return Ok(result);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string filePath)
        {

            var fileContent = await _dropboxService.DownloadFile(filePath);
            return File(fileContent, "application/octet-stream");
        }
    }
}
