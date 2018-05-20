using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileSort.Core.Extensions;
using FileSort.Core.Providers;
using FileSort.Models.Data;

namespace FileSort.Providers
{
    /// <summary>
    /// The default implementation of <see cref="IFileProvider" />.
    /// </summary>
    public class FileProvider : IFileProvider
    {
        #region Public methods

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// The streams encapsulated in a StreamData object.
        /// </returns>
        public StreamData OpenFileForReading(string filePath, Encoding encoding) =>
            OpenFile(filePath, encoding, FileAccess.Read, false);

        /// <summary>
        /// Opens file for writing.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="isNew">If set to <c>true</c> always create new file.</param>
        /// <returns>
        /// The streams encapsulated in a StreamData object.
        /// </returns>
        public StreamData OpenFileForWriting(string filePath, Encoding encoding, bool isNew = false) =>
            OpenFile(filePath, encoding, FileAccess.Write, isNew);

        /// <summary>
        /// Opens file for reading and writing.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="isNew">If set to <c>true</c> always create new file.</param>
        /// <returns>
        /// The streams encapsulated in a StreamData object.
        /// </returns>
        public StreamData OpenFileForReadingAndWriting(string filePath, Encoding encoding, bool isNew = false) =>
            OpenFile(filePath, encoding, FileAccess.ReadWrite, isNew);

        /// <summary>
        /// Reads the next text line asynchronous.
        /// </summary>
        /// <param name="streamData">The stream data.</param>
        /// <returns>
        /// The next student data row from the stream reader.
        /// </returns>
        public Task<string> ReadNextLineAsync(StreamData streamData) =>
            streamData.StreamReaderData.ReadLineAsync();

        /// <summary>
        /// Writes a text line in given file asynchronous.
        /// </summary>
        /// <param name="line">The text line.</param>
        /// <param name="streamData">The stream data.</param>
        public Task WriteLineInFileAsync(string line, StreamData streamData) =>
            streamData.StreamWriterData.WriteLineAsync(line);

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// File info for the selected file path.
        /// </returns>
        /// <exception cref="FileNotFoundException">The given file path is invalid.</exception>
        public FileInfo GetExistingFileInfo(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            // Check if the file exists
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(string.Format("File {0} does not exist", filePath));
            }

            return fileInfo;
        }

        /// <summary>
        /// Flushes the stream writer asynchronous.
        /// </summary>
        /// <param name="streamData">The stream data.</param>
        public Task FlushStreamWriterAsync(StreamData streamData) =>
            streamData.StreamWriterData.FlushAsync();

        /// <summary>
        /// Deletes the file if exists asynchronous.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public async Task DeleteFileIfExistsAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                await fileInfo.DeleteAsync();
            }
        }

        /// <summary>
        /// Deletes the directory recursively.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        public void DeleteDirectoryRecursively(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        /// <summary>
        /// Copies the provided input file into the output asynchronous.
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <param name="output">The output file.</param>
        /// <param name="encoding">The encoding.</param>
        public Task CopyFileAsync(StreamData input, StreamData output, Encoding encoding) =>
            input.BufferedStreamData.CopyToAsync(output.BufferedStreamData);

        /// <summary>
        /// Merges the files asynchronous.
        /// </summary>
        /// <param name="streams">The streams.</param>
        /// <param name="mergedFileName">Name of the merged file.</param>
        /// <param name="encoding">The encoding.</param>
        public async Task MergeFilesAsync(IEnumerable<StreamData> streams, string mergedFileName, Encoding encoding)
        {
            using (StreamData mergedStream = OpenFileForWriting(mergedFileName, encoding, true))
            {
                foreach (StreamData stream in streams)
                {
                    await CopyFileAsync(stream, mergedStream, encoding);
                }
            }
        }

        /// <summary>
        /// Adds the suffix to file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>
        /// The provided file name with added suffix before the extension.
        /// </returns>
        public string AddSuffixToFileName(string fileName, string suffix) =>
            Path.Combine(
                Path.GetDirectoryName(fileName),
                $"{Path.GetFileNameWithoutExtension(fileName)}_{suffix}",
                Path.GetExtension(fileName));

        #endregion

        #region Private methods

        private StreamData OpenFile(string filePath, Encoding encoding, FileAccess access, bool isNew = false)
        {
            string destinationDirectory = Path.GetDirectoryName(filePath);

            if (access.HasFlag(FileAccess.Write) && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            FileMode mode;

            if (access.HasFlag(FileAccess.Write))
            {
                mode = isNew ? FileMode.Create : FileMode.OpenOrCreate;
            }
            else
            {
                mode = FileMode.Open;
            }

            StreamData streamData = new StreamData
            {
                FileStreamData = File.Open(filePath, mode, access)
            };

            streamData.BufferedStreamData = new BufferedStream(streamData.FileStreamData);
            streamData.StreamReaderData = access.HasFlag(FileAccess.Read) ? new StreamReader(streamData.BufferedStreamData) : null;
            streamData.StreamWriterData = access.HasFlag(FileAccess.Write) ? new StreamWriter(streamData.BufferedStreamData, encoding) : null;

            return streamData;
        }

        #endregion
    }
}
