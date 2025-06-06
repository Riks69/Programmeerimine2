using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace KooliProjekt.PublicAPI.Api
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
                result.Error = "Ei saa serveriga ühendust. Palun proovi hiljem uuesti.";
            }

            return result;
        }

        public async Task<Result<Customer>> Get(int id)
        {
            var result = new Result<Customer>();

            try
            {
                var response = await _httpClient.GetAsync($"Customers/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Kliendi laadimine ebaõnnestus: {response.ReasonPhrase}";
                    return result;
                }

                result.Value = await response.Content.ReadFromJsonAsync<Customer>();
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task<Result> Save(Customer customer)
        {
            var result = new Result();

            try
            {
                HttpResponseMessage response;

                if (customer.Id == 0)
                {
                    response = await _httpClient.PostAsJsonAsync("Customers", customer);
                }
                else
                {
                    response = await _httpClient.PutAsJsonAsync($"Customers/{customer.Id}", customer);
                }

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Salvestamine ebaõnnestus: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        public async Task Delete(int id)
        {
            await _httpClient.DeleteAsync($"Customers/{id}");
        }
    }
}
