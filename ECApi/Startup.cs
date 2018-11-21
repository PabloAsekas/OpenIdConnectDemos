using ECApi.Data;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECApi
{
    public class Startup
    {
        private readonly string contentRootPath;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            contentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddDbContext<CinemaDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection").Replace("%CONTENTROOTPATH%", contentRootPath)));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetValue("AuthorityUrl", "http://localhost:5000/");
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "ECApi";
                    options.ApiSecret = Configuration.GetValue<string>("ApiSecret");
                });

            services.AddAuthorization(options => options.AddPolicy("ECApiPolicy", builder=>builder.RequireScope("ECApi")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
