using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class CopyFilesTask : Task
    {
        public CopyFilesTask(copy task) 
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            From = task.from ?? string.Empty;
            To = task.to ?? string.Empty;
            Overwrite = task.overwrite;
        }

        public CopyFilesTask(string from, string to, bool overwrite = false, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentNullException(nameof(to));
            }

            From = from;
            To = to;
            Overwrite = overwrite;
        }

        public string From { get; set; }

        public string To { get; set; }

        public bool Overwrite { get; }

        public override task ToTask()
        {
            return (copy) this;
        }

        public static explicit operator copy(CopyFilesTask copy)
        {
            return new copy
                {
                    overwrite = copy.Overwrite,
                    from = copy.From,
                    to = copy.To,
                    condition = string.IsNullOrWhiteSpace(copy.Condition) ? null : copy.Condition,
                };
        }
    }
}
