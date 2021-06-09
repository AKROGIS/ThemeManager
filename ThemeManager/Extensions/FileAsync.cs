using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.Extensions
{
    public class MyFile
    {
        /// <summary>
        /// Asynchronously return the data in filePath as text.
        /// </summary>
        /// <remarks>
        /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/using-async-for-file-access
        /// </remarks>
        /// <param name="filePath">Filesystem path to a text file</param>
        /// <returns></returns>
        public static async Task<string> ReadAllTextAsync(string filePath)
        {
            var sb = new StringBuilder();

            using (var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true))
            {

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Asynchronously check if a file system path is an accessible file
        /// </summary>
        /// <remarks>
        /// Useful when checking for paths that might be slow to respond (on a network or external HD)
        /// See discussion on Stack Overflow: https://stackoverflow.com/questions/19076652/check-if-a-file-exists-async
        /// </remarks>
        /// <param name="path">Filesystem path to a file</param>
        /// <returns></returns>
        public static async Task<bool> ExistsAsync(string path)
        {
            return await Task.Run(() => File.Exists(path));
        }

    }

    public class MyDirectory
    {
        /// <summary>
        /// Asynchronously check if a file system path is an accessible directory
        /// </summary>
        /// <remarks>
        /// Useful when checking for paths that might be slow to respond (on a network or external HD)
        /// See discussion on Stack Overflow: https://stackoverflow.com/questions/19076652/check-if-a-file-exists-async
        /// </remarks>
        /// <param name="path">Filesystem path to a directory</param>
        /// <returns></returns>
        public static async Task<bool> ExistsAsync(string path)
        {
            return await Task.Run(() => Directory.Exists(path));
        }

    }

}

