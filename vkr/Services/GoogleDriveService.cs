using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace vkr.Services
{
    public class GoogleDriveService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _baseUrl;

        public GoogleDriveService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _accessToken = GetAccessToken(configuration).Result;
            _baseUrl = configuration["GoogleDrive:ApiUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<string> GetAccessToken(IConfiguration configuration)
        {
            var clientId = configuration["GoogleDrive:client_id"];
            var clientSecret = configuration["GoogleDrive:client_secret"];
            var refreshToken = configuration["GoogleDrive:refresh_token"];

            var request = new HttpRequestMessage(HttpMethod.Post, configuration["GoogleDrive:RequestUrl"]);
            request.Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString();
        }
        public async Task<string> UploadFile(string localFilePath, string driveFolderId = null)
        {
            var fileName = Path.GetFileName(localFilePath);
            var fileContent = await File.ReadAllBytesAsync(localFilePath);

            using var content = new MultipartFormDataContent
        {
            { new StringContent("{\"name\":\"" + fileName + "\"" +
                (driveFolderId != null ? ",\"parents\":[\"" + driveFolderId + "\"]" : "") +
                "}", Encoding.UTF8, "application/json"), "metadata" },
            { new ByteArrayContent(fileContent), "file", fileName }
        };

            var response = await _httpClient.PostAsync($"{_baseUrl}/files?uploadType=multipart", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            return doc.RootElement.GetProperty("id").GetString();
        }
        public async Task DownloadFile(string fileId, string localSavePath)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/files/{fileId}?alt=media");
            response.EnsureSuccessStatusCode();

            await using var fileStream = new FileStream(localSavePath, FileMode.Create);
            await response.Content.CopyToAsync(fileStream);
        }
    }
}
