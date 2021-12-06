using System;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if(disco.IsError){
                Console.WriteLine(disco.Error);
                return;
            }
            else{
                Console.WriteLine(disco.AuthorizeEndpoint);
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest{
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if(tokenResponse.IsError){
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            Console.WriteLine(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:5003/identity");
            if(!response.IsSuccessStatusCode){
                Console.Write(response.StatusCode);
            }
            else{
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
