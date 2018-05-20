using System;
using System.Text;

namespace FileSort.Models.Common
{
    /// <summary>
    /// Constants used application wide.
    /// </summary>
    public static class AppConstants
    {
        #region Dependency injection

        /// <summary>
        /// The FileSort.Business assembly and namespace.
        /// </summary>
        public const string FileSortBusiness = "FileSort.Business";

        /// <summary>
        /// The FileSort.Business namespace.
        /// </summary>
        public const string FileSortBusinessServices = "FileSort.Business.Services";

        /// <summary>
        /// The FileSort.Providers namespace.
        /// </summary>
        public const string FileSortProviders = "FileSort.Providers";

        /// <summary>
        /// The service classes suffix.
        /// </summary>
        public const string ServiceSuffix = "Service";

        /// <summary>
        /// The provider classes suffix.
        /// </summary>
        public const string ProviderSuffix = "Provider";

        #endregion

        #region Sorting configuration

        /// <summary>
        /// The no sort file name format.
        /// </summary>
        public const string NoSortFileNameFormat = "{0}\\{1}\\no_sort_part_{2}.txt";

        /// <summary>
        /// The part file name format.
        /// </summary>
        public const string PartFileNameFormat = "{0}\\{1}\\part_{2}.txt";

        /// <summary>
        /// The workspace directory name.
        /// </summary>
        public const string WorkspaceDirectoryName = "Workspace";

        /// <summary>
        /// The sorted file suffix.
        /// </summary>
        public const string SortedFileSuffix = "sorted";

        /// <summary>
        /// The maximum size of a file part in bytes.
        /// </summary>
        public const int MaxFilePartSize = 100 * 1024 * 1024;

        /// <summary>
        /// Gets the BOM symbol.
        /// </summary>
        public static char[] BomSymbol => new char[] { '\uFEFF' };

        /// <summary>
        /// Gets the default string comparer.
        /// </summary>
        public static StringComparer Comparer => StringComparer.InvariantCultureIgnoreCase;

        /// <summary>
        /// Gets the default text file encoding.
        /// </summary>
        public static Encoding TextEncoding => Encoding.UTF8;

        #endregion
    }
}
