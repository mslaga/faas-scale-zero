using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace faasSzero.OpenFaaSApi
{
    public class Api
    {
        private HttpClient httpClient;

        private async Task CheckResponse(HttpResponseMessage response)
        {
            if ((int)response.StatusCode / 100 != 2) {
                string content = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"Api Error {response.StatusCode}\n{content}");
            }
        }

        public async Task ScaleAsync(string functionName, int replicas)
        {
            Model.ScaleRequest scale = new Model.ScaleRequest(functionName, replicas);

            StringContent stringContent = new StringContent(JsonSerializer.Serialize(scale), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync($"system/scale-function/{functionName}", stringContent);
            await CheckResponse(response);
        }

        public async Task<List<Model.Function>> GetFunctionsAsync()
        {
            var response = await httpClient.GetAsync("system/functions");
            await CheckResponse(response);
            string stringResult = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Model.Function>>(stringResult);
        }

        public async Task<Model.SystemInfo> GetSystemAsync()
        {
            var response = await httpClient.GetAsync("system/info");
            await CheckResponse(response);
            string stringResult = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Model.SystemInfo>(stringResult);
        }

        public Api(string gateway, string user, string password)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(gateway);
            string basicToken = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(user + ":" + password));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + basicToken);
        }
    }
}
