using Microsoft.AspNetCore.Mvc; // Пространство имён, предоставляющее базовую инфраструктуру для построения API-контроллеров
using vkr.Services; // Подключение пространства имён, в котором определён сервис RAID5Service, реализующий бизнес-логику

namespace vkr.Controllers // Объявление пространства имён, где находится контроллер
{
    /// <summary>
    /// API-контроллер, предоставляющий конечные точки для работы с функциональностью RAID 5:
    /// загрузка файлов, скачивание с восстановлением и получение расширений файлов.
    /// </summary>
    [ApiController] // Атрибут указывает, что класс является API-контроллером и включает автоматическую проверку моделей
    [Route("api/[controller]")] // Маршрут для всех методов контроллера: "api/RAID5"
    public class RAID5Controller : Controller
    {
        // Приватное поле, содержащее экземпляр сервиса RAID5Service, реализующего логику RAID5
        public readonly RAID5Service _raid5Service;

        /// <summary>
        /// Конструктор контроллера, получает внедрённую зависимость RAID5Service.
        /// </summary>
        public RAID5Controller(RAID5Service raid5Service)
        {
            _raid5Service = raid5Service;
        }

        /// <summary>
        /// Метод загрузки файла, его распределения по схеме RAID5 и сохранения.
        /// </summary>
        /// <param name="file">Файл, загружаемый пользователем</param>
        /// <param name="distributionMethod">Метод распределения данных по дискам (облакам)</param>
        /// <param name="key">Ключ шифрования файла</param>
        /// <returns>Результат операции в формате JSON</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> WriteData(IFormFile file, [FromForm] int distributionMethod, [FromForm] string key)
        {
            try
            {
                // Проверка: если файл не передан или пустой — вернуть ошибку
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Файл не загружен");
                }

                // Чтение файла в память
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream); // Асинхронное копирование содержимого файла в поток
                    var fileBytes = memoryStream.ToArray(); // Преобразование потока в массив байтов

                    // Вызов бизнес-логики: распределение данных по RAID5 и сохранение
                    var result = await _raid5Service.WriteData(
                        fileBytes,          // Содержимое файла
                        file.FileName,      // Имя файла
                        distributionMethod, // Метод распределения
                        key);               // Ключ шифрования

                    // Возврат успешного ответа с результатом
                    return Ok(new { success = true, data = result });
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений: возвращается ошибка 500 и сообщение об ошибке
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Метод для скачивания и восстановления файла из всех облаков согласно схеме RAID5.
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="distributionMethod">Метод распределения</param>
        /// <param name="key">Ключ для расшифровки файла</param>
        /// <returns>Файл в бинарном виде</returns>
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> ReadData(string fileName, [FromQuery] int distributionMethod, [FromQuery] string key)
        {
            // Получение содержимого файла, восстановленного с учётом схемы RAID5
            var fileContent = await _raid5Service.ReadData(fileName, distributionMethod, key);
            // Возврат файла в виде потока с MIME-типом application/octet-stream
            return File(fileContent, "application/octet-stream");
        }

        /// <summary>
        /// Метод восстановления и скачивания файла без использования облачного хранилища Яндекс.Диск.
        /// Имитирует отказ одного из дисков RAID5.
        /// </summary>
        [HttpGet("downloadwithoutya/{fileName}")]
        public async Task<IActionResult> ReadDataWithoutYandex(string fileName, [FromQuery] int distributionMethod, [FromQuery] string key)
        {
            var fileContent = await _raid5Service.ReadDataWithoutYandex(fileName, distributionMethod, key);
            return File(fileContent, "application/octet-stream");
        }

        /// <summary>
        /// Метод восстановления и скачивания файла без использования Google Диска.
        /// Используется для проверки корректности восстановления при отказе одного из источников.
        /// </summary>
        [HttpGet("downloadwithoutgoogle/{fileName}")]
        public async Task<IActionResult> ReadDataWithoutGoogle(string fileName, [FromQuery] int distributionMethod, [FromQuery] string key)
        {
            var fileContent = await _raid5Service.ReadDataWithoutGoogle(fileName, distributionMethod, key);
            return File(fileContent, "application/octet-stream");
        }

        /// <summary>
        /// Метод восстановления и скачивания файла без использования Dropbox.
        /// Аналогично, предназначен для имитации потери одного диска.
        /// </summary>
        [HttpGet("downloadwithoutdropbox/{fileName}")]
        public async Task<IActionResult> ReadDataWithoutDropbox(string fileName, [FromQuery] int distributionMethod, [FromQuery] string key)
        {
            var fileContent = await _raid5Service.ReadDataWithoutDropbox(fileName, distributionMethod, key);
            return File(fileContent, "application/octet-stream");
        }

        /// <summary>
        /// Метод получения расширения файла (например, ".txt", ".jpg").
        /// Используется для восстановления корректного имени файла при скачивании.
        /// </summary>
        /// <param name="fileName">Имя файла без расширения</param>
        /// <param name="distributionMethod">Метод распределения</param>
        /// <returns>Расширение файла в виде строки</returns>
        [HttpGet("getextention/{fileName}")]
        public async Task<IActionResult> GetFileExtention(string fileName, [FromQuery] int distributionMethod)
        {
            var extention = await _raid5Service.GetFileExtention(fileName, distributionMethod);
            return Ok(extention);
        }
    }
}
