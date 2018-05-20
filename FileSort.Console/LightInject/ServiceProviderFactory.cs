using System;
using System.Reflection;
using FileSort.Console;
using FileSort.Models.Common;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FileSort.Api.LightInject
{
    /// <summary>
    /// A factory class that create and fill with components a service provider.
    /// </summary>
    public static class ServiceProviderFactory
    {
        #region Fields

        private static readonly Func<string, Assembly> Load = assemblyName => Assembly.Load(new AssemblyName(assemblyName));

        #endregion

        #region Public methods

        /// <summary>
        /// Create LightInject service provider that resolves objects.
        /// </summary>
        /// <param name="serviceCollection">The current registered services.</param>
        public static IServiceProvider Create(IServiceCollection serviceCollection)
        {
            // Create DI container.
            ServiceContainer container = new ServiceContainer(new ContainerOptions { EnablePropertyInjection = false });
            container.RegisterServices();
            IServiceProvider serviceProvider = container.CreateServiceProvider(serviceCollection);

            return serviceProvider;
        }

        #endregion

        #region Private methods

        private static void RegisterServices(this IServiceContainer container)
        {
            // Fixes issue when there is a continuation during asynchronous execution.
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();

            // Register the main application class.
            container.Register<App, App>(new PerContainerLifetime());

            // Register providers.
            container.RegisterAssembly(
                Load(AppConstants.FileSortProviders),
                () => new PerScopeLifetime(),
                (serviceType, implementationType) =>
                    serviceType.GetTypeInfo().IsInterface &&
                    implementationType.Name.EndsWith(AppConstants.ProviderSuffix) &&
                    implementationType.Namespace.StartsWith(AppConstants.FileSortProviders));

            // Register services.
            container.RegisterAssembly(
                Load(AppConstants.FileSortBusiness),
                () => new PerScopeLifetime(),
                (serviceType, implementationType) =>
                    serviceType.GetTypeInfo().IsInterface &&
                    implementationType.Name.EndsWith(AppConstants.ServiceSuffix) &&
                    implementationType.Namespace.StartsWith(AppConstants.FileSortBusinessServices));
        }

        #endregion
    }
}
