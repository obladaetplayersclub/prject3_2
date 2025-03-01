using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;

namespace ZametkaMan
{
    public class ApiWork
    {   
        private static readonly HttpClient client = new HttpClient();

        public async Task<List<string>> GetWordAsync(string word)
        {
            try
            {
                string request = $"https://words.bighugelabs.com/api/2/4ba92a2307cf26d63ddee4d5585f2190/{word}/json";
                HttpResponseMessage response = await client.GetAsync(request);
                string json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<Dictionary<string, ThesaurusData>>(json, options);
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

    public class ThesaurusData
    {
        [JsonPropertyName("syn")]
        public List<string>? Syn { get; set; }
    }
}

