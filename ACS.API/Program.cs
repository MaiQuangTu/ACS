using System.Reflection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ACS.API.Common;
using LIB.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ACS.API
{
	public class Program
	{
		private static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			// Config Service
			builder.Services.AddControllers();
			//builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
			//{
			//    options.Authority = "http://199.40.64.213:8081/";
			//    options.RequireHttpsMetadata = false;
			//    options.Audience = "Subscription";
			//    options.TokenValidationParameters = new TokenValidationParameters
			//    {
			//        ValidateIssuer = false
			//    };
			//});
			builder.Services.AddCors(CorsConfig);

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = builder.Configuration["Jwt:Audience"],
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});

			// Config Logging
			builder.Logging.AddConsole();
			builder.Logging.AddDebug();

			// Set Constant Value from appsettings
			string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(currentPath).AddJsonFile("appsettings.json").Build();



			Constant.RedirecLink = builder.Configuration["RedirecLink"];
			Constant.ShortLinkAPI = builder.Configuration["ShortLinkAPI"];


			#region Dependency Injection

			// Infrastructure
			builder.Services.AddScoped(typeof(IDbFactory), typeof(DbFactory));
			builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

			// Repositories
			MapRepositories(builder.Services);

			// Service
			MapServices(builder.Services);

			#endregion Dependency Injection

			WebApplication app = builder.Build();


			//app.MapGet("/{u:alpha}", (string name) => $"ShortenerURL {name}!");
			//app.MapGet("/{name:alpha}", (string name) => "{controller=ShortenerURL}/{action=GetShortURL}/{name?}");
			app.MapControllerRoute(
				  name: "ShortenerURL",
				  pattern: "/{u:string}",
				  defaults: new { controller = "ShortenerURL", action = "GetShortURL" }
			  );

			app.UseRouting();

			//app.UseEndpoints(endpoints =>
			//{
			//    endpoints.MapGet("/", async context =>
			//    {
			//        await context.Response.WriteAsync("Hello World!");
			//    });
			//});

			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseCors();
			app.UseEndpoints(EndpointConfig);

			app.Run();
		}

		private static void CorsConfig(CorsOptions options)
		{
			options.AddDefaultPolicy(PolicyConfig);
		}

		private static void PolicyConfig(CorsPolicyBuilder builder)
		{
			builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
		}

		private static void EndpointConfig(IEndpointRouteBuilder builder)
		{
			builder.MapControllers();
		}

		private static void MapRepositories(IServiceCollection collection)
		{
			string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LIB.Repositories.dll");
			Assembly assembly = Assembly.LoadFrom(path);

			Type[] types = assembly.GetTypes();
			int length = types.Length;

			for (int i = 0; i < length; i++)
			{
				Type type = types[i];
				if (type.Name.EndsWith("Repository") && type.IsInterface)
				{
					Type typeInterface = type;

					Type typeRepository = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeInterface.IsAssignableFrom(p) && p != typeInterface).FirstOrDefault();

					collection.AddScoped(typeInterface, typeRepository);
				}
			}
		}

		private static void MapServices(IServiceCollection collection)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			Type[] types = assembly.GetTypes();
			int length = types.Length;

			for (int i = 0; i < length; i++)
			{
				Type type = types[i];
				if (type.Name.EndsWith("Service") && type.IsInterface)
				{
					Type typeInterface = type;

					Type typeRepository = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeInterface.IsAssignableFrom(p) && p != typeInterface).FirstOrDefault();

					collection.AddScoped(typeInterface, typeRepository);
				}
			}
		}
	}
}