
using DBQueue;
using GINotificationManager.Services;
using GINotificationManager.Tasks;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace GINotificationManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GINotificationManager",
                    Description = "Gestisce le notifiche GI",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "L. Maddalena",
                        Email = "l.maddalena@example.com",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });

                // using System.Reflection;
                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddTransient<IQueueProvider, QueueProvider>();
            builder.Services.AddHostedService<QueuedHostedService>();
            builder.Services.AddTransient<IWorkTask<CambioStatoTask>, CambioStatoTask>();
            builder.Services.AddTransient<IWorkTask<DispatchNotificaGITask>, DispatchNotificaGITask>();
            builder.Services.AddTransient<IWorkTask<ScartaNotificaTask>, ScartaNotificaTask>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
