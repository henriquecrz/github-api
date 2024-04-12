using System.Text.RegularExpressions;

namespace github_api;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length != default)
        {
            var input = args
                .First()
                .Replace(SpecialCharsRegex(), string.Empty);

            Console.WriteLine($"GitHub user: {input}");

            await GetUsersDontFollowBack(input);
        }

        string user;

        do
        {
            Console.Write("Insert GitHub user: ");

            var input = Console
                .ReadLine()!
                .Replace(SpecialCharsRegex(), string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            user = input.FirstOrDefault()!;

            if (string.IsNullOrWhiteSpace(user))
            {
                continue;
            }

            await GetUsersDontFollowBack(user);
        }
        while (!string.IsNullOrWhiteSpace(user));
    }

    private static async Task GetUsersDontFollowBack(string user)
    {
        if (await GitHubService.CheckIfUserExists(user))
        {
            var usersDontFollowBack = await GitHubService.GetUsersDontFollowBack(user);

            Console.WriteLine($"\nTotal: {usersDontFollowBack.Count()}\n");
            Console.WriteLine($"{string.Join("\n", usersDontFollowBack.Select(u => u.Login))}\n");
        }
        else
        {
            Console.WriteLine($"\nUser '{user}' not found");
        }
    }

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex SpecialCharsRegex();
}
