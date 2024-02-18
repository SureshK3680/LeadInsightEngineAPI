using Integrate.Core;
using LeadInsightEngineAPI.Repository;
using LeadInsightEngineAPI.Services;

namespace LeadInsightEngineUploadAPI.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CoreRegistrar.RegisterLogging(services);
            services.AddSingleton<IEntitiesRepository, EntitiesRepository>();
            services.AddSingleton<IEntitiesBatchRepository, EntitiesBatchRepository>();
            services.AddSingleton<IAllocationrepository, Allocationrepository>();
            services.AddSingleton<ILeadQueuePublisher, LeadQueuePublisher>();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}