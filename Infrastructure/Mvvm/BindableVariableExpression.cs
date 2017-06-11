using System;
using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm
{
    public class BindableVariableExpression<TValue> : BindableVariableObject<TValue>
    {
        public BindableVariableExpression(INotifyPropertyChanged container, string propertyName)
            : base(container, propertyName)
        {
        }

        protected override void SetVariableValue(TValue value)
        {
            // Cannot set the value of an expression
        }

        protected override TValue GetVariableValue()
        {
            TValue value;

            if (!Variables.TryEvaluateExpression(Expression, out value))
            {
                // Log and keep going returning default value
                Logger.Log(new InvalidOperationException($"Evaluating expression: {Expression}"));
            }

            return value;
        }
    }
}
