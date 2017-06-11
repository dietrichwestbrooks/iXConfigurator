using System;
using System.IO;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    internal class CopyFilesCommand : TaskCommand
    {
        public CopyFilesCommand(CopyFilesTask task)
            : base(task)
        {
            From = task.From;
            To = task.To;
            Overwrite = task.Overwrite;
            Override = false;
        }

        public CopyFilesCommand(string from, string to, bool overwrite, bool @override = false)
            : base (string.Empty)
        {
            From = from;
            To = to;
            Overwrite = overwrite;
            Override = @override;
        }

        public string From { get; }

        public string To { get; }

        public bool Overwrite { get; }

        public bool Override { get; }

        public override Task Execute()
        {
            return Task.Run(async () => await CopyFiles());
        }

        private async Task CopyFiles()
        {
            try
            {
                var to = To;
                var from = From;

                Events.GetEvent<DebugOutputEvent>().Publish($"> ixconfig copy -From \"{from}\" -To \"{to}\" -Overwrite {Overwrite}\n");

                from = Variables.ExpandVariables(from);
                from = Environment.ExpandEnvironmentVariables(from);

                to = Variables.ExpandVariables(to);
                to = Environment.ExpandEnvironmentVariables(to);

                Events.GetEvent<DebugOutputEvent>().Publish($"from => {from}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"to => {to}\n");

                var installPath = (string)Variables[WellKnownVariableNames.InstallPath];
                var tempPath = (string)Variables[WellKnownVariableNames.TempPath];

                if (!Override && (to.IndexOf(installPath, StringComparison.OrdinalIgnoreCase) != 0 &&
                    to.IndexOf(tempPath, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    throw new InvalidOperationException($"Security risk copying files to location other than install or temp path: {to}");
                }

                int filesCopied;

                if (Directory.Exists(from))
                {
                    filesCopied = await FileService.CopyDirectoryAsync(from, to, Overwrite);
                }
                else if (File.Exists(from))
                {
                    await FileService.CopyFileAsync(from, to, Overwrite);
                    filesCopied = 1;
                }
                else
                {
                    throw new FileNotFoundException($"Directory/file does not exist: {from}");
                }

                Events.GetEvent<DebugOutputEvent>().Publish($"({filesCopied}) files copied\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
