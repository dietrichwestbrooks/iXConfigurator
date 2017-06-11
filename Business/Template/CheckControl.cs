using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class CheckControl : Control
    {
        public CheckControl(check control)
            : base(control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Default = control.@default;

            Checked = control.@checked != null ? new Checked(control.@checked) : new Checked();
            Unchecked = control.@unchecked != null ? new Unchecked(control.@unchecked) : new Unchecked();
        }

        public CheckControl(string visible, string enabled, bool @default = false)
            : base(visible, enabled)
        {
            Default = @default;
        }

        public Checked Checked { get; set; }

        public Unchecked Unchecked { get; set; }

        public bool Default { get; set; }

        public override control ToControl()
        {
            return (check)this;
        }

        public static explicit operator check(CheckControl check)
        {
            return new check
            {
                visible = !string.IsNullOrWhiteSpace(check.Visible) ? check.Visible : null,
                enabled = !string.IsNullOrWhiteSpace(check.Enabled) ? check.Enabled : null,
                @default = check.Default,
                @checked = (@checked)check.Checked,
                @unchecked = (@unchecked)check.Unchecked,
            };
        }
    }
}
