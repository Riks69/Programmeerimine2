using KooliProjekt.PublicAPI.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
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
                result.AddError("_", ex.Message);
            }

            return result;

        }


        public async Task<Result<Customer>> Get(int id)
        {
            var result = new Result<Customer>();

            try
            {
                result.Value = await _httpClient.GetFromJsonAsync<Customer>("Customers/" + id);
            }
            catch (Exception ex)
            {
                result.AddError("_", ex.Message);
            }

            return result;
        }

        public async Task<Result> Save(Customer list)
        {
            var result = new Result();

            try
            {
                if (list.Id == 0)
                {
                    await _httpClient.PostAsJsonAsync("Customers/", list);
                }
                else
                {
                    await _httpClient.PutAsJsonAsync("Customers/" + list.Id, list);
                }
            }
            catch (Exception ex)
            {
                result.AddError("_", ex.Message);
            }
            return result;
        }

        public async Task<Result> Delete(int id)
        {
            var result = new Result();

            try
            {
                await _httpClient.DeleteAsync("Customers/" + id);
            }
            catch (Exception ex)
            {
                result.AddError("_", ex.Message);
            }

            return result;
        }
    }
}