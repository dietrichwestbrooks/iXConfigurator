using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public interface ITaskCommandFactory
    {
        TaskCommand CreateCommand(Task task);

        TaskCommand CreateCopyCommand(string from, string to, bool overwrite, bool @override = false);

        TaskCommand CreateDeleteCommand(string path, bool @override = false);
    }
}
