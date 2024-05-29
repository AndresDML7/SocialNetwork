using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

var client = new HttpClient { BaseAddress = new Uri("http://localhost:5001/api/") };

while (true)
{
    Console.WriteLine("\n");
    Console.Write("> ");
    var input = Console.ReadLine();
    Console.WriteLine("\n");
    var parts = input.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length < 2)
    {
        Console.WriteLine("Comando inválido");
        continue;
    }

    var command = parts[0].ToLower();
    var username = parts[1].TrimStart('@');

    switch (command)
    {
        case "post":
            if (parts.Length < 3)
            {
                Console.WriteLine("Mensaje no proporcionado");
                continue;
            }
            var content = parts[2];
            await PostMessage(username, content);
            break;

        case "follow":
            if (parts.Length < 3)
            {
                Console.WriteLine("Usuario a seguir no proporcionado");
                continue;
            }
            var followeeUsername = parts[2].TrimStart('@');
            await FollowUser(username, followeeUsername);
            break;

        case "dashboard":
            await GetDashboard(username);
            break;

        default:
            Console.WriteLine("Comando no reconocido");
            break;
    }
}

async Task PostMessage(string username, string content)
{
    var request = new { Content = content, Timestamp = DateTime.Now, Username = username };
    var json = JsonConvert.SerializeObject(request);
    var contentString = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await client.PostAsync($"user/post", contentString);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{username} publicó -> \"{content}\" @{request.Timestamp:HH:mm}");
    }
    else
    {
        Console.WriteLine("Error publicando mensaje");
    }
}

async Task FollowUser(string followerUsername, string followeeUsername)
{
    var response = await client.PostAsync($"user/{followerUsername}/follow/{followeeUsername}", null);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{followerUsername} empezó a seguir a {followeeUsername}");
    }
    else
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
    }
}

async Task GetDashboard(string username)
{
    var response = await client.GetAsync($"user/{username}/dashboard");

    if (response.IsSuccessStatusCode)
    {
        var messages = await response.Content.ReadFromJsonAsync<IEnumerable<MessageModel>>();
        if (messages != null)
        {
            foreach (var message in messages)
            {
                Console.WriteLine($"\"{message.Content}\" @{message.Username} @{message.Timestamp:HH:mm}");
            }
        }
    }
    else
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
    }
}
