using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileSort.Core.Providers;
using FileSort.Core.Services;
using FileSort.Models.Common;
using FileSort.Models.Data;
using Microsoft.Extensions.Logging;

namespace FileSort.Business.Services
{
    /// <summary>
    /// The default implementation of <see cref="ISortingService"/>
    /// </summary>
    public class SortingService : ISortingService
    {
        #region Fields

        private readonly IFileProvider _fileProvider;
        private string _baseDir;

        private int _sortFileCount = 0;
        private int _noSortFileCount = 0;

        private SortedDictionary<string, StreamData> _sortParts;
        private SortedDictionary<string, StreamData> _noSortParts;

        #endregion

        #region Construcotrs

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingService" /> class.
        /// </summary>
        /// <param name="fileProvider">The file provider.</param>
        public SortingService(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the default logger.
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Generates the name of the sorted file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <returns>
        /// The name of the sorted file.
        /// </returns>
        public string GenerateSortedFileName(string fileName) =>
            _fileProvider.AddSuffixToFileName(fileName, AppConstants.SortedFileSuffix);

        /// <summary>
        /// Sorts the specified input file.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="outputFileName">Name of the output file.</param>
        public async Task SortAsync(string inputFileName, string outputFileName)
        {
            FileInfo fileInfo = _fileProvider.GetExistingFileInfo(inputFileName);

            _baseDir = fileInfo.DirectoryName;

            if (fileInfo.Length < AppConstants.MaxFilePartSize)
            {
                Logger.LogInformation($"File less than {AppConstants.MaxFilePartSize} bytes");
                await SortFileEntriesAsync(fileInfo.FullName, outputFileName);
            }
            else
            {
                Logger.LogInformation("Sort bigger files");

                _noSortParts = new SortedDictionary<string, StreamData>(AppConstants.Comparer);
                _sortParts = new SortedDictionary<string, StreamData>(AppConstants.Comparer);

                await SplitFileAsync(inputFileName, 1);
                await MergeFilesAsync(outputFileName);
                _fileProvider.DeleteDirectoryRecursively($"{_baseDir}\\{AppConstants.WorkspaceDirectoryName}");
            }
        }

        /// <summary>
        /// Determines whether the sorting is valid.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <returns>
        ///   <c>true</c> if the sorting is valid; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsSotringValid(string fileName)
        {
            StreamData stream = _fileProvider.OpenFileForReading(fileName, AppConstants.TextEncoding);

            bool isValid = true;
            string line;

            string previousLine = null;

            while ((line = await _fileProvider.ReadNextLineAsync(stream)) != null)
            {
                line = line.Trim(AppConstants.BomSymbol);

                if (line != string.Empty && string.Compare(previousLine, line, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    isValid = false;
                    break;
                }

                previousLine = line;
            }

            stream.Dispose();

            return isValid;
        }

        #endregion

        #region Private methods

        private async Task SplitFileAsync(string inputFileName, int chars)
        {
            var sortFiles = new Dictionary<string, StreamData>(AppConstants.Comparer);

            using (StreamData inputStream = _fileProvider.OpenFileForReading(inputFileName, AppConstants.TextEncoding))
            {
                string line;

                while ((line = await _fileProvider.ReadNextLineAsync(inputStream)) != null)
                {
                    if (line.Length < chars)
                    {
                        if (!_noSortParts.TryGetValue(line, out StreamData noSortStream))
                        {
                            string partFileName = GetNextNoSortPartFileName();
                            noSortStream = _fileProvider.OpenFileForReadingAndWriting(partFileName, AppConstants.TextEncoding, true);
                            _noSortParts.Add(line, noSortStream);

                            Logger.LogInformation($"Create no sort file {partFileName}");
                        }

                        await _fileProvider.WriteLineInFileAsync(line, noSortStream);
                    }
                    else
                    {
                        string start = line.Substring(0, chars);

                        if (!sortFiles.TryGetValue(start, out StreamData sortFilePart))
                        {
                            string partFileName = GetNextPartFileName();
                            sortFilePart = _fileProvider.OpenFileForReadingAndWriting(partFileName, AppConstants.TextEncoding, true);
                            sortFiles.Add(start, sortFilePart);

                            Logger.LogInformation($"Create sort file {partFileName}");
                        }

                        await _fileProvider.WriteLineInFileAsync(line, sortFilePart);
                    }
                }
            }

            foreach (var file in sortFiles)
            {
                string fileName = file.Value.FileStreamData.Name;
                long fileSize = file.Value.FileStreamData.Length;

                await _fileProvider.FlushStreamWriterAsync(file.Value);
                file.Value.Dispose();

                if (fileSize > AppConstants.MaxFilePartSize)
                {
                    Logger.LogInformation($"Split part file {fileName}");

                    await SplitFileAsync(fileName, chars + 1);
                    await _fileProvider.DeleteFileIfExistsAsync(fileName);
                }
                else
                {
                    Logger.LogInformation($"Sort part file {fileName}");

                    await SortFileEntriesAsync(fileName, fileName);

                    if (!_sortParts.TryGetValue(file.Key, out StreamData fileStream))
                    {
                        fileStream = _fileProvider.OpenFileForReadingAndWriting(fileName, AppConstants.TextEncoding);
                        _sortParts.Add(file.Key, fileStream);
                    }
                    else
                    {
                        _sortParts[file.Key] = _fileProvider.OpenFileForReadingAndWriting(fileName, AppConstants.TextEncoding);
                    }
                }
            }
        }

        private async Task SortFileEntriesAsync(string inputFileName, string outputFileName)
        {
            List<string> entries = new List<string>();

            using (StreamData inputStream = _fileProvider.OpenFileForReading(inputFileName, AppConstants.TextEncoding))
            {
                string line;

                while ((line = await _fileProvider.ReadNextLineAsync(inputStream)) != null)
                {
                    entries.Add(line);
                }
            }

            entries.Sort(AppConstants.Comparer);

            using (StreamData outputStream = _fileProvider.OpenFileForWriting(outputFileName, AppConstants.TextEncoding, true))
            {
                foreach (string line in entries)
                {
                    await _fileProvider.WriteLineInFileAsync(line, outputStream);
                }

                await _fileProvider.FlushStreamWriterAsync(outputStream);
            }
        }

        private async Task MergeFilesAsync(string outputFileName)
        {
            foreach (KeyValuePair<string, StreamData> file in _noSortParts)
            {
                if (_sortParts.ContainsKey(file.Key))
                {
                    IEnumerable<StreamData> streams = new List<StreamData> { file.Value, _sortParts[file.Key] };
                    string mergedFileName = _fileProvider.AddSuffixToFileName(_sortParts[file.Key].FileStreamData.Name, "merged");

                    await _fileProvider.MergeFilesAsync(streams, mergedFileName, AppConstants.TextEncoding);

                    _sortParts[file.Key].Dispose();
                    _sortParts[file.Key] = _fileProvider.OpenFileForReading(mergedFileName, AppConstants.TextEncoding);
                }
                else
                {
                    _sortParts.Add(file.Key, file.Value);
                }
            }

            Logger.LogInformation($"Merging files.");

            await _fileProvider.MergeFilesAsync(_sortParts.Values, outputFileName, AppConstants.TextEncoding);

            foreach (StreamData stream in _sortParts.Values)
            {
                stream.Dispose();
            }
        }

        private string GetNextNoSortPartFileName() => string.Format(AppConstants.NoSortFileNameFormat, _baseDir, AppConstants.WorkspaceDirectoryName, _noSortFileCount++);

        private string GetNextPartFileName() => string.Format(AppConstants.PartFileNameFormat, _baseDir, AppConstants.WorkspaceDirectoryName, _sortFileCount++);

        #endregion
    }
}
