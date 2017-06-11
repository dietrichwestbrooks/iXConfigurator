using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm
{
    public class InteractiveCommand : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            if (AssociatedObject != null)
            {
                var command = ResolveCommand();

                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }

        private ICommand ResolveCommand()
        {
            ICommand command = null;

            if (Command != null)
            {
                return Command;
            }

            //if (AssociatedObject == null)
            //{
            //    return null;
            //}

            //foreach (var info in AssociatedObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    if (typeof (ICommand).IsAssignableFrom(info.PropertyType) &&
            //        string.Equals(info.Name, CommandName, StringComparison.Ordinal))
            //    {
            //        command = (ICommand) info.GetValue(AssociatedObject, null);
            //    }
            //}

            return command;
        }

        private string _commandName;
        public string CommandName
        {
            get
            {
                ReadPreamble();
                return _commandName;
            }
            set
            {
                if (_commandName != value)
                {
                    WritePreamble();
                    _commandName = value;
                    WritePostscript();
                }
            }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InteractiveCommand), new UIPropertyMetadata(null));
    }
}
