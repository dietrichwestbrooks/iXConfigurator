using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class ComboControl : Control
    {
        private List<Item> _items = new List<Item>();

        public ComboControl(combo control)
            : base(control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Filter = control.filter ?? string.Empty;

            if (control.item != null)
            {
                _items.AddRange(control.item.Select(item => new Item(item)));
            }
        }

        public ComboControl(string visible, string enabled, string filter = null)
            : base(visible, enabled)
        {
            Filter = filter ?? string.Empty;
        }

        public IList<Item> Items => _items;

        public string Filter { get; set; }

        public override control ToControl()
        {
            return (combo)this;
        }

        public static explicit operator combo(ComboControl combo)
        {
            return new combo
            {
                visible = !string.IsNullOrWhiteSpace(combo.Visible) ? combo.Visible : null,
                enabled = !string.IsNullOrWhiteSpace(combo.Enabled) ? combo.Enabled : null,
                filter = !string.IsNullOrWhiteSpace(combo.Filter) ? combo.Filter : null,
                item = combo.Items.Select(i => (item)i).ToArray(),
            };
        }
    }
}
