namespace CMS.News.API.Extensions
{
    internal static class ProjectServiceCollectionExtensions
    {
        internal static IServiceCollection AddProjectServices(this IServiceCollection services) =>
            services
                .AddHttpContextAccessor()
                .AddAutoMapper(typeof(AutoMapperProfile).Assembly)
                .AddDbContext<NewsDbContext>()
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddTransient<JwtAuthenticationManager>()
                .AddHandlerService();

        private static IServiceCollection AddHandlerService(this IServiceCollection services) =>
            services
                .AddScoped<ITokenLoginHandler, TokenLoginHandler>()
                .AddScoped<IUserHandler, UserHandler>()
                .AddScoped<IRightHandler, RightHandler>()
                .AddScoped<IRoleHandler, RoleHandler>()
                .AddScoped<IRoleRightHandler, RoleRightHandler>()
                .AddScoped<ISiteHandler, SiteHandler>();
    }
}
