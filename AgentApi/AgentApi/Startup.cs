using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentApi.Middlewares;
using AgentApi.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace AgentApi
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgentApi", Version = "v1" });
            });

            var ourServices = typeof(Program).Assembly
                .GetTypes()
                .Where(x => x.Name.EndsWith("Service"));

            foreach (var service in ourServices)
                services.AddScoped(service);

            var mongoConnectionString =
                Environment.GetEnvironmentVariable("AgentApiMongoDb") ?? "mongodb://localhost:27017";

            services.AddSingleton<IMongoClient, MongoClient>(s =>
            {
                var client = new MongoClient(mongoConnectionString);

                var userDb = client.GetDatabase("AgentApi");

                return client;
            });

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddMongoDbStores<User, Role, Guid>(mongoConnectionString, "AgentApi");

            services.AddAutoMapper(typeof(Startup));
            
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true);
            }));
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgentApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            
            app.UseCors("CorsPolicy");

            app.UseMiddleware<AntiXssMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}