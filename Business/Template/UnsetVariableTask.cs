using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class UnsetVariableTask : Task
    {
        public UnsetVariableTask(unset task) 
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Key = task.key ?? string.Empty;
        }

        public UnsetVariableTask(string key, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;
        }

        public string Key { get; set; }

        public override task ToTask()
        {
            return (unset)this;
        }

        public static explicit operator unset(UnsetVariableTask unset)
        {
            return new unset
            {
                key = unset.Key,
                condition = string.IsNullOrWhiteSpace(unset.Condition) ? null : unset.Condition,
            };
        }
    }
}
