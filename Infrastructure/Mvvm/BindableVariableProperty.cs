using System;
using System.ComponentModel;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm
{
    public class BindableVariableProperty<TValue> : BindableVariableObject<TValue>
    {
        public BindableVariableProperty(INotifyPropertyChanged container, string propertyName)
            : base(container, propertyName)
        {
        }

        public override void Bind(string expression)
        {
            base.Bind(expression);

            if (Keys.Count() > 1)
            {
                // Should be 0 (static value) or 1 key for a bindable preperty
                // Log and keep going
                Logger.Log(new InvalidOperationException($"More than one variable was specified for a bindable property {expression}"));
            }
        }

        protected override void SetVariableValue(TValue value)
        {
            // Grab what should be the one and only key
            var key = Keys.First();

            // Set variable value
            Variables.SetVariableValue(key, value);
        }

        protected override TValue GetVariableValue()
        {
            TValue value;

            // Grab what should be the one and only key
            var key = Keys.First();

            if (!Variables.TryGetVariableValue(key, out value))
            {
                // Log and keep going returning default value
                Logger.Log(new InvalidOperationException($"Getting variable value: {key}"));
            }

            return value;
        }
    }
}
