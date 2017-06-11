using System;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class TableControl : Control
    {
        public TableControl(table control)
            : base(control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Editable = control.editable;
            Data = control.Text?.Aggregate((s1, s2) => s1 + s2) ?? string.Empty;
        }

        public TableControl(string visible, string enabled, string data = null, bool? editable = null)
            : base(visible, enabled)
        {
            Editable = editable;
            Data = data ?? string.Empty;
        }

        public bool? Editable { get; set; }

        public string Data { get; set; }

        public override control ToControl()
        {
            return (table)this;
        }

        public static explicit operator table(TableControl table)
        {
            return new table
            {
                visible = !string.IsNullOrWhiteSpace(table.Visible) ? table.Visible : null,
                enabled = !string.IsNullOrWhiteSpace(table.Enabled) ? table.Enabled : null,
                editable = table.Editable.GetValueOrDefault(),
                Text = new [] { table.Data },
            };
        }
    }
}
