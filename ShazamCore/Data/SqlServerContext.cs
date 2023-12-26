using Microsoft.EntityFrameworkCore;
using ShazamCore.Models;

namespace ShazamCore.Data
{
    public class SqlServerContext : DbContext
    {
#if false // Create your [WpfShazamDB] and [SongInfo] as needed
USE [WpfShazamDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SongInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Artist] [nvarchar](30) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CoverUrl] [nvarchar](max) NULL,
	[Lyrics] [nvarchar](max) NULL,
	[SongUrl] [nvarchar](450) NOT NULL,
	[ModifiedDateTime] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SongInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
#endif

        private readonly string _connectionString;

        // This ctor is needed for PM> Update-Database
        public SqlServerContext()
        {
            _connectionString = "Data Source=localhost\\SQLDev2019;Initial Catalog=WpfShazamDB;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=True";
        }

        public SqlServerContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Note: without this, table name would be SongInfos
            modelBuilder.Entity<SongInfo>().ToTable("SongInfo");
        }

        // SongInfo table
        public DbSet<SongInfo> SongInfo { get; set; }
    }
}
