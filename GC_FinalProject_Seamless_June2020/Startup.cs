using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using GC_FinalProject_Seamless_June2020.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GC_FinalProject_Seamless_June2020.Models;

namespace GC_FinalProject_Seamless_June2020
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
            var connection = Configuration.GetConnectionString("DBContext");
            services.AddDbContext<SeamedInDBContext>(options => options.UseSqlServer(connection));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DBContext")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.Configure<SecurityStampValidatorOptions>(options =>
                    options.ValidationInterval = TimeSpan.Zero
                    );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
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
            //CreateRoles(services,"example@example.com").Wait();
        }
        //private async Task CreateRoles(IServiceProvider serviceProvider, string email)
        //{
        //    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        //    IdentityResult roleResult;
        //    //here in this line we are adding Admin Role
        //    var roleCheck = await RoleManager.RoleExistsAsync("Admin");
        //    if (!roleCheck)
        //    {
        //        //here in this line we are creating admin role and seed it to the database
        //        roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
        //    }
        //    //here we are assigning the Admin role to the User that we have registered above 
        //    //Now, we are assinging admin role to this user("Ali@gmail.com"). When will we run this project then it will
        //    //be assigned to that user.
        //    IdentityUser user = await UserManager.FindByEmailAsync(email);
        //    var User = new IdentityUser();
        //    await UserManager.AddToRoleAsync(user, "Admin");
        //}
    }
}
