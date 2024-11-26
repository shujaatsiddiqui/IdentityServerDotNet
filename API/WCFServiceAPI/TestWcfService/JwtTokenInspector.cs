using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.ServiceModel.Channels;
using Microsoft.IdentityModel.Protocols;

namespace TestWcfService
{
    public class JwtTokenInspector : IDispatchMessageInspector
    {
        private readonly TokenValidationParameters validationParameters;

        public string authority { get; private set; } = "https://localhost:44313";
        public string audience { get; private set; } = "vcmsAPI";

        public JwtTokenInspector()
        {
            validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // Fetch signing keys from IdentityServer
                    var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                        $"{authority}/.well-known/openid-configuration",
                        new OpenIdConnectConfigurationRetriever());
                    var config = configurationManager.GetConfigurationAsync().Result;
                    return config.SigningKeys;
                }
            };
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            string token = ExtractToken(request);
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Missing or invalid token.");

            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Add claims principal to the OperationContext
            OperationContext.Current.IncomingMessageProperties["Principal"] = validatedToken;
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState) { }

        private string ExtractToken(Message request)
        {
            // Access HTTP headers from the message
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out var httpRequestMessagePropertyObj))
            {
                var httpRequestMessageProperty = httpRequestMessagePropertyObj as HttpRequestMessageProperty;

                // Check if the "Authorization" header exists
                if (httpRequestMessageProperty?.Headers["Authorization"] != null)
                {
                    var authHeader = httpRequestMessageProperty.Headers["Authorization"];

                    // Check for Bearer token format
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return authHeader.Substring("Bearer ".Length).Trim();
                    }
                }
            }

            // Return null if no token is found
            return null;
        }

    }
}