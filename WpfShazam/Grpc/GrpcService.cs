using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.VisualBasic;
using ShazamCore.Models;

namespace WpfShazam.Grpc
{
    // Note: in songrepo.proto, names of package songrepo and service SongRepoGrpc match gRPC server side
    public class GrpcService
    {
        // Note: gRPC logic is basically disabled as of now due to Azure App Service, so comment out SongRepoGrpc.SongRepoGrpcClient
        //          because some PC may not have the right VS 2022 version (17.8.0 or higher) to generate C# code from songrepo.proto.
        //private SongRepoGrpc.SongRepoGrpcClient _songRepoGrpcClient;

        public GrpcService(string address)
        {
            //var channel = GrpcChannel.ForAddress(address);
            //_songRepoGrpcClient = new SongRepoGrpc.SongRepoGrpcClient(channel);
        }

        public async Task<List<SongInfo>> GetAllSongInfoListAsync()
        {
            await Task.CompletedTask;
            return new List<SongInfo>();

            //var response = await _songRepoGrpcClient.GetAllSongInfoListAsync(new GetAllSongInfoListRequest());
            //return response?.SongInfoDtoList.Select(x => new SongInfo
            //{
            //    Id = x.SongInfoId,
            //    Artist = x.Artist,
            //    Description = x.Description,
            //    CoverUrl = x.CoverUrl,
            //    Lyrics = x.Lyrics,
            //    SongUrl = x.SongUrl,
            //}).ToList() ?? new List<SongInfo>();
        }

        public async Task<string> AddSongInfoAsync(SongInfo songInfo)
        {
            await Task.CompletedTask;
            return "Error: gRPC logic commented out";

            //AddSongInfoResponse? response =
            //    await _songRepoGrpcClient.AddSongInfoAsync(new AddSongInfoRequest
            //    {
            //        SongInfoDTO = new SongInfoDTO
            //        {
            //            Artist = songInfo.Artist,
            //            Description = songInfo.Description,
            //            CoverUrl = songInfo.CoverUrl,
            //            Lyrics = songInfo.Lyrics,
            //            SongUrl = songInfo.SongUrl
            //        }
            //    });

            //return response?.Error ?? "Error: didn't get a response from gRPC service";
        }

        public async Task<string> DeleteSongInfoAsync(int songInfoId)
        {
            await Task.CompletedTask;
            return "Error: gRPC logic commented out";

            //DeleteSongInfoResponse? response =
            //    await _songRepoGrpcClient.DeleteSongInfoAsync(new DeleteSongInfoRequest { SongInfoId = songInfoId });

            //return response?.Error ?? "Error: didn't get a response from gRPC service";
        }
    }
}
