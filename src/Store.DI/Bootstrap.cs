using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Data;
namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connection));
        }
    }
}
