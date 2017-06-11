using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public abstract class Task
    {
        protected Task(task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Condition = task.condition ?? string.Empty;
        }

        protected Task(string condition = null)
        {
            Condition = condition ?? string.Empty;
        }

        public abstract task ToTask();

        public string Condition { get; set; }

        public static Task CreateTask(task task)
        {
            Task newTask;

            if (task is copy)
            {
                newTask = new CopyFilesTask((copy)task);
            }
            else if (task is delete)
            {
                newTask = new DeleteFilesTask((delete)task);
            }
            else if (task is move)
            {
                newTask = new MoveFilesTask((move)task);
            }
            else if (task is download)
            {
                newTask = new DownloadFileTask((download)task);
            }
            else if (task is readXml)
            {
                newTask = new ReadXmlTask((readXml)task);
            }
            else if (task is writeXml)
            {
                newTask = new WriteXmlTask((writeXml)task);
            }
            else if (task is writeXmlTable)
            {
                newTask = new WriteXmlTableTask((writeXmlTable)task);
            }
            else if (task is set)
            {
                newTask = new SetVariableTask((set)task);
            }
            else if (task is unset)
            {
                newTask = new UnsetVariableTask((unset)task);
            }
            else if (task is zip)
            {
                newTask = new ZipFilesTask((zip)task);
            }
            else if (task is unzip)
            {
                newTask = new UnzipFilesTask((unzip)task);
            }
            else
            {
                throw new NotSupportedException(task.GetType().AssemblyQualifiedName); 
            }

            return newTask;
        }
    }
}
