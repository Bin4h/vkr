using Microsoft.AspNetCore.Mvc;
using vkr.Services;

namespace vkr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RAID5Controller : Controller
    {
        public readonly RAID5Service _raid5Service;
        public RAID5Controller(RAID5Service raid5Service)
        {
            _raid5Service = raid5Service;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> WriteData(string filePath)
        {
            var result = await _raid5Service.WriteData(filePath);
            return Ok(result);
        }
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> ReadData(string fileName)
        {
            var fileContent = await _raid5Service.ReadData(fileName);
            return File(fileContent, "application/octet-stream");
        }
        [HttpGet("downloadwithoutya")]
        public async Task<IActionResult> ReadDataWithoutYandex(string fileName)
        {
            var fileContent = await _raid5Service.ReadDataWithoutYandex(fileName);
            return File(fileContent, "application/octet-stream");
        }
        [HttpGet("downloadwithoutgoogle")]
        public async Task<IActionResult> ReadDataWithoutGoogle(string fileName)
        {
            var fileContent = await _raid5Service.ReadDataWithoutGoogle(fileName);
            return File(fileContent, "application/octet-stream");
        }
        [HttpGet("downloadwithoutdropbox")]
        public async Task<IActionResult> ReadDataWithoutDropbox(string fileName)
        {
            var fileContent = await _raid5Service.ReadDataWithoutDropbox(fileName);
            return File(fileContent, "application/octet-stream");
        }
    }
}
