using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;

namespace ZametkaMan
{   
    /// <summary>
    /// Класс ApiWork нужен для получения синонимов через API Big Huge Thesaurus
    /// Кол-во запросов - 500 запросов в денб
    /// Он отправляет HTTP-запросы и десериализует JSON-ответ в список синонимов.
    /// </summary>
    public class ApiWork
    {   
        private static readonly HttpClient client = new HttpClient();
        
        /// <summary>
        /// Асинхронно получает список синонимов для заданного слова.
        /// Метод отправляет запрос к API, десериализует полученный JSON и возвращает все найденные синонимы.
        /// </summary>
        public async Task<List<string>> NaitiSinon(string word)
        {
            try
            {   
                // Формирование запроса к API с использованием переданного слова
                // Именно такой запрос по документации сайта
                string request = $"https://words.bighugelabs.com/api/2/4ba92a2307cf26d63ddee4d5585f2190/{word}/json";
                HttpResponseMessage response = await client.GetAsync(request);
                string json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                // Десериализация JSON-ответа в словарь
                // Ключом будет часть речи, а значение – слово (синоним)
                var result = JsonSerializer.Deserialize<Dictionary<string, SinonData>>(json, options);
                var synonyms = new List<string>();
                if (result != null)
                {
                    foreach (var entry in result.Values)
                    {
                        if (entry.Syn != null)
                            synonyms.AddRange(entry.Syn);
                    }
                }
                return synonyms;
            
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка при получении синонимов: {e.Message}[/]");
                return new List<string>();
            }
        }
    }
    
    /// <summary>
    /// Класс SinonData представляет данные, возвращаемые API для синонимов.
    /// </summary>
    public class SinonData
    {
        [JsonPropertyName("syn")]
        public List<string>? Syn { get; set; }
    }
}

