using BrazilianHedgeFunds.API.Repository;
using BrazilianHedgeFunds.API.Repository.Interfaces;
using BrazilianHedgeFunds.API.Services;
using BrazilianHedgeFunds.API.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace BrazilianHedgeFunds.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "BrazilianHedgeFunds.API", Version = "v1" }));

            services.AddDbContextFactory<Context>(option => option.UseSqlServer(GetConnectionString()));

            services.AddTransient<IHedgeFundService, HedgeFundService>();
            services.AddTransient<IHedgeFundRecordRepository, HedgeFundRecordRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BrazilianHedgeFunds.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private string GetConnectionString() => Configuration.GetConnectionString("Default");
    }
}
