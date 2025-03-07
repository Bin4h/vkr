using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace vkr.Services
{
    /// <summary>
    /// Сервис для работы с Яндекс диском
    /// Позволяет загружать и скачивать файлы
    /// </summary>
    public class YandexDiskService
    {
        private readonly HttpClient _httpClient; // клиент для отправки запросов
        private readonly string _apiUrl; // URL API Яндекс диска
        private readonly string _oauthToken; // Токен авторизации
        /// <summary>
        /// Инициализация экземпляра класса
        /// </summary>
        /// <param name="configuration">Конфигурация, содержащая настройки для работы с API</param>
        public YandexDiskService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiUrl = configuration["YandexDisk:ApiUrl"]; // URL API из конфигурации
            _oauthToken = configuration["YandexDisk:OAuthToken"]; // Токен из конфигурации
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", _oauthToken); // Заголовок авторизации
        }
        /// <summary>
        /// Загрузка файла на Яндекс диск
        /// </summary>
        /// <param name="filePath">Путь к файлу на Яндекс диске</param>
        /// <param name="fileContent">Содержимое файла в виде массива байтов</param>
        /// <returns>Результат загрузки</returns>
        public async Task<string> UploadFile(string filePath, byte[] fileContent)
        {
            var uploadUrlResponse = await _httpClient.GetAsync($"{_apiUrl}resources/upload?path={filePath}&overwrite=true"); // URL для загрузки файла
            uploadUrlResponse.EnsureSuccessStatusCode(); // Статус запроса

            // Десериализация ответа с адресом для загрузки
            var uploadUrl = JsonConvert.DeserializeObject<YandexDiskUploadResponse>(await uploadUrlResponse.Content.ReadAsStringAsync()).Href;

            var content = new ByteArrayContent(fileContent); // Загрузка файла по полученному URL
            var uploadResponse = await _httpClient.PutAsync(uploadUrl, content);
            uploadResponse.EnsureSuccessStatusCode(); // Проверка, что загрузка прошла успешно

            return await uploadResponse.Content.ReadAsStringAsync(); // Результат загрузки
        }
        /// <summary>
        /// Скачивание файла с Яндекс диска
        /// </summary>
        /// <param name="filePath">Путь к файлу на яндекс диске</param>
        /// <returns>Содержимое файла в виде массива байтов</returns>
        public async Task<byte[]> DownloadFile(string filePath)
        {
            
            var downloadUrlResponse = await _httpClient.GetAsync($"{_apiUrl}resources/download?path={filePath}"); // URL для загрузки файла
            downloadUrlResponse.EnsureSuccessStatusCode(); // Статус запроса
            // Десериализация ответа с адресом для скачивания
            var downloadUrl = JsonConvert.DeserializeObject<YandexDiskDownloadResponse>(await downloadUrlResponse.Content.ReadAsStringAsync()).Href;

            var downloadResponse = await _httpClient.GetAsync(downloadUrl); // Скачивание файла по полученному URL
            downloadResponse.EnsureSuccessStatusCode(); // Проверка результата запроса

            return await downloadResponse.Content.ReadAsByteArrayAsync(); // Содержимое файла
        }
        /// <summary>
        /// Класс для десериализации ответа API при получении URL для загрузки файла
        /// </summary>
        private class YandexDiskUploadResponse
        {
            public string Href { get; set; } // URL для загрузки файла
        }
        /// <summary>
        /// Класс для десериализации ответа API при получении URL для скачивания файла
        /// </summary>
        private class YandexDiskDownloadResponse
        {
            public string Href { get; set; } // URL для скачивания файла
        }
    }
}
