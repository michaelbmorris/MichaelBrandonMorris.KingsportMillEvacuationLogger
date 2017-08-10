using System;
using System.Threading.Tasks;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Data;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Models;
using MichaelBrandonMorris.KingsportMillEvacuationLogger.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NonFactors.Mvc.Grid;

namespace MichaelBrandonMorris.KingsportMillEvacuationLogger
{
    /// <summary>
    ///     Class Startup.
    /// </summary>
    /// TODO Edit XML Comment Template for Startup
    public class Startup
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="Startup" /> class.
        /// </summary>
        /// <param name="env">The env.</param>
        /// TODO Edit XML Comment Template for #ctor
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        /// TODO Edit XML Comment Template for Configuration
        public IConfigurationRoot Configuration
        {
            get;
        }

        /// <summary>
        ///     Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// TODO Edit XML Comment Template for Configure
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("logs/log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();

            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        "default",
                        "{controller=Home}/{action=Index}/{id?}");
                });

            CreateRolesAndUser(serviceProvider, loggerFactory).Wait();
        }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// TODO Edit XML Comment Template for ConfigureServices
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>(
                    config =>
                    {
                        config.SignIn.RequireConfirmedEmail = false;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddMvcGrid();

            services.Configure<SmtpOptions>(
                Configuration.GetSection("SmtpOptions"));

            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<ActiveDirectoryColumnMapping>(
                Configuration.GetSection("ActiveDirectoryColumnMapping"));
        }

        private async Task CreateRolesAndUser(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<Role>>();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<User>>();

            var emailSender = serviceProvider
                .GetRequiredService<IEmailSender>();

            var logger = loggerFactory.CreateLogger("");

            string[] roleNames =
            {
                "User",
                "Commander",
                "Security",
                "Administrator",
                "Owner"
            };

            for (var i = 0; i < roleNames.Length; i++)
            {
                if (!await roleManager.RoleExistsAsync(roleNames[i]))
                {
                    await roleManager.CreateAsync(new Role(roleNames[i], i));
                }
            }

            var ownerEmail = Configuration["OwnerEmail"];

            var owner = new User
            {
                Email = ownerEmail,
                UserName = ownerEmail
            };

            if (await userManager.FindByEmailAsync(owner.Email) == null)
            {
                var allowedChars = new[]
                {
                    'a',
                    'b',
                    'c',
                    'd',
                    'e',
                    'f',
                    'g',
                    'h',
                    'i',
                    'j',
                    'k',
                    'l',
                    'm',
                    'n',
                    'o',
                    'p',
                    'q',
                    'r',
                    's',
                    't',
                    'u',
                    'v',
                    'w',
                    'x',
                    'y',
                    'z',
                    'A',
                    'B',
                    'C',
                    'D',
                    'E',
                    'F',
                    'G',
                    'H',
                    'I',
                    'J',
                    'K',
                    'L',
                    'M',
                    'N',
                    'O',
                    'P',
                    'Q',
                    'R',
                    'S',
                    'T',
                    'U',
                    'V',
                    'W',
                    'X',
                    'Y',
                    'Z',
                    '1',
                    '2',
                    '3',
                    '4',
                    '5',
                    '6',
                    '7',
                    '8',
                    '9',
                    '0',
                    '!',
                    '@',
                    '#',
                    '$',
                    '%',
                    '^',
                    '&',
                    '*',
                    '(',
                    ')'
                };

                var password = new char[8];
                var random = new Random();

                for (var i = 0; i < 8; i++)
                {
                    password[i] =
                        allowedChars[random.Next(allowedChars.Length)];
                }

                var passwordString = new string(password);

                logger.LogInformation("Owner password: " + passwordString);
                await userManager.CreateAsync(owner, passwordString);
                owner = await userManager.FindByEmailAsync(owner.Email);

                var token = await userManager
                    .GenerateEmailConfirmationTokenAsync(owner);

                await userManager.ConfirmEmailAsync(owner, token);
                await userManager.AddToRoleAsync(owner, "Owner");

                await emailSender.SendEmailAsync(
                    string.Empty,
                    owner.Email,
                    "Password for Kingsport Mill Evacuation Logger",
                    null,
                    passwordString);
            }
        }
    }
}