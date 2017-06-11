using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class ListControl : Control
    {
        private List<Item> _items = new List<Item>();

        public ListControl(list control)
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

        public ListControl(string visible, string enabled, string filter = null)
            : base(visible, enabled)
        {
            Filter = filter ?? string.Empty;
        }

        public string Filter { get; set; }

        public IList<Item> Items => _items;

        public override control ToControl()
        {
            return (list)this;
        }

        public static explicit operator list(ListControl list)
        {
            return new list
            {
                visible = !string.IsNullOrWhiteSpace(list.Visible) ? list.Visible : null,
                enabled = !string.IsNullOrWhiteSpace(list.Enabled) ? list.Enabled : null,
                filter = !string.IsNullOrWhiteSpace(list.Filter) ? list.Filter : null,
                item = list.Items.Select(i => (item)i).ToArray(),
            };
        }
    }
}
