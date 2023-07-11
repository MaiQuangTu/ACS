
using ACS.Web.Common;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using LIB.Infrastructure;

namespace ACS.Web
{
	public class Program
	{
		private static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			// Config Service
			builder.Services.AddControllersWithViews();
			builder.Services.AddSession(SessionConfig);

			// Config Logging
			builder.Logging.AddConsole();
			builder.Logging.AddDebug();

			// Config Logging
			Logger logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				//.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
				.CreateLogger();


			builder.Logging.ClearProviders();
			builder.Logging.AddSerilog(logger);

			//builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
			//builder.Services.AddAuthorization(options => options.FallbackPolicy = options.DefaultPolicy);
			//// Get Roles
			//builder.Services.AddSingleton<IClaimsTransformation, ClaimsTransformer>();

			//// Set Constant Value from appsettings
			Constant.ACS_API_SERVER = builder.Configuration["ACS_API_SERVER"];
			Constant.RedirecLink = builder.Configuration["RedirecLink"];

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

			if (!app.Environment.IsDevelopment())
				app.UseHsts();

			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseSession();
			app.UseRouting();
			//app.UseAuthentication();
			//app.UseAuthorization();


		app.UseEndpoints(EndpointConfig);

			app.Run();
		}
		private static void EndpointConfig(IEndpointRouteBuilder builder)
		{

			//builder.MapControllerRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");

			builder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

			//builder.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");
		}
		private static void SessionConfig(SessionOptions options)
		{
			options.IdleTimeout = TimeSpan.FromDays(1);
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

