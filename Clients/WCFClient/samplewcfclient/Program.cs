using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace samplewcfclient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceReference1.Service1Client client= new ServiceReference1.Service1Client();
            var httpRequestMessageProperty = new HttpRequestMessageProperty();

            string jwtToken = GetTokenAsync().Result;

            // Add the token to the HTTP headers
            using (new OperationContextScope(client.InnerChannel))
            {
                // Add Authorization header with Bearer token
                var httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers["Authorization"] = "Bearer " + jwtToken;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                // Call the service method
                var result = client.GetData(123); // Replace with your service method
                Console.WriteLine("Service response: " + result);
            }

            Console.ReadLine();
            // Close the client
            client.Close();

            //// httpRequestMessageProperty.Headers["X-Api-Key"] = "abcd";
            // using (new OperationContextScope(sc.InnerChannel))
            // {
            //     // Get the HTTP request property
            //     var httpRequest = new HttpRequestMessageProperty();
            //     httpRequest.Headers["X-Api-Key"] = "abcd";

            //     // Add the HTTP request property to the request context
            //     OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequest;

            //     // Now you can call the service method
            //     var result = sc.GetData(123);
            //     Console.WriteLine(result);
            //     Console.ReadLine();
            // }
        }

        public static async Task<string> GetTokenAsync()
        {
            try
            {
                // Discovery document URL (replace with your IdentityServer URL)
                string identityServerUrl = "https://localhost:5443";

                // Client credentials
                string clientId = "ELServiceClient";
                string clientSecret = "ELServiceClientSecret";
                string scope = "el.manage";

                // Get the discovery document
                var client = new HttpClient();
                var discovery = await client.GetDiscoveryDocumentAsync(identityServerUrl);

                if (discovery.IsError)
                {
                    Console.WriteLine($"Error retrieving discovery document: {discovery.Error}");
                    throw new Exception(discovery.Error);
                }

                // Request the token
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discovery.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                });

                if (tokenResponse.IsError)
                {
                    Console.WriteLine($"Error requesting token: {tokenResponse.Error}");
                    throw new Exception(tokenResponse.Error);
                }

                // Token received
                Console.WriteLine("Token retrieved successfully:");
                Console.WriteLine(tokenResponse.AccessToken);

                // Use the token in your WCF call
                string jwtToken = tokenResponse.AccessToken;
                return jwtToken;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
