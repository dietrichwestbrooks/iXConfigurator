using System;
using System.Net;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class DownloadFileCommand : TaskCommand
    {
        public DownloadFileCommand(DownloadFileTask task) 
            : base(task)
        {
            Url = task.Url;
            File = task.File;
            Overwrite = task.Overwrite;
        }

        public string Url { get; set; }

        public string File { get; set; }

        public bool Overwrite { get; }

        public override Task Execute()
        {
            return Task.Run(async () => await DownloadFile());
        }

        private async Task DownloadFile()
        {
            try
            {
                var url = Url;
                var file = File;

                Events.GetEvent<DebugOutputEvent>().Publish($"> ixconfig download -Url \"{url}\" -File \"{file}\" -Overwrite {Overwrite}\n");

                file = Variables.ExpandVariables(file);
                file = Environment.ExpandEnvironmentVariables(file);

                url = Variables.ExpandVariables(url);

                Events.GetEvent<DebugOutputEvent>().Publish($"url => {url}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"path => {file}\n");

                var installPath = (string)Variables[WellKnownVariableNames.InstallPath];
                var tempPath = (string)Variables[WellKnownVariableNames.TempPath];

                if (file.IndexOf(installPath, StringComparison.OrdinalIgnoreCase) != 0 &&
                    file.IndexOf(tempPath, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw new InvalidOperationException($"Security risk copying files to location other than install path: {file}");
                }

                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(url, file);
                }

                Events.GetEvent<DebugOutputEvent>().Publish("File downloaded\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
