using System;
using System.IO;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    internal class MoveFilesCommand : TaskCommand
    {
        public MoveFilesCommand(MoveFilesTask task) 
            : base(task)
        {
            From = task.FromPath;
            To = task.ToPath;
            Overwrite = task.Overwrite;
        }

        public string From { get; set; }

        public string To { get; set; }

        public bool Overwrite { get; }

        public override Task Execute()
        {
            return Task.Run(async () => await MoveFiles());
        }

        private async Task MoveFiles()
        {
            try
            {
                var to = To;
                var from = From;

                Events.GetEvent<DebugOutputEvent>()
                    .Publish($"> ixconfig move -From \"{from}\" -To \"{to}\" -Overwrite {Overwrite}\n");

                from = Variables.ExpandVariables(from);
                from = Environment.ExpandEnvironmentVariables(from);

                to = Variables.ExpandVariables(to);
                to = Environment.ExpandEnvironmentVariables(to);

                Events.GetEvent<DebugOutputEvent>().Publish($"from => {from}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"to => {to}\n");

                var installPath = (string)Variables[WellKnownVariableNames.InstallPath];
                var tempPath = (string)Variables[WellKnownVariableNames.TempPath];

                if (to.IndexOf(installPath, StringComparison.OrdinalIgnoreCase) != 0 &&
                    to.IndexOf(tempPath, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw new InvalidOperationException($"Security risk moving files to location other than install path: {to}");
                }

                int filesMoved;

                if (Directory.Exists(from))
                {
                    filesMoved = await FileService.MoveDirectoryAsync(from, to, Overwrite);
                }
                else if (File.Exists(from))
                {
                    await FileService.MoveFileAsync(from, to, Overwrite);
                    filesMoved = 1;
                }
                else
                {
                    throw new FileNotFoundException($"Directory/file does not exist: {from}");
                }

                Events.GetEvent<DebugOutputEvent>().Publish($"({filesMoved}) files moved\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
