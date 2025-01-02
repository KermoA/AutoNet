using AutoNet.ApplicationServices.Services;
using AutoNet.Core.Domain;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using CarsTest.Macros;
using CarsTest.Mock;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CarsTest
{
    public abstract class TestBase
    {
        protected IServiceProvider serviceProvider { get; set; }

        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            // Dispose any resources if needed
        }

        protected T Svc<T>()
        {
            return serviceProvider.GetService<T>();
        }

        protected virtual void SetupServices(IServiceCollection services)
        {
            // Add services
            services.AddScoped<ICarsServices, CarsServices>();
            services.AddScoped<IFileServices, FileServices>();
            services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

            // Mock and configure UserManager
            services.AddScoped(serviceProvider =>
            {
                var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
                var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                );
                return userManagerMock.Object;
            });

            // Configure the in-memory database
            services.AddDbContext<AutoNetContext>(options =>
            {
                options.UseInMemoryDatabase("TEST");
                options.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }

        private void RegisterMacros(IServiceCollection services)
        {
            var macroBaseType = typeof(IMacros);

            var macros = macroBaseType.Assembly.GetTypes()
                .Where(x => macroBaseType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var macro in macros)
            {
                services.AddSingleton(macro);
            }
        }
    }

}
