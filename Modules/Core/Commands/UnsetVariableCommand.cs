using System;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class UnsetVariableCommand : TaskCommand
    {
        public UnsetVariableCommand(UnsetVariableTask task) 
            : base(task)
        {
            Key = task.Key;
        }

        public string Key { get; set; }

        public override Task Execute()
        {
            try
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"> ixconfig unset -Key {Key}\n");

                if (Key == WellKnownVariableNames.InstallPath || 
                    Key == WellKnownVariableNames.InstallDrive ||
                    Key == WellKnownVariableNames.LibraryPath ||
                    Key == WellKnownVariableNames.TempPath)
                {
                    throw new InvalidOperationException($"Attempt to remove reserved variable: {Key}");
                }

                Variables.RemoveVariable(Key);

                Events.GetEvent<DebugOutputEvent>().Publish($"Removed {Key}\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }

            return Task.Delay(0);
        }
    }
}
