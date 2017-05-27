using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mdrdb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using mdrdb.Infrastructure;

namespace mdrdb
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc();

            services.AddDbContext<DrdbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DRDBConnection")));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new EFLoggerProvider());

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc(m =>
            {
                m.MapRoute(
                    name: "list-page",
                    template: "{id:int?}",
                    defaults: new { controller = "Drs", action = "List" });

                m.MapRoute(
                    name: "default",
                    template: "{controller=Drs}/{action=List}/{id:int?}");
            });

            app.UseStaticFiles();
        }
    }
}
