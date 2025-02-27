using Football_Predictions.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Football_Predictions.Services
{
    public class FootballDataService
    {
        private readonly HttpClient _httpClient;
        private readonly FootballDataApiSettings _apiSettings;

        public FootballDataService(HttpClient httpClient, IOptions<FootballDataApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;

            // Set API key in request headers (ensure API key is configured in appsettings.json)
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _apiSettings.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Fetch last matches for a given team
        public async Task<List<Match>> GetLastMatchesAsync(int teamId, int count = 5)
        {
            var url = $"{_apiSettings.BaseUrl}teams/{teamId}/matches?status=FINISHED&limit={count}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var matches = JsonSerializer.Deserialize<MatchesResponse>(content);
                return matches?.Matches ?? new List<Match>();
            }

            throw new Exception($"Failed to fetch matches: {response.StatusCode}");
        }

        // Fetch current league standings
        public async Task<StandingsResponse> GetLeagueStandingsAsync()
        {
            var url = $"{_apiSettings.BaseUrl}competitions/PL/standings";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var standings = JsonSerializer.Deserialize<StandingsResponse>(content);
                return standings;
            }

            throw new Exception($"Failed to fetch league standings: {response.StatusCode}");
        }
    }

    // Classes to deserialize API responses
    public class MatchesResponse
    {
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Status { get; set; }
        public DateTime UtcDate { get; set; }
        public Score Score { get; set; }
    }

    public class Score
    {
        public string Winner { get; set; }
        public int? FullTimeHome { get; set; }
        public int? FullTimeAway { get; set; }
    }
}
