using ShazamCore.Data;
using ShazamCore.Models;

namespace ShazamCore.Services
{    
    public class SqlServerService
    {
        private readonly string _connectionString;

        public SqlServerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<SongInfo> GetAllSongInfoList()
        {
            using var context = new SqlServerContext(_connectionString);
            return context.SongInfo.ToList();
        }

        public bool AddSongInfo(SongInfo songInfo, out string error)
        {
            error = string.Empty;
            using var context = new SqlServerContext(_connectionString);
            if (context.SongInfo.Any(x => x.SongUrl == songInfo.SongUrl))
            {
                error = $"Song url '{songInfo.SongUrl}' already exists in local SQL Server DB";
                return false;
            }

            context.SongInfo.Add(songInfo);
            context.SaveChanges();
            return true;
        }

        public bool DeleteSongInfo(string songUrl)
        {
            using var context = new SqlServerContext(_connectionString);
            var songInfo = context.SongInfo.FirstOrDefault(x => x.SongUrl == songUrl);
            if (songInfo != null)
            {
                context.Entry(songInfo).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
