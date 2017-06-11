using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Option : Element
    {
        public Option(option option)
            : base(option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            Key = option.key ?? string.Empty;
            Label = option.label ?? string.Empty;
            Required = option.required;

            if (option.Item != null)
            {
                Control = Control.CreateControl(option.Item);
            }
        }

        public Option(string key, string label, string visible, string required = "false")
            : base(visible)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            Key = key;
            Label = label;
            Required = required;
        }

        public string Key { get; set; }

        public string Label { get; set; }

        public string Required { get; set; }

        public Control Control { get; set; }

        public static explicit operator option(Option option)
        {
            return new option
            {
                key = option.Key,
                visible = option.Visible,
                label = option.Label,
                required = option.Required,
                Item = option.Control.ToControl()
            };
        }
    }
}
