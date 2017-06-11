using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class RadioControl : Control
    {
        private List<Choice> _choices = new List<Choice>();
         
        public RadioControl(radio control)
            : base(control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Orientation = Translate(control.orientation);
            Filter = control.filter ?? string.Empty;

            if (control.choice != null)
            {
                _choices.AddRange(control.choice.Select(choice => new Choice(choice)));
            }
        }

        public RadioControl(string visible, string enabled, Orientation orientation = Orientation.Horizontal, string filter = null)
            : base(visible, enabled)
        {
            Orientation = orientation;
            Filter = filter ?? string.Empty;
        }

        public Orientation Orientation { get; set; }

        public string Filter { get; set; }

        public IList<Choice> Choices => _choices;

        private Orientation Translate(orientation orientation)
        {
            switch (orientation)
            {
                case orientation.vertical:
                    return Orientation.Vertical;

                case orientation.horizontal:
                    return Orientation.Horizontal;

                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        private static orientation Translate(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return Template.orientation.horizontal;

                case Orientation.Vertical:
                    return Template.orientation.vertical;

                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        public override control ToControl()
        {
            return (radio)this;
        }

        public static explicit operator radio(RadioControl radio)
        {
            return new radio
                {
                    visible = !string.IsNullOrWhiteSpace(radio.Visible) ? radio.Visible : null,
                    enabled = !string.IsNullOrWhiteSpace(radio.Enabled) ? radio.Enabled : null,
                    orientation = Translate(radio.Orientation),
                    filter = !string.IsNullOrWhiteSpace(radio.Filter) ? radio.Filter : null,
                    choice = radio.Choices.Select(c => (choice) c).ToArray(),
                };
        }
    }
}
