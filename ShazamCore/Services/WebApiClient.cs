﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClientServerShared;

namespace ShazamCore.Services
{
    public class WebApiClient
    {
        private HttpClient _httpClient;

        // WebApiClient class supports *one* endpoint (e.g. auth or no-auth)
        public WebApiClient(string azureServiceWebApiEndpoint, string? accessToken = null)
        {
            _httpClient = new HttpClient();
            AzureServiceWebApiEndpoint = azureServiceWebApiEndpoint;

            var defaultRequestHeaders = _httpClient.DefaultRequestHeaders;
            if (defaultRequestHeaders.Accept == null ||
               !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new
                            MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            }
        }

        public string AzureServiceWebApiEndpoint { get; private set; }

        public void ReplaceAccessToken(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
        }

        public async Task<GetAllSongInfoListResponse?> GetAllSongInfoListAsync(GetAllSongInfoListRequest request)
        {
            return await CallWebAPIAsync<GetAllSongInfoListRequest, GetAllSongInfoListResponse>(request, "GetAllSongInfoList");
        }

        public async Task<AddSongInfoResponse?> AddSongInfoAsync(AddSongInfoRequest request)
        {
            return await CallWebAPIAsync<AddSongInfoRequest, AddSongInfoResponse>(request, "AddSongInfo");
        }
        
        public async Task<DeleteSongInfoResponse?> DeleteSongInfoAsync(DeleteSongInfoRequest request)
        {
            // 2023-11-22: complete path doesn't work with DeleteAsync() in Azure,
            //              so just pass Qy7z_oiN6nQ (video ID) by removing YouTubeWatchPrefix.
            //              But PostAsJsonAsync() version below still works with the server, though.
            string url = $"{AzureServiceWebApiEndpoint}/{request.SongUrl.Replace(Constants.YouTubeWatchPrefix, string.Empty)}";
            HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(url);
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadFromJsonAsync<DeleteSongInfoResponse>();

            // Note: as of 2023-11-08, DeleteSongInfo with HttpDelete on the server side
            //          doesn't work in Azure (working on my machine!).
            //          So use PostAsJsonAsync instead of DeleteAsync.
            //return await CallWebAPIAsync<DeleteSongInfoRequest, DeleteSongInfoResponse>(request, "DeleteSongInfo");
        }

        private async Task<RSP?> CallWebAPIAsync<REQ, RSP>(REQ request, string methodName)
        {
            string url = $"{AzureServiceWebApiEndpoint}/{methodName}";

            HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(url, request);
            responseMessage.EnsureSuccessStatusCode();
            RSP? response = await responseMessage.Content.ReadFromJsonAsync<RSP>();
            return response;
        }
    }
}
