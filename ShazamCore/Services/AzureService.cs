using ClientServerShared;
using ShazamCore.AzureADClientSecret;
using ShazamCore.Models;

namespace ShazamCore.Services
{
    // Note: after going with multiple VMs in WpfShazam, IAzureService is now used in both WPF and WinUI3
    public interface IAzureService
    {
        Task CreateWebApiClientsAsync();
        string? WebApiUrlNoAuth { get; }
        string? WebApiUrlAuth { get; }
        Task UseNewAccessTokenAsync();
        Task<List<SongInfo>> GetAllSongInfoListAsync(bool viaAuth);
        Task<string> AddSongInfoAsync(SongInfo songInfo, bool viaAuth);
        Task<string> DeleteSongInfoAsync(int songInfoId, bool viaAuth);
    }

    public class AzureService : IAzureService
    {
        private WebApiClient? _webApiClientNoAuth;
        private WebApiClient? _webApiClientAuth;

        public AzureService()
        {
            // This ctor is used when CreateAsync() can't be used, so need to set up _webApiClient[No]Auth
            // with CreateWebApiClientsAsync()
        }

        private AzureService(AzureADInfo? azureADInfo)
        {
            CreateWebApiClients(azureADInfo);
        }

        public async Task CreateWebApiClientsAsync()
        {
            AzureADInfo azureADInfo = await AuthConfig.GetAzureADInfoAsync();
            CreateWebApiClients(azureADInfo);
        }

        private void CreateWebApiClients(AzureADInfo? azureADInfo)
        {
            if (azureADInfo != null)
            {
#if DEBUG                
                //azureADInfo.WebApiEndpoint = "https://localhost:7026/songrepo";
                //System.Diagnostics.Debug.WriteLine($"****Overwrite WebApiEndpoint in Debug build: {azureADInfo.WebApiEndpoint}");
#endif

                // Make no-auth as https://localhost:7024/songreponoauth
                _webApiClientNoAuth = new WebApiClient(azureADInfo.WebApiEndpoint + "noauth");
                _webApiClientAuth = new WebApiClient(azureADInfo.WebApiEndpoint, azureADInfo.AccessToken);
            }
        }

        public static async Task<AzureService> CreateAsync()
        {
            AzureADInfo? azureADInfo = await GetAzureADInfoAsync();
            return new AzureService(azureADInfo);
        }

        private static async Task<AzureADInfo?> GetAzureADInfoAsync()
        {
            AzureADInfo? azureADInfo = null;
            try
            {
                azureADInfo = await AuthConfig.GetAzureADInfoAsync();
            }
            catch (Microsoft.Identity.Client.MsalClientException)
            {
                // AuthConfig.GetAzureADInfoUserNamePasswordAsync() call would cause this error:
                // { "Unsupported User Type 'Unknown'. Please see https://aka.ms/msal-net-up. "}                
                // https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/desktop-mobile/username-password-authentication
                // Guess: Only available for work and school accounts and not personal Microsoft accounts.
                // So GetAzureADInfoUserNamePasswordAsync() should work in theory, but not in this app
            }
            catch (Exception)
            {                
            }
            return azureADInfo;
        }

        public string? WebApiUrlNoAuth => _webApiClientNoAuth?.AzureServiceWebApiEndpoint;
        public string? WebApiUrlAuth => _webApiClientAuth?.AzureServiceWebApiEndpoint;

        public async Task UseNewAccessTokenAsync()
        {
            AzureADInfo azureADInfo = await AuthConfig.GetAzureADInfoAsync();
            _webApiClientAuth?.ReplaceAccessToken(azureADInfo.AccessToken);
        }
        
        public async Task<List<SongInfo>> GetAllSongInfoListAsync(bool viaAuth)
        {
            WebApiClient webApiClient = GetWebApiClient(viaAuth)!;
            GetAllSongInfoListResponse? response = await webApiClient.GetAllSongInfoListAsync(
                                                                        new GetAllSongInfoListRequest());

            return response?.SongInfoDtoList.Select(x => new SongInfo
            {
                Id = x.SongInfoId,
                Artist = x.Artist,
                Description = x.Description,
                CoverUrl = x.CoverUrl,
                Lyrics = x.Lyrics,
                SongUrl = x.SongUrl,                
            }).ToList() ?? new List<SongInfo>();
        }

        public async Task<string> AddSongInfoAsync(SongInfo songInfo, bool viaAuth)
        {
            WebApiClient webApiClient = GetWebApiClient(viaAuth)!;
            AddSongInfoResponse? response =
                await webApiClient.AddSongInfoAsync(new AddSongInfoRequest
                {
                    SongInfoDTO = new SongInfoDTO
                    {
                        Artist = songInfo.Artist,
                        Description = songInfo.Description,
                        CoverUrl = songInfo.CoverUrl,
                        Lyrics = songInfo.Lyrics,
                        SongUrl = songInfo.SongUrl
                    }
                });

            return response?.Error ?? "Error: didn't get a response from Web API";
        }

        public async Task<string> DeleteSongInfoAsync(int songInfoId, bool viaAuth)
        {
            WebApiClient webApiClient = GetWebApiClient(viaAuth)!;
            DeleteSongInfoResponse? response =
                await webApiClient.DeleteSongInfoAsync(new DeleteSongInfoRequest { SongInfoId = songInfoId });

            return response?.Error ?? "Error: didn't get a response from Web API";
        }

        private WebApiClient? GetWebApiClient(bool viaAuth)
        {
            WebApiClient? webApiClient = viaAuth ? _webApiClientAuth : _webApiClientNoAuth;
            if (webApiClient == null)
            {
                throw new Exception("Azure web client is not properly set up");
            }

            return webApiClient;
        }
    }
}
