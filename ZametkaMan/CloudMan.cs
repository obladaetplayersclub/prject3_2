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
    /// <summary>
    /// Класс, отвечающий за взаимодействие с Google Drive
    /// Содержит логику авторизации и методы для синхронизации файлов
    /// </summary>
    public class CloudManager
    {   
        // Области доступа (Scopes) для работы с файлами в Google Drive
        private static readonly string[] Scopes = { DriveService.Scope.DriveFile };
        
        // Название приложения, под которым будет выполняться авторизация
        private static readonly string ApplicationName = "ZametkaApp";
        private readonly DriveService driveService;
    
        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// Выполняет авторизацию через Google OAuth2, используя credentials.json и сохраняя токен в token.json
        /// </summary>
        public CloudManager()
        {
            try
            {
                // Объект, содержащий учётные данные пользователя
                UserCredential credential;
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {   
                    // Авторизация через Google OAuth2; при первом запуске откроется браузер
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
                
                // Создаём экземпляр DriveService, используя полученные учётные данные
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка авторизации Google API: {ex.Message}[/]");
                throw;
            }
        }
        
        /// <summary>
        /// Синхронизирует указанный локальный файл с Google Drive
        /// </summary>
        public void SyncFile(string localFilePath)
        {
            try
            {
                string fileName = Path.GetFileName(localFilePath);
                var listRequest = driveService.Files.List();
                listRequest.Q = $"name = '{fileName}' and trashed = false";
                listRequest.Fields = "files(id, name)";
                var fileList = listRequest.Execute().Files;

                using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                {   
                    // Если нашли хотя бы один файл с таким именем — обновляем
                    if (fileList != null && fileList.Count > 0)
                    {
                        string fileId = fileList[0].Id;
                        var updateRequest = driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId, stream, "application/octet-stream");
                        updateRequest.Fields = "id";
                        updateRequest.Upload();
                        AnsiConsole.MarkupLine($"[green]Файл '{fileName}' успешно обновлён в Google Drive.[/]");
                    }
                    
                    // Создаем новый
                    else
                    {
                        var fileMetadata = new Google.Apis.Drive.v3.Data.File() { Name = fileName };
                        var createRequest = driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
                        createRequest.Fields = "id";
                        createRequest.Upload();
                        AnsiConsole.MarkupLine($"[green]Файл '{fileName}' успешно загружен в Google Drive.[/]");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка синхронизации с Google Drive: {ex.Message}[/]");
            }
        }
    }
}
