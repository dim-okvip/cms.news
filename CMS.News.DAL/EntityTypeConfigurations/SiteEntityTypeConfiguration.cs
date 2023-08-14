namespace CMS.News.DAL.EntityTypeConfigurations
{
    internal class SiteEntityTypeConfiguration : IEntityTypeConfiguration<Site>
    {
        public void Configure(EntityTypeBuilder<Site> builder)
        {
            builder.ToTable("Site").HasKey(x => x.Id);
        }
    }
}
