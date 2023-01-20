using DocumentSaver.Data.Entities;
using DocumentSaver.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentSaver.Data
{
    public class AppDbContext : DbContext
    {

        //public AppDbContext(DbContextOptions<AppDbContext> options)
        //    : base(options)
        //{

        //}

        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            //options.UseSqlite(Configuration.GetConnectionString("DbConnection"));

            options.UseSqlServer(Configuration.GetConnectionString("DbConnection"), options => options.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                        ));

        }

        public DbSet<DocumentInfo> DocumentInfo { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();

            // alternately this is built-in to EF Core 2.2
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
