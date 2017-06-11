using System;
using System.IO.Compression;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class ZipFilesCommand : TaskCommand
    {
        public ZipFilesCommand(ZipFilesTask task) 
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
            return Task.Run(async () => await ZipFiles());
        }

        private async Task ZipFiles()
        {
            try
            {
                var file = File;
                var path = Path;

                Events.GetEvent<DebugOutputEvent>()
                    .Publish($"> ixconfig zip -Path \"{path}\" -File \"{file}\"\n");

                path = Variables.ExpandVariables(path);
                path = Environment.ExpandEnvironmentVariables(path);

                file = Variables.ExpandVariables(file);
                file = Environment.ExpandEnvironmentVariables(file);

                var installPath = (string)Variables[WellKnownVariableNames.InstallPath];
                var tempPath = (string)Variables[WellKnownVariableNames.TempPath];

                if (file.IndexOf(installPath, StringComparison.OrdinalIgnoreCase) != 0 &&
                    file.IndexOf(tempPath, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw new InvalidOperationException($"Security risk compressing files to location other than install or temp path: {file}");
                }

                if (System.IO.Directory.Exists(file))
                {
                    throw new InvalidOperationException($"Existing destination file cannot be a directory: {file}");
                }

                if (System.IO.File.Exists(file))
                {
                    if (!Overwrite)
                    {
                        throw new InvalidOperationException($"File already exists: {file}");
                    }

                    await FileService.DeleteFileAsync(file);
                }

                ZipFile.CreateFromDirectory(path, file, CompressionLevel.Optimal, false);

                Events.GetEvent<DebugOutputEvent>().Publish($"{path} compressed to {file}\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
