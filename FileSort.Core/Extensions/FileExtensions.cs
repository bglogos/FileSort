using System.IO;
using System.Threading.Tasks;

namespace FileSort.Core.Extensions
{
    /// <summary>
    /// Files and I/O operatipons extension methods.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Deletes asynchronous.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        public static Task DeleteAsync(this FileSystemInfo fileInfo)
            => Task.Factory.StartNew(() => fileInfo.Delete());
    }
}
