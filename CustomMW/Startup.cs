using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CustomMW
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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,

                   ValidIssuer = "alesta.bordatech.com",
                   ValidAudience = "alesta.bordatech.com",
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("fdsfsdfsdfsd"))
               };
           });

            services.AddHangfire(m => m.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
                        
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.Use(async (HttpContext ctx, Func<Task> next) =>
            {
                var request = ctx.Request;
                //await ctx.Response.WriteAsync("1. asamada");
                string test = ctx.Request.Headers["username"].ToString();

                Console.WriteLine("REQUEST {0}", DateTime.Now.ToLongTimeString());
                Console.WriteLine("{0}-{1}{2}", request.Method, request.Host, request.Path);

                await next.Invoke();

                var response = ctx.Response;
                if (response.StatusCode == 401)
                    await ctx.Response.WriteAsync("basarili");
                Console.WriteLine("RESPONSE:{0}", DateTime.Now.ToLongTimeString());
                Console.WriteLine("{0}\t({1}){2}", response.ContentType, response.StatusCode, (HttpStatusCode)response.StatusCode);
            });

            app.UseMvc();
        }
    }
}
