using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public abstract class BuilderViewModelBase : PopupViewModelBase, IAcceptNavigationParameters
    {
        private IApplicationLogger _logger;
        private ITemplateManager _templateManager;

        protected BuilderViewModelBase(IApplicationLogger logger, ITemplateManager templateManager)
        {
            _logger = logger;
            _templateManager = templateManager;
        }

        public ObservableCollection<string> ConfiguratorVariables { get; } = new ObservableCollection<string>();

        protected StringBuilder ResultBuilder { get; set; } = new StringBuilder();

        private async Task BuildVariablesAsync()
        {
            try
            {
                ConfiguratorVariables.Clear();

                ConfiguratorVariables.Add("libraryPath");
                ConfiguratorVariables.Add("tempPath");
                ConfiguratorVariables.Add("installPath");

                var template = await _templateManager.GetTemplateAsync(_templateManager.CurrentKey);

                foreach (var option in template.Pages
                    .SelectMany(page => page.Sections)
                    .SelectMany(section => section.Options)
                    .Concat(template.Pages.SelectMany(page => page.Options)))
                {
                    if (!ConfiguratorVariables.Contains(option.Key))
                    {
                        ConfiguratorVariables.Add(option.Key);
                    }

                    if (option.Control is CheckControl)
                    {
                        var check = option.Control as CheckControl;

                        foreach (var key in check.Checked.Options
                            .Concat(check.Unchecked.Options)
                            .Select(opt => opt.Key))
                        {
                            if (!ConfiguratorVariables.Contains(key))
                            {
                                ConfiguratorVariables.Add(key);
                            }
                        }

                        foreach (var key in check.Checked.Tasks
                            .Concat(check.Unchecked.Tasks)
                            .OfType<SetVariableTask>()
                            .Select(task => task.Key))
                        {
                            if (!ConfiguratorVariables.Contains(key))
                            {
                                ConfiguratorVariables.Add(key);
                            }
                        }
                    }
                    else if (option.Control is ComboControl)
                    {
                        var combo = option.Control as ComboControl;

                        foreach (var key in combo.Items
                            .SelectMany(item => item.Tasks)
                            .OfType<SetVariableTask>()
                            .Select(task => task.Key))
                        {
                            if (!ConfiguratorVariables.Contains(key))
                            {
                                ConfiguratorVariables.Add(key);
                            }
                        }
                    }
                    else if (option.Control is ListControl)
                    {
                        var list = option.Control as ListControl;

                        foreach (var key in list.Items
                            .SelectMany(item => item.Tasks)
                            .OfType<SetVariableTask>()
                            .Select(task => task.Key))
                        {
                            if (!ConfiguratorVariables.Contains(key))
                            {
                                ConfiguratorVariables.Add(key);
                            }
                        }
                    }
                    else if (option.Control is RadioControl)
                    {
                        var radio = option.Control as RadioControl;

                        foreach (var key in radio.Choices
                            .SelectMany(choice => choice.Tasks)
                            .OfType<SetVariableTask>()
                            .Select(task => task.Key))
                        {
                            if (!ConfiguratorVariables.Contains(key))
                            {
                                ConfiguratorVariables.Add(key);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        public virtual async void AcceptNavigationParameters(NavigationParameters navigationParameters)
        {
            if (navigationParameters.Any(parameter => parameter.Key == "Result"))
            {
                ResultBuilder = navigationParameters["Result"] as StringBuilder;
            }

            await BuildVariablesAsync();
        }
    }
}
