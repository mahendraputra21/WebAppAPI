using System;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebApiCQRS.Context;
using WebApiCQRS.Features.Commands;
using WebApiCQRS.Features.Queries;
using WebApiCQRS.Models;

namespace WebApiCQRS
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
            // Configuring the API Services to support Entity Framework Core
            var mySqlVer = new Version(8, 0, 21);
            var connection = Configuration.GetConnectionString("CqrsDatabase");
            services.AddDbContext<ApplicationContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connection, new MySqlServerVersion(mySqlVer),
                        mysqlOptions => mysqlOptions
                                .EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null)
                                .CommandTimeout(600)
                                .MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)
                            )
                );
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //set the MediatR
            services.AddScoped<IApplicationContext, ApplicationContext>();
            services.AddMediatR(typeof(ApplicationContext).Assembly);

            //set the Swagger
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiCQRS", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiCQRS v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
