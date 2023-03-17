using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Configuration;

namespace smarthomeAPI.Data
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) : base(options)
        {

            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer(_configuration.GetSection("sqlpath").Value);
            
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RawData> RawDatas => Set<RawData>();
        public DbSet<EnvironmentType> Environments => Set<EnvironmentType>();
    }
}
