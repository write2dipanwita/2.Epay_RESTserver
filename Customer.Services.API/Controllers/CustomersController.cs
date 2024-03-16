using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Customers.Services.API.Models;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Customers.Services.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : ControllerBase
	{
		private static List<Customer> customers = new List<Customer>();
		private static object lockObj = new object();
		private static int currentid = 1;
		private static readonly string dataFilePath = "customerList.json";

		public CustomersController() {

			LoadDataFromFile();
		}

		[HttpPost]
		public IActionResult PostCustomers(List<Customer> newCustomers)
		{
			lock (lockObj)
			{
				List<string> errors = new List<string>();
				foreach (var customer in newCustomers)
				{
					string validationError = ValidateCustomer(customer);

					if (validationError != null)
					{
						errors.Add(validationError);
						continue; 
					}

					customer.id = currentid++;
					InsertCustomer(customer);
				}
				SaveDataToFile();
			}

			return Ok();
		}

		[HttpGet]
		public IActionResult GetCustomers()
		{
			return Ok(customers);
		}

		private string ValidateCustomer(Customer customer)
		{
			if (string.IsNullOrWhiteSpace(customer.firstName))
			{
				return "First name is required.";
			}

			if (string.IsNullOrWhiteSpace(customer.lastName))
			{
				return "Last name is required.";
			}

			if (customer.age < 18)
			{
				return "Age must be above 18.";
			}

			if (customers != null && customers.Any(c => c.id == customer.id))
			{
				return $"ID {customer.id} has already been used.";
			}

			return null; // Return null if validation passes
		}

		private void InsertCustomer(Customer newCustomer)
		{
			if (customers == null)
			{
				customers = new List<Customer>();
			}

			int index = 0;

			// Find the correct index to insert the new customer based on last name and then first name
			while (index < customers.Count &&
				   string.Compare(customers[index].lastName, newCustomer.lastName, StringComparison.OrdinalIgnoreCase) < 0)
			{
				index++;
			}

			while (index < customers.Count &&
				   string.Compare(customers[index].lastName, newCustomer.lastName, StringComparison.OrdinalIgnoreCase) == 0 &&
				   string.Compare(customers[index].firstName, newCustomer.firstName, StringComparison.OrdinalIgnoreCase) < 0)
			{
				index++;
			}

			// Insert the new customer at the determined index
			customers.Insert(index, newCustomer);
		}

		private void LoadDataFromFile()
		{
			if (System.IO.File.Exists(dataFilePath))
			{
				var json = System.IO.File.ReadAllText(dataFilePath);
				customers = JsonConvert.DeserializeObject<List<Customer>>(json);
			}
		}
		private void SaveDataToFile()
		{
			var json = JsonConvert.SerializeObject(customers, Formatting.Indented);
			System.IO.File.WriteAllText(dataFilePath, json);
		}
	}

	
}

