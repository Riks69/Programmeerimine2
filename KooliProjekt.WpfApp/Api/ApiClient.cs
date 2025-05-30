using KooliProjekt.WpfApp.ApiAdd;
using System.Net.Http;
using System.Net.Http.Json;

namespace  KooliProjekt.WpfApp.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7136/api/");
        }

        public async Task<Result<List<Customer>>> List()
        {
            try
            {
                var customers = await _httpClient.GetFromJsonAsync<List<Customer>>("Customers");
                return new Result<List<Customer>> { Value = customers };
            }
            catch (Exception ex)
            {
                return new Result<List<Customer>> { Error = ex.Message };
            }
        }

        public async Task<Result> Save(Customer customer)
        {
            try
            {
                HttpResponseMessage response;

                if (customer.Id == 0)
                    response = await _httpClient.PostAsJsonAsync("Customers", customer);
                else
                    response = await _httpClient.PutAsJsonAsync($"Customers/{customer.Id}", customer);

                if (!response.IsSuccessStatusCode)
                    return new Result { Error = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}" };

                return new Result();
            }
            catch (Exception ex)
            {
                return new Result { Error = ex.Message };
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Customers/{id}");

                if (!response.IsSuccessStatusCode)
                    return new Result { Error = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}" };

                return new Result();
            }
            catch (Exception ex)
            {
                return new Result { Error = ex.Message };
            }
        }
    }
}