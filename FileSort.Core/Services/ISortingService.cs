using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FileSort.Core.Services
{
    /// <summary>
    /// Service for sorting operations.
    /// </summary>
    public interface ISortingService
    {
        /// <summary>
        /// Gets or sets the default logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Generates the name of the sorted file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <returns>
        /// The name of the sorted file.
        /// </returns>
        string GenerateSortedFileName(string fileName);

        /// <summary>
        /// Sorts the specified input file asynchronous.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="outputFileName">Name of the output file.</param>
        Task SortAsync(string inputFileName, string outputFileName);

        /// <summary>
        /// Determines whether the sorting is valid.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <returns>
        ///   <c>true</c> if the sorting is valid; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsSotringValid(string fileName);
    }
}
