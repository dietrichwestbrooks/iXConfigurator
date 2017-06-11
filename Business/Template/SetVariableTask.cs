using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class SetVariableTask : Task
    {
        public SetVariableTask(set task)
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Key = task.key ?? string.Empty;
            Value = task.value ?? string.Empty;
        }

        public SetVariableTask(string key, string value, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public override task ToTask()
        {
            return (set)this;
        }

        public static explicit operator set(SetVariableTask set)
        {
            return new set
            {
                key = set.Key,
                value = set.Value,
                condition = string.IsNullOrWhiteSpace(set.Condition) ? null : set.Condition,
            };
        }
    }
}
