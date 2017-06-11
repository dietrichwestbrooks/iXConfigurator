using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Item
    {
        private List<Task> _tasks = new List<Task>();

        public Item(item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Value = item.value ?? string.Empty;
            Name = item.name ?? string.Empty;
            Filter = item.filter ?? string.Empty;

            if (item.Items != null)
            {
                _tasks.AddRange(item.Items.Select(Task.CreateTask));
            }
        }

        public Item(string value, string name = null, string filter = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
            Name = name ?? string.Empty;
            Filter = filter ?? string.Empty;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Filter { get; set; }

        public IList<Task> Tasks => _tasks;

        public static explicit operator item(Item item)
        {
            return new item
            {
                name = !string.IsNullOrWhiteSpace(item.Name) ? item.Name : null,
                filter = !string.IsNullOrWhiteSpace(item.Filter) ? item.Filter : null,
                value = item.Value,
                Items = item.Tasks.Select(t => t.ToTask()).ToArray()
            };
        }
    }
}
