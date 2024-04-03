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

    public static async Task<IEnumerable<User>> GetUsersDontFollowBack(string user)
    {
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
                var usersResponse = await _httpClient.GetAsync($"{user}/{endpoint}?{queryString}");
                var usersData = await usersResponse.Content.ReadAsStringAsync();
                usersPaged = JsonConvert.DeserializeObject<User[]>(usersData);

                if (usersPaged is not null)
                {
                    users.AddRange(usersPaged);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
