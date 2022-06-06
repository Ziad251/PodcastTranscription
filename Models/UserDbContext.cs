using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using transcription_project.WebApp.Services;
using transcription_project.WebApp.Extensions;
using Microsoft.Extensions.Configuration;

namespace transcription_project.WebApp.Models
{
   

    public class UserDbContext : DbContext
    {

        public DbSet<UserData> UserDatas { get; set; }
        public DbSet<Container> Containers { get; set; }

        public DbSet<YouTubeNames> YouTubeTitles { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserData>()
            .HasMany(u => u.Containers)
            .WithOne(u => u.user);

            modelBuilder.Entity<UserData>()
            .HasMany(u => u.YouTubeNames)
            .WithOne(u => u.user);

            modelBuilder.Entity<YouTubeNames>()
            .HasKey(y => y.id);

            modelBuilder.Entity<Container>()
            .HasMany(c => c.Participants)
            .WithOne(c => c.container);

            modelBuilder.Entity<Container>()
            .HasKey(c => c.containerUid);

            modelBuilder.Entity<Container>()
            .HasIndex(c => c.containerName)
            .IsUnique();

            modelBuilder.Entity<Participant>()
            .HasKey(p => p.participantId);


        }

        
    }
}
