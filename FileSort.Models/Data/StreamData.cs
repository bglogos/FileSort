using System;
using System.IO;

namespace FileSort.Models.Data
{
    /// <summary>
    /// Contains the srteam reader and writer objects.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class StreamData : IDisposable
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the file stream data.
        /// </summary>
        /// <value>
        /// The file stream data.
        /// </value>
        public FileStream FileStreamData { get; set; }

        /// <summary>
        /// Gets or sets the buffered stream data.
        /// </summary>
        /// <value>
        /// The buffered stream data.
        /// </value>
        public BufferedStream BufferedStreamData { get; set; }

        /// <summary>
        /// Gets or sets the stream writer data.
        /// </summary>
        /// <value>
        /// The stream writer data.
        /// </value>
        public StreamWriter StreamWriterData { get; set; }

        /// <summary>
        /// Gets or sets the stream reader data.
        /// </summary>
        /// <value>
        /// The stream reader data.
        /// </value>
        public StreamReader StreamReaderData { get; set; }

        #endregion

        #region IDisposable methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (StreamReaderData != null)
            {
                StreamReaderData.Dispose();
            }
            else if (StreamWriterData != null)
            {
                StreamWriterData.Dispose();
            }

            if (BufferedStreamData != null)
            {
                BufferedStreamData.Dispose();
            }

            if (FileStreamData != null)
            {
                FileStreamData.Dispose();
            }
        }

        #endregion
    }
}
