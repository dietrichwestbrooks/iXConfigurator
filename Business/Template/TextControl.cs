using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class TextControl : Control
    {
        public TextControl(text control)
            : base(control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Value = control.value ?? string.Empty;
            Hint = control.hint ?? string.Empty;
            Pattern = control.pattern ?? string.Empty;
            Restriction = Translate(control.restriction);
        }

        public TextControl(string visible, string enabled, string value = null, string hint = null, string pattern = null,
            TextRestriction restriction = TextRestriction.None)
            : base(visible, enabled)
        {
            Value = value ?? string.Empty;
            Hint = hint ?? string.Empty;
            Pattern = pattern ?? string.Empty;
            Restriction = restriction;
        }

        public string Value { get; set; }

        public string Hint { get; set; }

        public string Pattern { get; set; }

        public TextRestriction Restriction { get; set; }

        private static textRestriction Translate(TextRestriction from)
        {
            textRestriction to;

            switch (from)
            {
                case TextRestriction.None:
                {
                    to = textRestriction.none;
                    break;
                }

                case TextRestriction.IPv4:
                {
                    to = textRestriction.ipv4;
                    break;
                }

                case TextRestriction.IPv6:
                {
                    to = textRestriction.ipv6;
                    break;
                }

                case TextRestriction.Number:
                {
                    to = textRestriction.number;
                    break;
                }

                case TextRestriction.Port:
                {
                    to = textRestriction.port;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(from), from, null);
            }

            return to;
        }

        private TextRestriction Translate(textRestriction from)
        {
            TextRestriction to;

            switch (from)
            {
                case textRestriction.none:
                {
                    to = TextRestriction.None;
                    break;
                }

                case textRestriction.ipv4:
                {
                    to = TextRestriction.IPv4;
                    break;
                }

                case textRestriction.ipv6:
                {
                    to = TextRestriction.IPv6;
                    break;
                }

                case textRestriction.number:
                {
                    to = TextRestriction.Number;
                    break;
                }

                case textRestriction.port:
                {
                    to = TextRestriction.Port;
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(from), from, null);
            }

            return to;
        }

        public override control ToControl()
        {
            return (text) this;
        }

        public static explicit operator text(TextControl text)
        {
            return new text
                {
                    visible = !string.IsNullOrWhiteSpace(text.Visible) ? text.Visible : null,
                    enabled = !string.IsNullOrWhiteSpace(text.Enabled) ? text.Enabled : null,
                    value = !string.IsNullOrWhiteSpace(text.Value) ? text.Value : null,
                    hint = !string.IsNullOrWhiteSpace(text.Hint) ? text.Hint : null,
                    pattern = !string.IsNullOrWhiteSpace(text.Pattern) ? text.Pattern : null,
                    restriction = Translate(text.Restriction)
                };
        }
    }
}
