using System;
using System.IO;
using System.Threading.Tasks;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IFileService
    {
        Task<string> ReadTextFileAsync(string fileName, bool create = false);

        Task<FileStream> OpenFileAsync(string fileName, bool create = false, bool readOnly = true, bool shareRead = true, bool shareWrite = true);

        Task SaveTextFileAsync(string fileName, string text);

        Task DeleteFileAsync(string fileName);

        Task<int> DeleteDirectoryAsync(string dirName, bool preserve = false);

        Task CopyFileAsync(string sourceFileName, string destFileName, bool overwrite = false);

        Task<int> CopyDirectoryAsync(string sourceDirName, string destDirName, bool overwrite = false, bool recursive = true);

        Task MoveFileAsync(string sourceFileName, string destFileName, bool overwrite = false);

        Task<int> MoveDirectoryAsync(string sourceDirName, string destDirName, bool overwrite = false, bool recursive = true);

        Task<DateTime> GetModifiedTimeAsync(string fileName);

        Task CreateDirectoryAsync(string dirName, bool createNew = false);
    }
}
