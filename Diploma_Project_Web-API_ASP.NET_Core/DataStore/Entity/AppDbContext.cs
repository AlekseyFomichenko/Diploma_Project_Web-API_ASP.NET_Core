using Microsoft.EntityFrameworkCore;

namespace Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity
{
    public class AppDbContext : DbContext
    {
        private readonly string _connectingString;
        public AppDbContext() 
        {
            
        }
        public AppDbContext(string connectionString)
        {
            _connectingString = connectionString;
        }

        public DbSet<UserEntity> Users {  get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectingString).UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserEntity>( entity =>
            {
                entity.HasKey(x => x.Id)
                .HasName("UserEntity");
                entity.HasIndex(x => x.Login).IsUnique();

                entity.Property(e => e.Password)
                .HasColumnName("Password")
                .HasMaxLength(255)
                .IsRequired();

                entity.HasOne(e => e.Role)
                .WithMany(c => c.Users);
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.RoleType);
                entity.HasIndex(x => x.Name).IsUnique();
            });
        }
    }
}
