using github_api;

Console.Write("Insira o usuário do GitHub: ");

var user = Console.ReadLine();
var usersDontFollowBack = await GitHubService.GetUsersDontFollowBack(user!);

Console.WriteLine(usersDontFollowBack.Count());
Console.WriteLine(string.Join("\n", usersDontFollowBack.Select(u => u.Login)));
Console.ReadKey();
