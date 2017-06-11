using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Unchecked
    {
        private List<Option> _options = new List<Option>();
        private List<Task> _tasks = new List<Task>();

        public Unchecked()
        {
            
        }

        public Unchecked(@unchecked @unchecked)
        {
            if (@unchecked == null)
            {
                throw new ArgumentNullException(nameof(@unchecked));
            }

            if (@unchecked.Items != null)
            {
                _tasks.AddRange(@unchecked.Items.OfType<task>().Select(Task.CreateTask));
                _options.AddRange(@unchecked.Items.OfType<option>().Select(option => new Option(option)));
            }
        }

        public IList<Option> Options => _options;

        public IList<Task> Tasks => _tasks;

        public static explicit operator @unchecked(Unchecked @unchecked)
        {
            return new @unchecked
                {
                    Items = @unchecked.Options.Select(o => (option) o)
                        .Concat<object>(@unchecked.Tasks.Select(t => t.ToTask()))
                        .ToArray()
                };
        }
    }
}
