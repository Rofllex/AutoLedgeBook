using AutoLedgeBook.UpdateServer.Controllers;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;

namespace AutoLedgeBook.UpdateServer
{
    public class Program
    {
        public static readonly string ExecutableFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static Program()
        {

        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.Configure<TraceFileOptions>(builder.Configuration.GetSection(TraceFileOptions.ConfigLocation));

            

            builder.Services.AddSingleton<FileVersionaryWatcher>(services =>
            {
                var options = services.GetRequiredService<IOptions<TraceFileOptions>>();
                var logger = services.GetRequiredService<ILogger<FileVersionaryWatcher>>();

                string zipArchive = options.Value.TraceZipArchive;
                string zipPath = Path.Combine(ExecutableFileDirectory, zipArchive);
                var watcher = new FileVersionaryWatcher(zipPath, options.Value.MainFileName);
                watcher.ModelChanged += (_, model) =>
                {
                    string message = new StringBuilder().AppendLine("Модель изменилась")
                                                            .AppendLine(model.BranchName)
                                                            .AppendLine(model.Sha)
                                                            .AppendLine(model.InformationalVersion)
                                                            .ToString();

                    logger.LogInformation(message);
                };


                return watcher;
            });

            var app = builder.Build();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            
            app.Run();
        }
    }
}