namespace CourtAuth.IdentityServer.Models
{
    public class AzureAdOptions
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackPath { get; set; }
    }

}
