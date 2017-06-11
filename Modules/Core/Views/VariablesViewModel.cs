using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class VariablesViewModel : ViewModelBase
    {
        public VariablesViewModel()
        {
            Title = "Variables";

            var variables = ServiceLocator.Current.GetInstance<IVariableStore>();

            foreach (var key in variables.Select(variable => variable.Key))
            {
                Variables.Add(new VariableViewModel(key));
            }

            variables.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var key in args.NewItems.Cast<string>())
                    {
                        Variables.Add(new VariableViewModel(key));
                    }
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var key in args.OldItems.Cast<string>())
                    {
                        var viewModel = Variables.FirstOrDefault(vm => vm.Key == key);

                        if (viewModel != null)
                        {
                            Variables.Remove(viewModel);
                        }
                    }
                }
            };
        }

        public ObservableCollection<VariableViewModel> Variables { get; } = new ObservableCollection<VariableViewModel>();
    }
}
