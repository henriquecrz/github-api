using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace github_api;

public class GitHubService
{
    private const string FOLLOWERS = "followers";
    private const string FOLLOWING = "following";
    private const int MAX_PER_PAGE = 100;

    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.github.com/users/"),
        DefaultRequestHeaders =
        {
            UserAgent = { new ProductInfoHeaderValue("github-api", productVersion: null) },
            Accept = { new MediaTypeWithQualityHeaderValue("application/vnd.github+json") }
        }
    };

    public static async Task<bool> CheckIfUserExists(string user)
    {
        try
        {
            var response = await _httpClient.GetAsync(user);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");

            return false;
        }
    }

    public static async Task<IEnumerable<User>> GetUsersDontFollowBack(string user)
    {
        // WhenAll?
        var followers = await GetUsers(user, FOLLOWERS);
        var following = await GetUsers(user, FOLLOWING);

        return GetUsersDontFollowBack(followers, following);
    }

    private static async Task<User[]> GetUsers(string user, string endpoint)
    {
        var users = new List<User>();
        var usersPaged = Array.Empty<User>();
        var page = 1;

        do
        {
            var queryParams = new Dictionary<string, string>
            {
                { "per_page", MAX_PER_PAGE.ToString() },
                { "page", page.ToString() }
            };
            var query = queryParams.Select(param => $"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
            var queryString = string.Join('&', query);

            try
            {
                var response = await _httpClient.GetAsync($"{user}/{endpoint}?{queryString}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    usersPaged = JsonConvert.DeserializeObject<User[]>(content)!;

                    users.AddRange(usersPaged);
                }
                else
                {
                    var errorReponse = JsonConvert.DeserializeObject<NotFoundResponse>(content)!;

                    Console.WriteLine($"GitHub response: '{errorReponse.Message}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            page++;
        }
        while (usersPaged?.Length == MAX_PER_PAGE);

        return [.. users];
    }

    private static IEnumerable<User> GetUsersDontFollowBack(User[] followers, User[] following)
    {
        return following.Where(f => !followers.Any(u => u.Login == f.Login));
    }
}
