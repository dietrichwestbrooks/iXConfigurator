using Prism.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class OutputViewModel : ViewModelBase
    {
        private string _text;

        public OutputViewModel(IEventAggregator events)
        {
            Title = "Output";

            events.GetEvent<DebugOutputEvent>().Subscribe(text => Text += $"{text}");
            events.GetEvent<TemplateSelectedEvent>().Subscribe(key => Text = string.Empty);
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
    }
}