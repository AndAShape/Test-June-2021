using DataAccess;
using Domain.Abstract.Orders;
using Domain.Orders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MMT_Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var dBConnection = Configuration.GetValue<string>("DbConnectionString");

            var apiKey = Configuration.GetValue<string>("ApiKey");
            var apiUrl = Configuration.GetValue<string>("BaseUrl");

            services.AddTransient<IOrderService, OrderService>(f =>
            {
                var customerRepository = new CustomerRepository(apiKey, apiUrl);
                var orderRepository = new OrderRepository(dBConnection);

                var orderService = new OrderService(customerRepository, orderRepository);

                return orderService;
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
