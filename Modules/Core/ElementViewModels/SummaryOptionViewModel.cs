using System;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class SummaryOptionViewModel : ElementViewModel
    {
        private OptionViewModel _option;

        public SummaryOptionViewModel(OptionViewModel option)
        {
            _option = option;
        }

        public string Name => _option.Control.Label.TrimEnd(':');

        public object Value => GetValue();

        public new bool IsVisible => _option.Control.IsVisible;

        private object GetValue()
        {
            object value = null;

            try
            {
                // All controls have a Value property
                var valueProperty = _option.Control.GetType().GetProperty("Value");
                value = valueProperty?.GetValue(_option.Control);

                if (value is bool)
                {
                    value = ((bool) value) ? "Yes" : "No";
                }
            }
            catch (Exception ex)
            {
                // Log and keep going
                Logger.Log(ex);
            }

            return value;
        }
    }
}
