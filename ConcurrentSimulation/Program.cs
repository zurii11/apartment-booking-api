using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        const int totalRequests = 10;

        var client = new HttpClient();

        string baseUrl = "http://localhost:5000";
        string endpoint = "/api/Bookings?apartment_id=1&start_date=2025-11-05&end_date=2025-11-10";

        var tasks = new List<Task>();

        for (int i = 0; i < totalRequests; i++)
        {
            tasks.Add(SendBookingRequest(client, baseUrl + endpoint, i + 1));
        }

        await Task.WhenAll(tasks);
    }

    static async Task SendBookingRequest(HttpClient client, string url, int attemptNumber)
    {
        try
        {
            var response = await client.PostAsJsonAsync(url, new { });
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[Attempt {attemptNumber}] Status: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"[Attempt {attemptNumber}] Response: {result}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Attempt {attemptNumber}] ERROR: {ex.Message}");
        }
    }
}

