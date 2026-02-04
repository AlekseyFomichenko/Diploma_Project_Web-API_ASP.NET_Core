using Microsoft.EntityFrameworkCore;

namespace UserService.Db
{
    public partial class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");
                entity.HasIndex(e => e.Email).IsUnique();

                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.Salt).HasColumnName("salt");

                entity.Property(e => e.RoleId).HasConversion<int>();
            });
            modelBuilder.Entity<Role>()
                .Property(e => e.RoleId)
                .HasConversion<int>();

            modelBuilder.Entity<Role>().HasData(Enum
                .GetValues(typeof(RoleId))
                .Cast<RoleId>()
                .Select(e => new Role()
                {
                    RoleId = e,
                    Name = e.ToString()
                }));
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}