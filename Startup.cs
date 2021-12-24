using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HostAccessibilityCheckingSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddRouting();
            services.AddMvc(opt => opt.EnableEndpointRouting = false);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/home/login";
                options.AccessDeniedPath = "/home/denied";
                options.Events = new CookieAuthenticationEvents()
                {
                    //when trying to sign in
                    OnSigningIn = async context =>
                    {
                        //here i can add some claims
                        await Task.CompletedTask;
                    },
                    //when password and username is correct
                    OnSignedIn = async context =>
                    {
                        await Task.CompletedTask;
                    },
                    //called on every auth request (every time that someone authentificated trying access some page that needs authentification)
                    OnValidatePrincipal = async context =>
                    {
                        await Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days.
                app.UseHsts();
            }
            app.UseMvcWithDefaultRoute();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
