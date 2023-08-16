namespace CMS.News.DAL.EntityTypeConfigurations
{
    internal class MenuEntityTypeConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menu").HasKey(x => x.Id);
        }
    }
}
