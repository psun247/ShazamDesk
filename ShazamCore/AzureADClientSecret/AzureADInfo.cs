namespace ShazamCore.AzureADClientSecret
{
    public class AzureADInfo
    {
        // https://localhost:7026/songrepo in AzureADClientSecret\appsettings.json (copied to runtime folder)
        public string WebApiEndpoint { get; set; } = string.Empty;
        // JWT bearer token
        public string AccessToken { get; set; } = string.Empty;
    }
}
