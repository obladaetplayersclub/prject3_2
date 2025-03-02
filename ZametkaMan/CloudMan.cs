using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Spectre.Console;
using System;
using System.IO;
using System.Threading;

namespace ZametkaMan
{
    public class CloudSyncManager
    {
        static string[] Scopes = { DriveService.Scope.DriveFile };
        static string ApplicationName = "ZametkaApp";

        private DriveService driveService;

        public CloudSyncManager()
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        
        public void SyncFile(string localFilePath)
        {
            try
            {
                string fileName = Path.GetFileName(localFilePath);
                
                var listRequest = driveService.Files.List();
                listRequest.Q = $"name = '{fileName}' and trashed = false";
                listRequest.Fields = "files(id, name)";
                var fileList = listRequest.Execute().Files;

                if (fileList != null && fileList.Count > 0)
                {
                    string fileId = fileList[0].Id;
                    using (var stream = new FileStream(localFilePath, FileMode.Open))
                    {
                        var updateRequest = driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId, stream, "application/octet-stream");
                        updateRequest.Fields = "id";
                        updateRequest.Upload();
                    }
                    AnsiConsole.MarkupLine($"[green]Файл '{fileName}' успешно обновлён в Google Drive.[/]");
                }
                else
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File() { Name = fileName };
                    using (var stream = new FileStream(localFilePath, FileMode.Open))
                    {
                        var createRequest = driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
                        createRequest.Fields = "id";
                        createRequest.Upload();
                    }
                    AnsiConsole.MarkupLine($"[green]Файл '{fileName}' успешно загружен в Google Drive.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка синхронизации с Google Drive: {ex.Message}[/]");
            }
        }
    }
}
