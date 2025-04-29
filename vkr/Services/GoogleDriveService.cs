using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using Google.Apis.Upload;
using System;

namespace vkr.Services
{
    public class GoogleDriveService
    {
        private readonly string credentialsPath;
        private readonly string folderId;
        public GoogleDriveService(IConfiguration configuration)
        {
            credentialsPath = "credentials.json";
            folderId = "1sxHqWNxEFt-5gVvqM_WtdUG1ZQaPFsiB";
        }
        public async Task<string> UploadFile(string localFilePath)
        {
            if (!File.Exists(localFilePath))
                throw new FileNotFoundException("File not found", localFilePath);
            var fileName = Path.GetFileName(localFilePath);
            var fileContent = await File.ReadAllBytesAsync(localFilePath);
            UserCredential credentials;
            var clientSecrets = await GoogleClientSecrets.FromFileAsync(credentialsPath);

            credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets.Secrets, new[] { DriveService.ScopeConstants.DriveFile }, "user", CancellationToken.None);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Raid5"
            });
            var fileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(localFilePath),
                Parents = new List<string> { folderId }
            };
            FilesResource.CreateMediaUpload request;

            using (var stream = new FileStream(localFilePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetaData, stream, "");
                request.Fields = "id";
                var progress = await request.UploadAsync();
                if (progress.Status != UploadStatus.Completed)
                    throw new Exception($"Upload failed: {progress.Exception?.Message}");
            }
            return request.ResponseBody.Name;
        }
        public async Task DownloadFile(string fileName, string localSavePath)
        {
            UserCredential credentials;
            var clientSecrets = await GoogleClientSecrets.FromFileAsync(credentialsPath);

            credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets.Secrets, new[] { DriveService.ScopeConstants.DriveFile }, "user", CancellationToken.None);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Raid5"
            });
            var fileId = await FindFileIdByName(fileName, service);
            var fileInfo = await service.Files.Get(fileId).ExecuteAsync();
            if (fileInfo == null)
                throw new Exception("File not found in Google Drive");
            using (var fileStream = new FileStream(localSavePath, FileMode.Create, FileAccess.Write))
            {
                var request = service.Files.Get(fileId);
                await request.DownloadAsync(fileStream);
            }
        }
        public async Task<List<string>> GetFileNamesInFolder()
        {
            UserCredential credentials;
            var clientSecrets = await GoogleClientSecrets.FromFileAsync(credentialsPath);

            credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets.Secrets,
                new[] { DriveService.ScopeConstants.DriveFile },
                "user",
                CancellationToken.None);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Raid5"
            });

            var request = service.Files.List();
            request.Q = $"'{folderId}' in parents and trashed = false";
            request.Fields = "files(name)";

            var result = await request.ExecuteAsync();
            return result.Files.Select(file => file.Name).ToList();
        }
        public async Task<string> FindFileIdByName(string fileName, DriveService service)
        {
            var request = service.Files.List();
            request.Q = $"name = '{fileName}' and trashed = false";
            request.Fields = "files(id, name)";

            var result = await request.ExecuteAsync();
            return result.Files.FirstOrDefault()?.Id;
        }
    }
}
