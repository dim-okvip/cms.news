namespace CMS.News.DAL.EntityTypeConfigurations
{
    internal class MenuItemEntityTypeConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItem").HasKey(x => x.Id);
        }
    }
}
