using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class ExpandableBuilderViewModel : BuilderViewModelBase
    {
        private string _result;
        private bool _showEnvironment;

        public ExpandableBuilderViewModel(IApplicationLogger logger, ITemplateManager templateManager) 
            : base(logger, templateManager)
        {
            Title = "Expandable Builder";

            EnvironmentVariables.AddRange(Environment.GetEnvironmentVariables().Keys.OfType<string>());
        }

        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value, () => OnResultChanged(value)); }
        }

        public bool ShowEnvironment
        {
            get { return _showEnvironment; }
            set { SetProperty(ref _showEnvironment, value); }
        }

        public ObservableCollection<string> EnvironmentVariables { get; } = new ObservableCollection<string>();

        private void OnResultChanged(string result)
        {
            ResultBuilder.Clear();
            ResultBuilder.Append(result);
        }

        public override void AcceptNavigationParameters(NavigationParameters navigationParameters)
        {
            base.AcceptNavigationParameters(navigationParameters);

            if (navigationParameters.Any(parameter => parameter.Key == "Environment"))
            {
                ShowEnvironment = (bool)navigationParameters["Environment"];
            }
        }
    }
}
