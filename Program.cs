
using Microsoft.EntityFrameworkCore;
using Quartz;
using TMAMS_Data_Transmitter.Data;
using TMAMS_Data_Transmitter.Jobs;
using TMAMS_Data_Transmitter.Requests;
using TMAMS_Data_Transmitter.Services;
using Topshelf;

namespace TMAMS_Data_Transmitter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));
            // Add services to the container.
            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=mydatabase.db"));

            builder.Services.AddTransient<MqttClientService>();

            // Add Quartz services
            builder.Services.AddQuartz(q =>
            {
                // Use a Scoped DI job factory
                q.UseMicrosoftDependencyInjectionJobFactory();

                // Register the job and trigger
                var jobKey = new JobKey("DataDeleter");
                q.AddJob<DataDeleter>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey) // link to the MyJob
                    .WithIdentity("DataDeleter-trigger") // give the trigger a unique name
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromSeconds(1)) // run every 10 seconds
                        .RepeatForever())); // repeat forever



                var job2Key = new JobKey("DataTransmitter");
                q.AddJob<DataTransmitter>(opts => opts.WithIdentity(job2Key));
                q.AddTrigger(opts => opts
                    .ForJob(job2Key) // link to the MyJob
                    .WithIdentity("DataTransmitter-trigger") // give the trigger a unique name
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromSeconds(1)) // run every 10 seconds
                        .RepeatForever())); // repeat forever

            });

            // Add the Quartz hosted service
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapReverseProxy();
            app.MapControllers();

            HostFactory.Run(x =>
            {
                x.Service<WebApiService>(s =>
                {
                    s.ConstructUsing(name => new WebApiService(app));
                    s.WhenStarted(async tc => await tc.StartAsync(default));
                    s.WhenStopped(async tc => await tc.StopAsync(default));
                });
                x.RunAsLocalSystem();

                x.SetDescription("CM Data Transfer Worker Service");
                x.SetDisplayName("CM Data Transfer Worker");
                x.SetServiceName("CMDataTransferWorker");
            });
        }
    }
}
