using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using transcription_project.WebApp.Models;

namespace transcription_project.WebApp.Extensions
{
    public class DbExtension
    {
        private IConfiguration _configuration;
       
         public DbExtension(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        public DbContextOptionsBuilder<UserDbContext> BuildOptions()
        {
            DbContextOptionsBuilder<UserDbContext> dbContextOptions = new DbContextOptionsBuilder<UserDbContext>();
            dbContextOptions.UseSqlServer((_configuration.GetValue<string>("SQLServer:ConnectionString")));
            SqlServerDbContextOptionsBuilder sqlServerOptionsAction = new SqlServerDbContextOptionsBuilder(dbContextOptions);
            sqlServerOptionsAction.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(60),
                    errorNumbersToAdd: null);
            return dbContextOptions;
        }
    }
}