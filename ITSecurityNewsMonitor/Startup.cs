using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using ITSecurityNewsMonitor.Data;
using ITSecurityNewsMonitor.Helper;
using ITSecurityNewsMonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("Postgres")));

            services.AddDbContext<SecNewsDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("Postgres")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            var mvcBuilder = services.AddControllersWithViews();

            if (Env.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            services.AddControllers(config =>
            {
                // using Microsoft.AspNetCore.Mvc.Authorization;
                // using Microsoft.AspNetCore.Authorization;
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddHangfire(configuration => configuration
             .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
             .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
             .UsePostgreSqlStorage(Configuration.GetConnectionString("Hangfire")));

            services.AddHangfireServer();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                // options.ExcludedHosts.Add("example.com");
            });

            services.AddTransient<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            // BackgroundJob.Enqueue<Crawler>(c => c.ExecuteCrawl());
            // RecurringJob.AddOrUpdate<Crawler>(c => c.ExecuteCrawl(), "*/10 * * * *");
            // RecurringJob.AddOrUpdate<Crawler>(c => c.DeleteOld(), "0 1 * * *");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHangfireDashboard();
            });

            SeedRoles(roleManager, userManager).Wait();
        }

        public async Task SeedRoles(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {

            string[] roles = new[] {
                "Admin"
            };

            foreach (var role in roles)
            {

                if (!await roleManager.RoleExistsAsync(role))
                {
                    IdentityResult create = await roleManager.CreateAsync(new IdentityRole(role));

                    if (!create.Succeeded)
                    {

                        throw new Exception("Failed to create role");

                    }
                }
            }

            if(Configuration.GetValue<string>("DefaultUser:Name") != null && Configuration.GetValue<string>("DefaultUser:Name") != null)
            {
                IdentityUser defaultUser = new IdentityUser { UserName = Configuration.GetValue<string>("DefaultUser:Name"), Email = Configuration.GetValue<string>("DefaultUser:Name"), EmailConfirmed = true };
                IdentityResult result = await userManager.CreateAsync(defaultUser, Configuration.GetValue<string>("DefaultUser:Password"));

                if (result.Succeeded)
                {
                    IdentityResult result1 = await userManager.AddToRoleAsync(defaultUser, "Admin");
                }
            }


        }
    }
}
