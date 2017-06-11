using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class TextViewModel : ControlViewModel<string>, INotifyDataErrorInfo
    {
        private TextControl _control;
        private double _width;
        private string _hint;
        private string _pattern;
        private TextRestriction _restriction;
        private bool _allowValidation;

        public TextViewModel(string key, string label, string required, TextControl control)
            : base(key, label, required, control)
        {
            _control = control;

            _width = 300;
            _pattern = control.Pattern;
            _restriction = control.Restriction;
            _hint = control.Hint;
        }

        public override void InitializeVariables()
        {
            string value;

            if (!string.IsNullOrEmpty(_control.Value))
            {
                Variables.AddVariable(Key, _control.Value);
            }
            else if (Variables.TryGetVariableValue(Key, out value))
            {
                Value = value;
            }
            else
            {
                Variables.AddVariable(Key, string.Empty);
            }

            _allowValidation = true;
        }

        public override bool Validate()
        {
            var isValid = true;

            Errors.Clear();

            if (!IsActive || !_allowValidation)
            {
                return true;
            }

            if (IsRequired && string.IsNullOrWhiteSpace(Value))
            {
                Errors.Add($"{Label.TrimEnd(':')} is required");
                isValid = false;
            }
            else if (!string.IsNullOrWhiteSpace(Value) && _restriction == TextRestriction.IPv4 && !ValidateIPv4(Value))
            {
                Errors.Add("Not a valid IP Address");
                isValid = false;
            }
            else if (!string.IsNullOrWhiteSpace(Value) && _restriction == TextRestriction.IPv6 && !ValidateIPv6(Value))
            {
                Errors.Add("Not a valid IP Address");
                isValid = false;
            }
            else if (!string.IsNullOrWhiteSpace(Value) && _restriction == TextRestriction.Port && !ValidatePort(Value))
            {
                Errors.Add("Not a valid Port");
                isValid = false;
            }
            else if (!string.IsNullOrWhiteSpace(Value) && _restriction == TextRestriction.Number &&
                     !ValidateNumber(Value))
            {
                Errors.Add("Not a valid Number");
                isValid = false;
            }
            else if (!string.IsNullOrWhiteSpace(Value) && !string.IsNullOrWhiteSpace(Pattern) &&
                     !ValidatePattern(Value, Pattern))
            {
                Errors.Add("Invalid format");
                isValid = false;
            }

            HasErrors = !isValid;

            RaiseErrorsChanged(nameof(Value));

            return isValid;
        }

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        public string Hint
        {
            get { return _hint; }
            set { SetProperty(ref _hint, value); }
        }

        public string Pattern
        {
            get { return _pattern; }
            set { SetProperty(ref _pattern, value); }
        }

        protected List<string> Errors { get; } = new List<string>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != nameof(Value))
            {
                return Enumerable.Empty<string>();
            }

            return Errors;
        }

        public bool HasErrors { get; private set; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private static bool ValidatePattern(string value, string pattern)
        {
            var rgx = new Regex(pattern);
            return rgx.IsMatch(value);
        }

        private static bool ValidateNumber(string value)
        {
            var rgx = new Regex(@"(?:\d+)");
            return rgx.IsMatch(value);
        }

        private static bool ValidatePort(string value)
        {
            var rgx = new Regex(@"(?:[0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])");
            return rgx.IsMatch(value);
        }

        // ReSharper disable once InconsistentNaming
        private static bool ValidateIPv4(string value)
        {
            var rgx =
                new Regex(
                    @"(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])");
            return rgx.IsMatch(value);
        }

        // ReSharper disable once InconsistentNaming
        private static bool ValidateIPv6(string value)
        {
            var rgx = new Regex(@"\A(?:[0 - 9a - fA - F]{ 1,4}:){ 7}[0-9a-fA-F]{1,4}\z");

            if (rgx.IsMatch(value))
            {
                return true;
            }

            rgx = new Regex(
                    @"\A((?:[0-9A-Fa-f]{ 1,4}(?::[0-9A-Fa-f]{1,4})*)?)::((?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)\z");

            if (rgx.IsMatch(value))
            {
                return true;
            }

            rgx = new Regex(
                    @"\A((?:[0-9A-Fa-f]{ 1,4}:){6,6})(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}\z");

            if (rgx.IsMatch(value))
            {
                return true;
            }

            rgx = new Regex(
                    @"\A((?:[0-9A-Fa-f]{ 1,4}(?::[0-9A-Fa-f]{1,4})*)?) ::((?:[0-9A-Fa-f]{1,4}:)*)(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}\z");

            return rgx.IsMatch(value);
        }
    }
}
