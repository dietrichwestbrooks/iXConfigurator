using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Choice
    {
        private List<Option> _options = new List<Option>();
        private List<Task> _tasks = new List<Task>();

        public Choice(choice choice)
        {
            if (choice == null)
            {
                throw new ArgumentNullException(nameof(choice));
            }

            Value = choice.value ?? string.Empty;
            Name = choice.name ?? string.Empty;
            Filter = choice.filter ?? string.Empty;

            if (choice.Items != null)
            {
                _tasks.AddRange(choice.Items.Select(Task.CreateTask));
            }
        }

        public Choice(string value, string name = null, string filter = null)
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

        public IList<Option> Options => _options;

        public IList<Task> Tasks => _tasks;

        public static explicit operator choice(Choice choice)
        {
            return new choice
            {
                name = !string.IsNullOrWhiteSpace(choice.Name) ? choice.Name : null,
                filter = !string.IsNullOrWhiteSpace(choice.Filter) ? choice.Filter : null,
                value = choice.Value,
                Items = choice.Tasks.Select(t => t.ToTask()).ToArray()
            };
        }
    }
}
