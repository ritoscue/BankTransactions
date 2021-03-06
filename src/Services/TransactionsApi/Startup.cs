﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TransactionsApi.Data;
using System.Text;
namespace TransactionsApi
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
            services.AddDbContext<TransactionDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TransactionConexion")));  
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(Options =>
                    Options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "transaction.com",
                        ValidAudience = "transaction.com",
                        IssuerSigningKey =  new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("#PreguntaSecreta1")),
                        ClockSkew = TimeSpan.Zero
                    });
            services.AddMvc();
            services.AddSwaggerGen(Options =>
            {
                Options.DescribeAllEnumsAsStrings();
                Options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info{
                    Title = "Transactions API",
                    Version ="v1",
                    Description ="Transactions API",
                    TermsOfService ="Terms of Service"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json","Transaction Catalog v1");
            });
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
