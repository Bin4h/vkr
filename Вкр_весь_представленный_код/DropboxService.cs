using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace vkr.Services
{
    public class DropboxService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _apiUrl;

        public DropboxService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _accessToken = configuration["Dropbox:AccessToken"];
            _apiUrl = configuration["Dropbox:ApiUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
        public async Task<string> UploadFile(string localFilePath, string dropboxPath)
        {

            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Файл не найден", localFilePath);
            }

            byte[] fileContent = File.ReadAllBytes(localFilePath);

            using (var content = new ByteArrayContent(fileContent))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}upload");

                request.Headers.Add("Dropbox-API-Arg",
                    JsonConvert.SerializeObject(new
                    {
                        path = dropboxPath,
                        mode = "overwrite",
                        autorename = true,
                        mute = false
                    }));

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<byte[]> DownloadFile(string dropboxPath)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}download");

            request.Headers.Add("Dropbox-API-Arg",
                JsonConvert.SerializeObject(new
                {
                    path = dropboxPath
                }));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<DropboxFileMetadata>(
                response.Headers.GetValues("Dropbox-API-Result").First());

            return await response.Content.ReadAsByteArrayAsync();
        }

        private class DropboxFileMetadata
        {
            public string name { get; set; }
            public string path_lower { get; set; }
            public long size { get; set; }
        }
    }
}
