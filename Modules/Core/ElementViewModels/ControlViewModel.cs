using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public abstract class ControlViewModel : ElementViewModel
    {
        private string _label;
        private BindableVariableExpression<bool> _isEnabledProperty;
        private BindableVariableExpression<bool> _isRequiredProperty;
        private string _key;
        private bool _isEnabled;
        private bool _isRequired;
        private string _enabled;
        private string _required;

        protected ControlViewModel(string key, string label, string required, Control control)
            : base(control)
        {
            _key = key;
            _required = required;
            _enabled = control?.Enabled ?? "true";
            _label = string.IsNullOrWhiteSpace(label) ? key : label;

            _isEnabledProperty = new BindableVariableExpression<bool>(this, nameof(IsEnabled));
            _isRequiredProperty = new BindableVariableExpression<bool>(this, nameof(IsRequired));
        }

        public override void BindVariables()
        {
            base.BindVariables();

            _isEnabledProperty.Bind(_enabled);
            _isRequiredProperty.Bind(_required);
        }

        public virtual string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public bool IsRequired
        {
            get { return _isRequired; }
            set { SetProperty(ref _isRequired, value, () => OnIsRequiredChanged(value)); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value, () => OnIsEnabledChanged(value)); }
        }

        protected virtual void OnIsRequiredChanged(bool isRequired)
        {

        }

        protected override void OnIsVisbleChanged(bool isVisible)
        {
            IsActive = IsEnabled && isVisible;
        }

        protected void OnIsEnabledChanged(bool isEnabled)
        {
            IsActive = IsVisible && isEnabled;
        }

        public override void Dispose()
        {
            base.Dispose();

            _isEnabledProperty.Dispose();
            _isRequiredProperty.Dispose();
        }
    }

    public abstract class ControlViewModel<TValue> : ControlViewModel
    {
        private BindableVariableProperty<TValue> _valueProperty;
        private TValue _value;

        protected ControlViewModel(string key, string label, string required, Control control) 
            : base(key, label, required, control)
        {
            _valueProperty = new BindableVariableProperty<TValue>(this, nameof(Value));
        }

        public override void BindVariables()
        {
            base.BindVariables();

            _valueProperty.BindKey(Key);
        }

        public TValue Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value, () => OnValueChanged(value)); }
        }

        protected virtual void OnValueChanged(TValue newValue)
        {
            Events.GetEvent<DebugOutputEvent>().Publish($"> ixconfig set -Key {Key} -Value \"{newValue}\"\n");
            Events.GetEvent<DebugOutputEvent>().Publish($"{Key}={newValue}\n\n");
        }

        public override void Dispose()
        {
            base.Dispose();

            _valueProperty.Dispose();
        }
    }
}
