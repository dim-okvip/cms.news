namespace CMS.News.DAL.EntityTypeConfigurations
{
    public class RoleRightEntityTypeConfiguration : IEntityTypeConfiguration<RoleRight>
    {
        public void Configure(EntityTypeBuilder<RoleRight> builder)
        {
            builder.ToTable("RoleRight").HasKey(x => new { x.RoleId, x.RightId });
        }
    }
}
