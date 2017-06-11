using System;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public abstract class TaskEditorViewModel : SnippetEditorViewModelBase
    {
        private string _condition;

        public string Condition
        {
            get { return _condition; }
            set { SetProperty(ref _condition, value?.Trim(), () => OnConditionChanged(value?.Trim())); }
        }

        private void OnConditionChanged(string condition)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<BooleanExpressionEnum>(condition) == BooleanExpressionEnum.Build)
                {
                    var expression = OpenExpressionBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Condition = expression));
                }
            }
            catch (InvalidOperationException)
            {
                // Do Nothing
            }
        }

        protected string GetCondition()
        {
            return !string.IsNullOrWhiteSpace(Condition) ? $"condition=\"{BooleanToLower(Condition)}\"" : string.Empty;
        }
    }
}
