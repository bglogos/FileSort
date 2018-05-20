using System;
using FileSort.Api.LightInject;
using Microsoft.Extensions.DependencyInjection;

namespace FileSort.Console
{
    /// <summary>
    /// Main application entry point class.
    /// </summary>
    public class Program
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        protected Program()
        {
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            // Setup DI.
            IServiceProvider serviceProvider = ServiceProviderFactory.Create(GetServices());

            App application = serviceProvider.GetService<App>();
            application.Run(args);
        }

        private static ServiceCollection GetServices()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            return services;
        }

        #endregion
    }
}
