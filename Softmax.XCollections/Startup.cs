using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Softmax.XCollections.Data;
using Softmax.XCollections.Models;
using Softmax.XCollections.Services;
using AutoMapper;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Contracts.Validations;
using Softmax.XCollections.Data.Repositories;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Services.Validations;
using Softmax.XCollections.Utilities;
using Softmax.XCollections.Models.Settings;

namespace Softmax.XCollections
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
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.Configure<SmtpSettingsModel>(Configuration.GetSection("SmtpSettings"));
            services.Configure<FeeSettingsModel>(Configuration.GetSection("FeeSettings"));


            // Add application services.
            services.AddTransient<DbContext, ApplicationDbContext>();
            services.AddTransient<IMessager, Messager>();
            services.AddTransient<IGenerator, Generator>();
            services.AddTransient<IXLogger, XLogger>();
            services.AddTransient<IMapper, Mapper>();

            services.AddTransient<IRepository<Branch>, Repository<Branch>>();
            services.AddTransient<IRepository<Customer>, Repository<Customer>>();
            services.AddTransient<IRepository<Deposit>, Repository<Deposit>>();
            services.AddTransient<IRepository<DepositConfirm>, Repository<DepositConfirm>>();
            services.AddTransient<IRepository<Employee>, Repository<Employee>>();
            services.AddTransient<IRepository<Loan>, Repository<Loan>>();
            services.AddTransient<IRepository<LoanApproval>, Repository<LoanApproval>>();
            services.AddTransient<IRepository<Product>, Repository<Product>>();
            services.AddTransient<IRepository<Refund>, Repository<Refund>>();
            services.AddTransient<IRepository<RefundConfirm>, Repository<RefundConfirm>>();
            
            services.AddTransient<IBranchService, BranchService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IDepositService, DepositService>();
            services.AddTransient<IDepositConfirmService, DepositConfirmService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<ILoanService, LoanService>();
            services.AddTransient<ILoanApprovalService, LoanApprovalService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IRefundService, RefundService>();
            services.AddTransient<IRefundConfirmService, RefundConfirmService>();
            services.AddTransient<IWithdrawalService, WithdrawalService>();
            services.AddTransient<IBranchValidation, BranchValidation>();
            services.AddTransient<ICustomerValidation, CustomerValidation>();
            services.AddTransient<IDepositValidation, DepositValidation>();
            services.AddTransient<IDepositConfirmValidation, DepositConfirmValidation>();
            services.AddTransient<IEmployeeValidation, EmployeeValidation>();
            services.AddTransient<ILoanValidation, LoanValidation>();
            services.AddTransient<ILoanApprovalValidation, LoanApprovalValidation>();
            services.AddTransient<IProductValidation, ProductValidation>();
            services.AddTransient<IRefundValidation, RefundValidation>();
            services.AddTransient<IRefundConfirmValidation, RefundConfirmValidation>();

            services.AddMvc();
            services.AddAutoMapper();          
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IServiceProvider serviceProvider, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateRoles(serviceProvider).Wait();
            loggerFactory.AddFile("wwwroot/Logs/messages-{Date}.txt");
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "Manager", "Supervisor", "Officer", "Customer"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var poweruser = new ApplicationUser
            {
                UserName = Configuration.GetSection("UserSettings")["UserEmail"],
                Email = Configuration.GetSection("UserSettings")["UserEmail"],
                IsTempPassword = true,
                IsActive = true,
                FirstName = "System",
                LastName = "Admin",
                PhoneNumber = "08021276962",
                EmailConfirmed = true
            };

            var userPassword = Configuration.GetSection("UserSettings")["UserPassword"];
            var _user = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPassword);
                if (createPowerUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(poweruser, "Admin");

                }
            }
        }
    }
}
