using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileSort.Models.Data;

namespace FileSort.Core.Providers
{
    /// <summary>
    /// Provides methods for reading and writing files.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Opens file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// The streams encapsulated in a StreamData object.
        /// </returns>
        StreamData OpenFileForReading(string filePath, Encoding encoding);

        /// <summary>
        /// Opens file for writing.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="isNew">If set to <c>true</c> always create new file.</param>
        /// <returns>
        /// The streams encapsulated in a StreamData object.
        /// </returns>
        StreamData OpenFileForWriting(string filePath, Encoding encoding, bool isNew = false);

        /// <summary>
        /// Opens file for reading and writing.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="isNew">If set to <c>true</c> always create new file.</param>
        /// <returns>
        /// The streams encapsulated in a StreamData object.
        /// </returns>
        StreamData OpenFileForReadingAndWriting(string filePath, Encoding encoding, bool isNew = false);

        /// <summary>
        /// Reads the next text line asynchronously.
        /// </summary>
        /// <param name="streamData">The stream data.</param>
        /// <returns>
        /// The next student data row from the stream reader.
        /// </returns>
        Task<string> ReadNextLineAsync(StreamData streamData);

        /// <summary>
        /// Writes a text line in given file asynchronous.
        /// </summary>
        /// <param name="line">The text line.</param>
        /// <param name="streamData">The stream data.</param>
        Task WriteLineInFileAsync(string line, StreamData streamData);

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// File info for the selected file path.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the file path is invalid.
        /// </exception>
        FileInfo GetExistingFileInfo(string filePath);

        /// <summary>
        /// Flushes the stream writer asynchronous.
        /// </summary>
        /// <param name="streamData">The stream data.</param>
        Task FlushStreamWriterAsync(StreamData streamData);

        /// <summary>
        /// Deletes the file if exists asynchronous.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        Task DeleteFileIfExistsAsync(string filePath);

        /// <summary>
        /// Deletes the directory recursively.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        void DeleteDirectoryRecursively(string directoryPath);

        /// <summary>
        /// Copies the provided input file into the output asynchronous.
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <param name="output">The output file.</param>
        /// <param name="encoding">The encoding.</param>
        Task CopyFileAsync(StreamData input, StreamData output, Encoding encoding);

        /// <summary>
        /// Merges the files asynchronous.
        /// </summary>
        /// <param name="streams">The streams.</param>
        /// <param name="mergedFileName">Name of the merged file.</param>
        /// <param name="encoding">The encoding.</param>
        Task MergeFilesAsync(IEnumerable<StreamData> streams, string mergedFileName, Encoding encoding);

        /// <summary>
        /// Adds the suffix to file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>
        /// The provided file name with added suffix before the extension.
        /// </returns>
        string AddSuffixToFileName(string fileName, string suffix);
    }
}
