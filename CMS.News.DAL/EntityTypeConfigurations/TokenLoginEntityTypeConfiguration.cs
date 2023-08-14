namespace CMS.News.DAL.EntityTypeConfigurations
{
    internal class TokenLoginEntityTypeConfiguration : IEntityTypeConfiguration<TokenLogin>
    {
        public void Configure(EntityTypeBuilder<TokenLogin> builder)
        {
            builder.ToTable("TokenLogin").HasKey(x => x.UserId);
        }
    }
}
