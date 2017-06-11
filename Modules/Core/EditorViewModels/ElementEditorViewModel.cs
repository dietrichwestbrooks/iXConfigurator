using System;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class ElementEditorViewModel : SnippetEditorViewModelBase
    {
        private string _visible;

        public string Visible
        {
            get { return _visible; }
            set { SetProperty(ref _visible, value?.Trim(), () => OnVisibleChanged(value?.Trim())); }
        }

        private void OnVisibleChanged(string visible)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<BooleanExpressionEnum>(visible) == BooleanExpressionEnum.Build)
                {
                    var expression = OpenExpressionBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                        new Action(() => Visible = expression));
                }
            }
            catch (InvalidOperationException)
            {
                // Do Nothing
            }
        }

        protected string GetVisible()
        {
            return !string.IsNullOrWhiteSpace(Visible) ? $"visible=\"{BooleanToLower(Visible)}\"" : string.Empty;
        }
    }
}
