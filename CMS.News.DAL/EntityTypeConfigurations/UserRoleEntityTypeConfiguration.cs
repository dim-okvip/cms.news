namespace CMS.News.DAL.EntityTypeConfigurations
{
    public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole").HasKey(x => new { x.UserId, x.RoleId, x.SiteId });
        }
    }
}
