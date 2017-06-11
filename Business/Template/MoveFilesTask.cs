using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class MoveFilesTask : Task
    {
        public MoveFilesTask(move task) 
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            FromPath = task.from ?? string.Empty;
            ToPath = task.to ?? string.Empty;
            Overwrite = task.overwrite;
        }

        public MoveFilesTask(string fromPath, string toPath, bool overwrite = false, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(fromPath))
            {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrWhiteSpace(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }

            FromPath = fromPath;
            ToPath = toPath;
            Overwrite = overwrite;
        }

        public string FromPath { get; set; }

        public string ToPath { get; set; }

        public bool Overwrite { get; set; }

        public override task ToTask()
        {
            return (move)this;
        }

        public static explicit operator move(MoveFilesTask move)
        {
            return new move
            {
                from = move.FromPath,
                to = move.ToPath,
                overwrite = move.Overwrite,
                condition = string.IsNullOrWhiteSpace(move.Condition) ? null : move.Condition,
            };
        }
    }
}
