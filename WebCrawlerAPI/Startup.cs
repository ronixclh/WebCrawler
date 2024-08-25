using Microsoft.EntityFrameworkCore;
using WebCrawlerAPI.Data;
using WebCrawlerAPI.Services;
using WebCrawlerAPI.Services.Contracts;

namespace WebCrawlerAPI
{
    public class Startup
    {
        
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen();

            services.AddScoped<IScrapper, Scraper>();
            services.AddScoped<IFilter, Filter>();
            services.AddScoped<ICrawlerLogger, Logger>();
            services.AddScoped<IHtmlLoader, HtmlLoader>(); 



            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebCrawler API v1");
                c.RoutePrefix = string.Empty; 
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
