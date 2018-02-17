using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Domain;
using Store.Domain.Products;
using Store.Data;
using Store.Data.Repositories;
using Store.Data.Contexties;
using Store.Domain.Sales;

namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connection));
            //Generic Injection
            services.AddSingleton(typeof(IRepository<Product>), typeof(ProductRepository));
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));  
            services.AddSingleton(typeof(CategoryStorer));
            services.AddSingleton(typeof(ProductStorer));
             services.AddSingleton(typeof(SaleFactory));
            services.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
        }
    }
}
