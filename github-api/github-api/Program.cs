using github_api;
using System.Text.RegularExpressions;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        string user;

        do
        {
            Console.Write("Insira o usuário do GitHub: ");

            var input = Console
                .ReadLine()!
                .Replace(SpecialCharsRegex(), string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            user = input.FirstOrDefault()!;

            if (string.IsNullOrWhiteSpace(user))
            {
                continue;
            }

            var usersDontFollowBack = await GitHubService.GetUsersDontFollowBack(user);

            Console.WriteLine($"\nTotal: {usersDontFollowBack.Count()}\n");
            Console.WriteLine(string.Join("\n", usersDontFollowBack.Select(u => u.Login)));
        }
        while (!string.IsNullOrWhiteSpace(user));
    }

    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex SpecialCharsRegex();
}