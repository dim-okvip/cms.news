using CMS.News.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
const string CORS_POLICY = "CorsPolicy";

builder.Configuration.AddJsonFile("appsettings.json", false, true);

builder.Services.AddCors(o => o.AddPolicy(CORS_POLICY, builder =>
{
    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
           .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NewsAPI", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Example: 'Bearer 12345abcdef",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement(){
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
            }
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSecurityKey"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddProjectServices();

WebApplication app = builder.Build();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});

app.UseCors(CORS_POLICY);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BillMaker v1"));

app.UseHttpsRedirection();

//var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
//var provider = new FileExtensionContentTypeProvider();
//app.UseStaticFiles(new StaticFileOptions
//{
//    OnPrepareResponse = ctx =>
//    {
//        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
//        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
//        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
//    },
//    ContentTypeProvider = provider,
//    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Upload")),
//    RequestPath = "/Upload",
//});

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    //OnPrepareResponse = (ctx) =>
    //{
    //    ICorsPolicyProvider corsPolicyProvider = app.Services.GetRequiredService<ICorsPolicyProvider>();
    //    ICorsService corsService = app.Services.GetRequiredService<ICorsService>();

    //    CorsPolicy? policy = corsPolicyProvider.GetPolicyAsync(ctx.Context, CORS_POLICY).ConfigureAwait(false).GetAwaiter().GetResult();
    //    if (policy is not null)
    //    {
    //        CorsResult corsResult = corsService.EvaluatePolicy(ctx.Context, policy);
    //        corsService.ApplyResult(corsResult, ctx.Context.Response);
    //    }
    //}
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
          "Origin, X-Requested-With, Content-Type, Accept");
    },
});

app.UseAuthorization();

app.MapControllers();

app.InitializeDatabase();

app.InitTokenLoginInMemory();

app.Run();