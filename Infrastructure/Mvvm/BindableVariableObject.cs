using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Properties;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm
{
    public abstract class BindableVariableObject<TValue> : IDisposable
    {
        private INotifyPropertyChanged _container;
        private string _propertyName;
        private PropertyInfo _targetProperty;
        private List<string> _keys = new List<string>();

        protected BindableVariableObject(INotifyPropertyChanged container, string propertyName)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            _targetProperty = container.GetType().GetProperty(propertyName);

            if (_targetProperty == null)
            {
                throw new ArgumentException(Resources.PropertyNotFound, nameof(propertyName));
            }

            _container = container;
            _propertyName = propertyName;

            Variables = ServiceLocator.Current.GetInstance<IVariableStore>();
            Logger = ServiceLocator.Current.GetInstance<IApplicationLogger>();
        }

        protected string Expression { get; private set; }

        protected IVariableStore Variables { get; }

        protected IApplicationLogger Logger { get; }

        public IEnumerable<string> Keys => _keys;

        protected abstract TValue GetVariableValue();

        protected abstract void SetVariableValue(TValue value);

        public void BindKey(string key)
        {
            Bind($"%{key}%");
        }

        public virtual void Bind(string expression)
        {
            Expression = expression;

            var keys = Variables.GetExpressionVariableKeys(Expression);

            _keys.AddRange(keys);

            if (!Keys.Any())
            {
                // No variable(s) found in expression so the expression is a value
                SetStaticValue();
                return;
            }

            // Add listener for container
            _container.PropertyChanged += ContainerPropertyChanged;

            // Add listener for each variables in the expression
            Variables.PropertyChanged += VariablePropertyChanged;
        }

        private void ContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _propertyName)
                return;

            var value = GetContainerValue();
            SetVariableValue(value);
        }

        private void VariablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_keys.Contains(e.PropertyName))
                return;

            var value = GetVariableValue();
            SetContainerValue(value);
        }

        private void SetStaticValue()
        {
            try
            {
                var value = ConverterHelper.Convert<TValue>(Expression);
                SetContainerValue(value);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private TValue GetContainerValue()
        {
            return (TValue) _targetProperty.GetValue(_container);
        }

        private void SetContainerValue(TValue value)
        {
            _targetProperty.SetValue(_container, value);
        }

        public void Dispose()
        {
            // Remove listener for container
            _container.PropertyChanged -= ContainerPropertyChanged;

            // Remove listener for variables
            Variables.PropertyChanged -= VariablePropertyChanged;
        }
    }
}
