using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace IgniteView.Core
{
    public abstract class FileResolver
    {
        /// <summary>
        /// Gets the relative path of the index file (eg. "/index.html")
        /// </summary>
        public abstract string GetIndexFile();

        /// <summary>
        /// Checks whether a file exists
        /// </summary>
        /// <param name="fileRelativeToRoot">The path of the file, relative to the URL root (eg. /index.html)</param>
        /// <returns>Whether the file exists</returns>
        public abstract bool DoesFileExist(string fileRelativeToRoot);

        /// <summary>
        /// Opens a file stream for reading
        /// </summary>
        /// <param name="fileRelativeToRoot">The path of the file, relative to the URL root (eg. /index.html)</param>
        /// <returns>A seekable stream</returns>
        public abstract Stream OpenFileStream(string fileRelativeToRoot);

        /// <summary>
        /// Reads an entire file as a string
        /// </summary>
        /// <param name="fileRelativeToRoot">The path of the file, relative to the URL root (eg. /index.html)</param>
        /// <returns>The contents of the file</returns>
        public virtual string ReadFileAsText(string fileRelativeToRoot)
        {
            var fileStream = OpenFileStream(fileRelativeToRoot);
            var reader = new StreamReader(fileStream);
            var fileContent = reader.ReadToEnd();
            fileStream.Dispose();

            return fileContent;
        }
    }
}
