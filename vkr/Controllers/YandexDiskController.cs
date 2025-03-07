using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using vkr.Services;

namespace vkr.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class YandexDiskController : ControllerBase
    {
        private readonly YandexDiskService _yandexDiskService;

        public YandexDiskController(YandexDiskService yandexDiskService)
        {
            _yandexDiskService = yandexDiskService;
        }
        /// <summary>
        /// Метод для загрузки файла на Яндекс диск
        /// </summary>
        /// <param name="filePath">Путь для сохранения файла на диске</param>
        /// <param name="fileContent">Содержимое файла в виде массива байтов</param>
        /// <returns>Статус запроса</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(string filePath, [FromBody] byte[] fileContent)
        {
            var result = await _yandexDiskService.UploadFile(filePath, fileContent);
            return Ok(result);
        }
        /// <summary>
        /// Метод для загрузки файла с Яндекс диска
        /// </summary>
        /// <param name="filePath">Путь к файлу на Яндекс диске</param>
        /// <returns>Файл приобразованный из массива байтов</returns>
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string filePath)
        {
            var fileContent = await _yandexDiskService.DownloadFile(filePath);
            return File(fileContent, "application/octet-stream");
        }
    }
}
