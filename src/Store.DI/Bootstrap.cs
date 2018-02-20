using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Domain;
using Store.Domain.Products;
using Store.Data;
using Store.Data.Repositories;
using Store.Data.Contexties;
using Store.Domain.Sales;
using Store.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Store.Domain.Account;

namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<ApplicationUser, IdentityRole>(config => {

                    config.Password.RequireDigit = false;
                    config.Password.RequiredLength = 3;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    //config.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //Generic Injection
            services.AddSingleton(typeof(IRepository<Product>), typeof(ProductRepository));
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));  
            services.AddSingleton(typeof(IAuthentication), typeof(Authentication)); 
            services.AddSingleton(typeof(IManager), typeof(Manager)); 
            services.AddSingleton(typeof(CategoryStorer));
            services.AddSingleton(typeof(ProductStorer));
            services.AddSingleton(typeof(SaleFactory));
            services.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
        }
    }
}
