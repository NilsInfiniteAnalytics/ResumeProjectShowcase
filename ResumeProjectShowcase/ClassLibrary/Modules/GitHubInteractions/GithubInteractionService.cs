using System.Text.Json;
using ClassLibrary.Modules.GitHubInteractions.Models;

namespace ClassLibrary.Modules.GitHubInteractions
{
    internal class GithubInteractionService
    {
        private readonly HttpClient _httpClient;
        public GithubInteractionService(HttpClient httpClient, string pat)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pat);
        }

        public async Task<ContributionCollection> RetrieveContributionData(string userName)
        {
            const string query = """
                                  query($userName:String!) {
                                                               user(login: $userName){
                                                                 contributionsCollection {
                                                                   contributionCalendar {
                                                                     totalContributions
                                                                     weeks {
                                                                       contributionDays {
                                                                         contributionCount
                                                                         date
                                                                       }
                                                                     }
                                                                   }
                                                                 }
                                                               }
                                                             }
                                                             
                                 """;
            var variables = $"{{\"userName\": \"{userName}\"}}";
            var body = JsonSerializer.Serialize(new { query, variables });
            var response = await _httpClient.PostAsync("https://api.github.com/graphql",
                new StringContent(body, System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var collection = JsonSerializer.Deserialize<ContributionCollection>(content);
                    if (collection != null) return collection;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            throw new Exception($"GitHub API request failed: {response.StatusCode}");
        }
    }
}