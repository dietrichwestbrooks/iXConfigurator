using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class DeleteFilesTask : Task
    {
        public DeleteFilesTask(delete task)
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Path = task.path ?? string.Empty;
        }

        public DeleteFilesTask(string path, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;
        }

        public string Path { get; set; }

        public override task ToTask()
        {
            return (delete)this;
        }

        public static explicit operator delete(DeleteFilesTask delete)
        {
            return new delete
                {
                    path = delete.Path,
                    condition = string.IsNullOrWhiteSpace(delete.Condition) ? null : delete.Condition,
                };
        }
    }
}
