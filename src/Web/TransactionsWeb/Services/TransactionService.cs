using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TransactionsWeb.Models.TransactionViewModels;

namespace TransactionsWeb.Services
{
    public class TransactionService : ITransactionService
    {
        static HttpClient client = new HttpClient();
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public TransactionService(IOptionsSnapshot<AppSettings> settings)
        {
            _settings = settings;
            _remoteServiceBaseUrl = $"{_settings.Value.TransactionsApiUrl}/api/transaction/";
        }

        public async Task<int> CreateTransactionAsync(Transaction order, string token)
        {
            var uri = $"{_remoteServiceBaseUrl}items";
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content= new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);
            var jsonString =  response.Content.ReadAsStringAsync();
            jsonString.Wait();
            dynamic data = JObject.Parse(jsonString.Result);
            string value = data.Id;
            return Convert.ToInt32(value);
        }

        public async Task<TransactionObjectService> GetTransactionsByFilter(int page, int take, bool? fraud, string destino, DateTime? TransactionDate, string token)
        {
            var filterQs = "";

            if (TransactionDate.HasValue || fraud.HasValue)
            {
                var fraudQs = (fraud.HasValue) ? fraud.Value.ToString() : "null";
                var transactionDateQS = (TransactionDate.HasValue) && TransactionDate > new DateTime(1900,01,01)  ? TransactionDate.Value.ToString("yyyy-MM-dd"): "null";
                var destinationQS = string.IsNullOrEmpty(destino) ? "M19" : destino;
                filterQs = $"/isFraud/{fraudQs}/nameDest/{destinationQS}/transactionDate/{transactionDateQS}";
            }

            
            var uri =  $"{_remoteServiceBaseUrl}items{filterQs}?pageIndex={page}&pageSize={take}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(requestMessage);
            var dataString = await response.Content.ReadAsStringAsync();
            //var dataString = await client.GetStringAsync(uri); // await client.SendAsync(requestMessage).Result..ReadAsStringAsync();
            var repositories = JsonConvert.DeserializeObject<TransactionObjectService>(dataString);

            return repositories;
        }

        public Task<IEnumerable<SelectListItem>> GetTypes()
        {
            throw new NotImplementedException();
        }
    }
}
