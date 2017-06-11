using System;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class SetVariableCommand : TaskCommand
    {
        public SetVariableCommand(SetVariableTask task)
            : base(task)
        {
            Key = task.Key;
            Value = task.Value;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public override Task Execute()
        {
            string value = Value;

            try
            {
                Events.GetEvent<DebugOutputEvent>()
                    .Publish($"> ixconfig set -Key {Key} -Value \"{value}\"\n");

                value = Variables.ExpandVariables(value);

                Events.GetEvent<DebugOutputEvent>().Publish($"value => {value}\n");

                if (Key == WellKnownVariableNames.InstallPath || 
                    Key == WellKnownVariableNames.InstallDrive ||
                    Key == WellKnownVariableNames.LibraryPath ||
                    Key == WellKnownVariableNames.TempPath)
                {
                    throw new InvalidOperationException($"Attempt to set reserved variable: {Key}");
                }

                Events.GetEvent<DebugOutputEvent>().Publish($"{Key}={value}\n\n");

                Variables[Key] = value;
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
