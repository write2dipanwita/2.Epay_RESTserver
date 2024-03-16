using System;
using System.Threading.Tasks;
using CustomerSimulator; // Import the namespace containing the simulator logic

namespace SimulatorRunner
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string serverUrl = "http://localhost:5020"; // Change to the actual URL of your API server

			var simulator = new Simulator();
			await simulator.SimulateRequests(serverUrl, 5); // Simulate 5 requests
		}
	}
}
