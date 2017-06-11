using System;
using System.IO;
using System.Threading.Tasks;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Properties;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class FileService : IFileService
    {
        public async Task<string> ReadTextFileAsync(string fileName, bool create = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                if (!create)
                {
                    throw new FileNotFoundException(fileName);
                }

                await SaveTextFileAsync(fileName, string.Empty);
            }

            return File.ReadAllText(fileName);
        }

        public Task<FileStream> OpenFileAsync(string fileName, bool create = false, bool readOnly = true, bool shareRead = true, bool shareWrite = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var mode = FileMode.Open;

            if (create)
            {
                mode = FileMode.Create;
            }

            var access = FileAccess.ReadWrite;

            if (readOnly)
            {
                access = FileAccess.Read;
            }

            var share = FileShare.None;

            if (shareRead)
            {
                share |= FileShare.Read;
            }

            if (shareWrite)
            {
                share |= FileShare.Write;
            }

            return Task.FromResult(File.Open(fileName, mode, access, share));
        }

        public Task SaveTextFileAsync(string fileName, string text)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var dirName = Path.GetDirectoryName(fileName);

            if (dirName == null)
            {
                throw new ArgumentException(Resources.InvalidDirectoryPath, nameof(fileName));
            }

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            File.WriteAllText(fileName, text ?? string.Empty);
            return Task.Delay(0);
        }

        public Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (File.Exists(fileName))
                File.Delete(fileName);

            return Task.Delay(0);
        }

        public async Task<int> CopyDirectoryAsync(string sourceDirName, string destDirName, bool overwrite = false,
            bool recursive = true)
        {
            if (string.IsNullOrWhiteSpace(sourceDirName))
            {
                throw new ArgumentNullException(nameof(sourceDirName));
            }

            if (string.IsNullOrWhiteSpace(destDirName))
            {
                throw new ArgumentNullException(nameof(destDirName));
            }

            return await Task.Run(() => CopyDirectory(sourceDirName, destDirName, overwrite, recursive));
        }

        private static int CopyDirectory(string sourceDirName, string destDirName, bool overwrite, bool recursive)
        {
            var filesCopied = 0;

            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, overwrite);
                filesCopied++;
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!recursive)
                return filesCopied;

            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                filesCopied += CopyDirectory(subdir.FullName, temppath, overwrite, true);
            }

            return filesCopied;
        }

        public async Task MoveFileAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName));
            }

            if (string.IsNullOrWhiteSpace(destFileName))
            {
                throw new ArgumentNullException(nameof(destFileName));
            }

            await CopyFileAsync(sourceFileName, destFileName, overwrite);
            await DeleteFileAsync(sourceFileName);
        }

        public async Task<int> MoveDirectoryAsync(string sourceDirName, string destDirName, bool overwrite = false, bool recursive = true)
        {
            if (string.IsNullOrWhiteSpace(sourceDirName))
            {
                throw new ArgumentNullException(nameof(sourceDirName));
            }

            if (string.IsNullOrWhiteSpace(destDirName))
            {
                throw new ArgumentNullException(nameof(destDirName));
            }

            var filesMoved = await CopyDirectoryAsync(sourceDirName, destDirName, overwrite);
            await DeleteDirectoryAsync(sourceDirName);

            return filesMoved;
        }

        public Task<DateTime> GetModifiedTimeAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            return Task.FromResult(File.GetLastWriteTime(fileName));
        }

        public async Task CreateDirectoryAsync(string dirName, bool createNew = false)
        {
            if (string.IsNullOrWhiteSpace(dirName))
            {
                throw new ArgumentNullException(nameof(dirName));
            }

            if (Directory.Exists(dirName))
            {
                if (!createNew)
                {
                    throw new InvalidOperationException("Directory already exist");
                }

                await DeleteDirectoryAsync(dirName, true);

                return;
            }

            Directory.CreateDirectory(dirName);
       }

        public async Task CopyFileAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName));
            }

            if (string.IsNullOrWhiteSpace(destFileName))
            {
                throw new ArgumentNullException(nameof(destFileName));
            }

            var dirName = Path.GetDirectoryName(destFileName);

            if (string.IsNullOrWhiteSpace(dirName))
            {
                throw new ArgumentException(string.Format(Resources.InvalidPath, destFileName), nameof(destFileName));
            }

            if (!Directory.Exists(dirName))
            {
                await CreateDirectoryAsync(dirName);
            }

            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public async Task<int> DeleteDirectoryAsync(string dirName, bool preserve = false)
        {
            if (string.IsNullOrWhiteSpace(dirName))
            {
                throw new ArgumentNullException(nameof(dirName));
            }

            return await Task.Run(() => DeleteDirectory(dirName, preserve));
        }

        private static int DeleteDirectory(string dirName, bool preserve = false)
        { 
            var filesDeleted = 0;

            foreach (var directory in Directory.EnumerateDirectories(dirName))
            {
                filesDeleted += DeleteDirectory(directory);
            }

            foreach (var file in Directory.EnumerateFiles(dirName))
            {
                File.Delete(file);
                filesDeleted += 1;
            }

            if (Path.GetPathRoot(dirName) != dirName && !preserve)
            {
                Directory.Delete(dirName);
            }

            return filesDeleted;
        }
    }
}
