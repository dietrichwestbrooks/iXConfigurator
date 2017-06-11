using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Checked
    {
        private List<Option> _options = new List<Option>();
        private List<Task> _tasks = new List<Task>();

        public Checked()
        {
            
        }

        public Checked(@checked @checked)
        {
            if (@checked == null)
            {
                throw new ArgumentNullException(nameof(@checked));
            }

            if (@checked.Items != null)
            {
                _tasks.AddRange(@checked.Items.OfType<task>().Select(Task.CreateTask));
                _options.AddRange(@checked.Items.OfType<option>().Select(option => new Option(option)));
            }
        }

        public IList<Option> Options => _options;

        public IList<Task> Tasks => _tasks;

        public static explicit operator @checked(Checked @checked)
        {
            return new @checked
                {
                    Items = @checked.Options.Select(o => (option) o)
                            .Concat<object>(@checked.Tasks.Select(t => t.ToTask()))
                            .ToArray()
                };
        }
    }
}
