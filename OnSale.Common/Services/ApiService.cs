using Newtonsoft.Json;
using OnSale.Common.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnSale.Common.Services
{
    public class ApiService : IApiService
    {
        public async Task<Response> GetListAsync<T>(
            string urlBase,
            string servicePrefix,
            string controller)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase), //cre un client
                };

                string url = $"{servicePrefix}{controller}"; //arma la direcciòn
                HttpResponseMessage response = await client.GetAsync(url); //ejecuta el mètodo get
                string result = await response.Content.ReadAsStringAsync(); //lee la respuesta

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                //lee resuly y devuelve una lista de <T>
                List<T> list = JsonConvert.DeserializeObject<List<T>>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }

}
