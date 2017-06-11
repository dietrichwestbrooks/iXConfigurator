using System;
using Microsoft.Practices.Unity;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class TaskCommandFactory : ITaskCommandFactory
    {
        private IUnityContainer _container;

        private const string Task = "Task";
        private const string Delete = "Delete";
        private const string Copy = "Copy";

        public TaskCommandFactory(IUnityContainer container)
        {
            _container = container;

            container.RegisterType<CopyFilesCommand>(Task, new InjectionConstructor(
                typeof(CopyFilesTask)));

            container.RegisterType<CopyFilesCommand>(Copy, new InjectionConstructor(
                typeof (string),
                typeof (string),
                typeof (bool),
                typeof (bool)));

            container.RegisterType<DeleteFilesCommand>(Task, new InjectionConstructor(
                typeof(DeleteFilesTask)));

            container.RegisterType<DeleteFilesCommand>(Delete, new InjectionConstructor(
                typeof (string),
                typeof (bool)));
        }

        public TaskCommand CreateCommand(Task task)
        {
            TaskCommand newCommand;

            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task is SetVariableTask)
            {
                newCommand = _container.Resolve<SetVariableCommand>(
                    new DependencyOverride(typeof(SetVariableTask), task));
            }
            else if (task is UnsetVariableTask)
            {
                newCommand = _container.Resolve<UnsetVariableCommand>(
                    new DependencyOverride(typeof(UnsetVariableTask), task));
            }
            else if (task is CopyFilesTask)
            {
                newCommand = _container.Resolve<CopyFilesCommand>(Task,
                    new DependencyOverride(typeof(CopyFilesTask), task));
            }
            else if (task is DeleteFilesTask)
            {
                newCommand = _container.Resolve<DeleteFilesCommand>(Task,
                    new DependencyOverride(typeof(DeleteFilesTask), task));
            }
            else if (task is DownloadFileTask)
            {
                newCommand = _container.Resolve<DownloadFileCommand>(
                    new DependencyOverride(typeof(DownloadFileTask), task));
            }
            else if (task is MoveFilesTask)
            {
                newCommand = _container.Resolve<MoveFilesCommand>(
                    new DependencyOverride(typeof(MoveFilesTask), task));
            }
            else if (task is ReadXmlTask)
            {
                newCommand = _container.Resolve<ReadXmlCommand>(
                    new DependencyOverride(typeof(ReadXmlTask), task));
            }
            else if (task is WriteXmlTask)
            {
                newCommand = _container.Resolve<WriteXmlCommand>(
                    new DependencyOverride(typeof(WriteXmlTask), task));
            }
            else if (task is WriteXmlTableTask)
            {
                newCommand = _container.Resolve<WriteXmlTableCommand>(
                    new DependencyOverride(typeof(WriteXmlTableTask), task));
            }
            else if (task is ZipFilesTask)
            {
                newCommand = _container.Resolve<ZipFilesCommand>(
                    new DependencyOverride(typeof(ZipFilesTask), task));
            }
            else if (task is UnzipFilesTask)
            {
                newCommand = _container.Resolve<UnzipFilesCommand>(
                    new DependencyOverride(typeof(UnzipFilesTask), task));
            }
            else
            {
                throw new ArgumentOutOfRangeException(task.GetType().Name);
            }

            return newCommand;
        }

        public TaskCommand CreateCopyCommand(string from, string to, bool overwrite, bool @override = false)
        {
            return _container.Resolve<CopyFilesCommand>(Copy,
                new ParameterOverride(nameof(from), from),
                new ParameterOverride(nameof(to), to),
                new ParameterOverride(nameof(overwrite), overwrite),
                new ParameterOverride(nameof(@override), @override));
        }

        public TaskCommand CreateDeleteCommand(string path, bool @override = false)
        {
            return _container.Resolve<DeleteFilesCommand>(Delete,
                new ParameterOverride(nameof(path), path),
                new ParameterOverride(nameof(@override), @override));
        }
    }
}
