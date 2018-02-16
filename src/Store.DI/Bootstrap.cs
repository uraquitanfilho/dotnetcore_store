using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Domain;
using Store.Domain.Products;

namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connection));
            //Generic Injection
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));  
            services.AddSingleton(typeof(CategoryStorer));
            services.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
        }
    }
}
