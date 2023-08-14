namespace CMS.News.API.Extensions
{
    public static class WebApplicationExtension
    {
        public static WebApplication InitializeDatabase(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<NewsDbContext>())
                {
                    try
                    {
                        dbContext.Database.Migrate();
                        DataSeeder.SeedData(dbContext);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return webApp;
        }

        public static WebApplication InitTokenLoginInMemory(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                ITokenLoginHandler tokenLoginHandler = scope.ServiceProvider.GetRequiredService<ITokenLoginHandler>();
                Response<List<TokenLogin>> response = tokenLoginHandler.GetAllFromDatabaseAsync().Result;

                if (response.Data.Count > 0)
                {
                    foreach (var item in response.Data)
                    {
                        TokenLoginHandler.DictionaryTokenLogin[item.UserId] = item;
                    }
                }
            }
            return app;
        }
    }
}
