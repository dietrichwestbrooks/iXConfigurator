using System;
using System.IO.Compression;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class UnzipFilesCommand : TaskCommand
    {
        public UnzipFilesCommand(UnzipFilesTask task) 
            : base(task)
        {
            Path = task.Path;
            File = task.File;
            Overwrite = task.Overwrite;
        }

        public string Path { get; set; }

        public string File { get; set; }

        public bool Overwrite { get; set; }

        public override Task Execute()
        {
            return Task.Run(async () => await UnzipFiles());
        }

        private async Task UnzipFiles()
        {
            try
            {
                var path = Path;
                var file = File;

                Events.GetEvent<DebugOutputEvent>()
                    .Publish($"> ixconfig unzip -File \"{file}\" -Path \"{path}\"\n");

                path = Variables.ExpandVariables(path);
                path = Environment.ExpandEnvironmentVariables(path);

                file = Variables.ExpandVariables(file);
                file = Environment.ExpandEnvironmentVariables(file);

                var installPath = (string)Variables[WellKnownVariableNames.InstallPath];
                var tempPath = (string)Variables[WellKnownVariableNames.TempPath];

                if (path.IndexOf(installPath, StringComparison.OrdinalIgnoreCase) != 0 &&
                    path.IndexOf(tempPath, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw new InvalidOperationException($"Security risk extracting files to location other than install or temp path: {path}");
                }

                if (System.IO.File.Exists(path))
                {
                    throw new InvalidOperationException($"Existing destination path cannot be a file: {path}");
                }

                if (System.IO.Directory.Exists(path))
                {
                    if (!Overwrite)
                    {
                        throw new InvalidOperationException($"Path already exists: {path}");
                    }

                    await FileService.DeleteDirectoryAsync(path, true);
                }

                ZipFile.ExtractToDirectory(file, path);

                Events.GetEvent<DebugOutputEvent>().Publish($"{file} extracted to {path}\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
