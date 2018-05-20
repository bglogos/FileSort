using System;
using FileSort.Core.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace FileSort.Console
{
    /// <summary>
    /// The main application class.
    /// </summary>
    public class App : IDisposable
    {
        #region Fields

        private readonly ILoggerFactory _loggerFactory;
        private readonly ISortingService _sortingService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="sortingService">The sorting service.</param>
        public App(ILoggerFactory loggerFactory, ISortingService sortingService)
        {
            _loggerFactory = loggerFactory;
            _sortingService = sortingService;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Runs the application with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Run(string[] args)
        {
            // Configure console logging.
            ILogger logger = _loggerFactory.AddConsole(LogLevel.Information).CreateLogger<Program>();

            try
            {
                // Configure the command line application.
                CommandLineApplication cmdApp = new CommandLineApplication(throwOnUnexpectedArg: true);
                CommandOption fileName = cmdApp.Option("-$|-f |--file <file>", "The text file that will be sorted.", CommandOptionType.SingleValue);
                cmdApp.HelpOption("-? | -h | --help");

                cmdApp.OnExecute(async () =>
                {
                    if (fileName.HasValue())
                    {
                        _sortingService.Logger = logger;
                        string sortedFileName = _sortingService.GenerateSortedFileName(fileName.Value());
                        await _sortingService.SortAsync(fileName.Value(), sortedFileName);
                        bool isSortingValid = await _sortingService.IsSotringValid(sortedFileName);

                        logger.LogInformation(isSortingValid ? "Sorting is valid." : "The sorting is invalid.");
                    }

                    return 0;
                });

                cmdApp.Execute(args);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, args);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loggerFactory.Dispose();
            }
        }

        #endregion
    }
}
