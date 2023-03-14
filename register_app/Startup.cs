using AutoMapper;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using register_app.Configuration;
using register_app.Data;
using register_app.Data.Roles;
using register_app.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace register_app
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            services.AddControllersWithViews();
            services.AddRazorPages();
            

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();

            services.AddAuthentication(o =>
            {
                // This forces challenge results to be handled by Google OpenID Handler, so there's no
                // need to add an AccountController that emits challenges for Login.
                o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                // This forces forbid results to be handled by Google OpenID Handler, which checks if
                // extra scopes are required and does automatic incremental auth.
                o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                // Default scheme that will handle everything else.
                // Once a user is authenticated, the OAuth2 token info is stored in cookies.
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogleOpenIdConnect(options =>
            {
                options.ClientId = Configuration.GetValue<string>("web:client_id");
                options.ClientSecret = Configuration.GetValue<string>("web:client_secret");
            });

            services.AddSingleton(mapper);
            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IAttendeeService, AttendeeService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IAdminService, AdminService>();

            InitializeRoles(services);
            InitializeAdmin(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private void InitializeRoles(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

                    foreach (var role in Roles.AllRoles)
                    {
                        if (roleManager.Roles.Any(x => x.Name == role))
                        {
                            continue;
                        }

                        var result = roleManager.CreateAsync(
                            new IdentityRole
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = role
                            }).Result;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void InitializeAdmin(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

                    if (userManager.Users.Any(x => x.UserName == "admin"))
                    {
                        return;
                    }

                    var identityResult = userManager.CreateAsync(
                        new IdentityUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = "admin",
                            UserName = "admin",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                        },
                        "P@ssw0rd").Result;

                    var adminUser = userManager.Users.FirstOrDefault(x => x.UserName == "admin");
                    var result = userManager.AddToRoleAsync(adminUser, "Admin").Result;
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
