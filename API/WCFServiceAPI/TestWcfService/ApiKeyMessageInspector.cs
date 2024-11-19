using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;

public class ApiKeyMessageInspector : IDispatchMessageInspector
{
    public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
    {
        // Extract the HTTP request
        var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        var apiKey = httpRequest.Headers["X-Api-Key"];

        // Validate the API key
        if (string.IsNullOrEmpty(apiKey) || !IsValidApiKey(apiKey))
        {
            throw new FaultException("Invalid or missing API key.");
        }

        return null;
    }

    public void BeforeSendReply(ref Message reply, object correlationState)
    {
        // No action required before sending the reply
    }

    private bool IsValidApiKey(string apiKey)
    {
        // Implement your API key validation logic here
        return apiKey == "abcd";
    }
}
