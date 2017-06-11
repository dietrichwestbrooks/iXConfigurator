using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class DownloadFileTask : Task
    {
        public DownloadFileTask(download task)
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Url = task.url ?? string.Empty;
            File = task.file ?? string.Empty;
            Overwrite = task.overwrite;
        }

        public DownloadFileTask(string url, string file, bool overwrite = false, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            Url = url;
            File = file;
            Overwrite = overwrite;
        }

        public string Url { get; set; }

        public string File { get; set; }

        public bool Overwrite { get; set; }

        public override task ToTask()
        {
            return (download)this;
        }

        public static explicit operator download(DownloadFileTask download)
        {
            return new download
            {
                condition = string.IsNullOrWhiteSpace(download.Condition) ? null : download.Condition,
                url = download.Url,
                file = download.File,
                overwrite = download.Overwrite,
            };
        }
    }
}
