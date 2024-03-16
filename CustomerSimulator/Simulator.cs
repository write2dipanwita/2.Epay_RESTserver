
using System.Net.Http.Json;

namespace CustomerSimulator
{
	public class Simulator
	{
		private static readonly string[] FirstNames = { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
		private static readonly string[] LastNames = { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };

		private static readonly Random random = new Random();

		public async Task SimulateRequests(string serverUrl, int numberOfRequests)
		{
			List<Task> tasks = new List<Task>();

			for (int i = 0; i < numberOfRequests; i++)
			{
				tasks.Add(SendRequest(serverUrl));
				await Task.Delay(TimeSpan.FromMilliseconds(100)); // Introducing delay between requests
			}

			await Task.WhenAll(tasks);
		}

		private async Task SendRequest(string serverUrl)
		{
			var customers = GenerateRandomCustomers();
			using var client = new HttpClient();

			var response = await client.PostAsJsonAsync($"{serverUrl}/api/customers", customers);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Request successful: {result}");
			}
			else
			{
				Console.WriteLine($"Request failed: {response.StatusCode}");
			}
		}

		private List<Customer> GenerateRandomCustomers()
		{
			var newCustomers = new List<Customer>();
			for (int i = 0; i < random.Next(2, 5); i++)
			{
				var customer = new Customer
				{
					firstName = FirstNames[random.Next(FirstNames.Length)],
					lastName = LastNames[random.Next(LastNames.Length)],
					age = random.Next(18, 91)
				};
				newCustomers.Add(customer);
			}
			return newCustomers;
		}
	}
}

