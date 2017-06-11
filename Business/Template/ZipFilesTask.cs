using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class ZipFilesTask : Task
    {
        public ZipFilesTask(zip task) 
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

        public ZipFilesTask(string path, string file, bool overwrite = false, string condition = null)
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
            return (zip)this;
        }

        public static explicit operator zip(ZipFilesTask zip)
        {
            return new zip
            {
                file = zip.File,
                path = zip.Path,
                overwrite = zip.Overwrite,
                condition = string.IsNullOrWhiteSpace(zip.Condition) ? null : zip.Condition,
            };
        }
    }
}
