using System.IO;
using System.Net.Http;
using System.Net.Http.Json;

namespace KooliProjekt.WpfApp.Api
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
            var result = new Result<List<Customer>>();

            try
            {
                result.Value = await _httpClient.GetFromJsonAsync<List<Customer>>("Customers");
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task Save(Customer list)
        {

            try
            {
                if (list.Id == 0)
                {
                    await _httpClient.PostAsJsonAsync("Customers", list);
                }
                else
                {
                    await _httpClient.PutAsJsonAsync("Customers/" + list.Id, list);

                }

            }
            catch (Exception ex)
            {

            }

        }

        public async Task Delete(int id)
        {
            try
            {
                await _httpClient.DeleteAsync("Customers/" + id);

            }
            catch (Exception ex)
            {

            }
        }
    }
}