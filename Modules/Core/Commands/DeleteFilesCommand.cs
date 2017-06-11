using System;
using System.IO;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class DeleteFilesCommand : TaskCommand
    {
        public DeleteFilesCommand(DeleteFilesTask task) 
            : base(task)
        {
            Path = task.Path;
            Override = false;
        }

        public DeleteFilesCommand(string path, bool @override = false) 
            : base(string.Empty)
        {
            Path = path;
            Override = @override;
        }

        public string Path { get; set; }

        public bool Override { get; }

        public override Task Execute()
        {
            return Task.Run(async () => await DeleteFiles());
        }

        private async Task DeleteFiles()
        {
            try
            {
                var path = Path;

                Events.GetEvent<DebugOutputEvent>().Publish($"> ixconfig delete -Path \"{path}\"\n");

                path = Variables.ExpandVariables(path);
                path = Environment.ExpandEnvironmentVariables(path);

                Events.GetEvent<DebugOutputEvent>().Publish($"path => {path}\n");

                var installPath = (string)Variables[WellKnownVariableNames.InstallPath];
                var tempPath = (string)Variables[WellKnownVariableNames.TempPath];

                if (!Override && path.TrimEnd('\\') == installPath.TrimEnd('\\'))
                {
                    throw new InvalidOperationException("Cannot delete install path");
                }

                if (!Override && path.TrimEnd('\\') == tempPath.TrimEnd('\\'))
                {
                    throw new InvalidOperationException("Cannot delete temp path");
                }

                if (!Override && (path.IndexOf(installPath, StringComparison.OrdinalIgnoreCase) != 0 &&
                    path.IndexOf(tempPath, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    throw new InvalidOperationException(
                        $"Security risk deleting files from location other than install or temp path: {path}");
                }

                int filesDeleted;

                if (Directory.Exists(path))
                {
                    filesDeleted = await FileService.DeleteDirectoryAsync(path);
                }
                else if (File.Exists(path))
                {
                    await FileService.DeleteFileAsync(path);
                    filesDeleted = 1;
                }
                else
                {
                    throw new FileNotFoundException($"Directory/file does not exist: {path}");
                }

                Events.GetEvent<DebugOutputEvent>().Publish($"({filesDeleted}) files deleted\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
