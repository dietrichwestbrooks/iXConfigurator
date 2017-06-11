using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class UnzipFilesTask : Task
    {
        public UnzipFilesTask(unzip task) 
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Path = task.path ?? string.Empty;
            File = task.file ?? string.Empty;
            Overwrite = task.overwrite;
        }

        public UnzipFilesTask(string file, string path, bool overwrite = false, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            File = file;
            Path = path;
            Overwrite = overwrite;
        }

        public string Path { get; set; }

        public string File { get; set; }

        public bool Overwrite { get; set; }

        public override task ToTask()
        {
            return (unzip)this;
        }

        public static explicit operator unzip(UnzipFilesTask unzip)
        {
            return new unzip
            {
                file = unzip.File,
                path = unzip.Path,
                overwrite = unzip.Overwrite,
                condition = string.IsNullOrWhiteSpace(unzip.Condition) ? null : unzip.Condition,
            };
        }
    }
}
