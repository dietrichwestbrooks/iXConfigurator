using System;
using Microsoft.Practices.Unity;
using Prism.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public abstract class TaskCommand : ITaskCommand
    {
        protected TaskCommand(string condition)
        {
            Condition = condition;
        }

        protected TaskCommand(Template.Task task)
        {
            Condition = task.Condition;
        }

        [Dependency]
        protected IApplicationLogger Logger { get; set; }

        [Dependency]
        protected IEventAggregator Events { get; set; }

        [Dependency]
        protected IVariableStore Variables { get; set; }

        [Dependency]
        protected IFileService FileService { get; set; }

        public string Condition { get; }

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public virtual bool CanExecute()
        {
            if (string.IsNullOrWhiteSpace(Condition))
                return true;

            bool value;

            if (!Variables.TryEvaluateExpression(Condition, out value))
            {
                return false;
            }

            return value;
        }

        public abstract Task Execute();

        public void Execute(object parameter)
        {
            Execute();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
