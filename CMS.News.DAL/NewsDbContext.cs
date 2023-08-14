using CMS.News.DAL.EntityTypeConfigurations;

namespace CMS.News.DAL
{
    public class NewsDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public NewsDbContext(DbContextOptions<NewsDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        #region DbSet
        public virtual DbSet<TokenLogin> TokenLogins { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Right> Rights { get; set; }
        public virtual DbSet<RoleRight> RoleRights { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("NewsDB");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TokenLoginEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SiteEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RightEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleRightEntityTypeConfiguration());
        }
    }
}
