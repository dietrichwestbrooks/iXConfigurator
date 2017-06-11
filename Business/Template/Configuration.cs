using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Configuration
    {
        private List<Page> _pages = new List<Page>();
        private List<Task> _build = new List<Task>();
        private List<Task> _import = new List<Task>();

        public Configuration(string libraryPath, string templatePath, configurationTemplate template)
        {
            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                throw new ArgumentNullException(nameof(libraryPath));
            }

            if (string.IsNullOrWhiteSpace(templatePath))
            {
                throw new ArgumentNullException(nameof(templatePath));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            TemplatePath = templatePath;
            LibraryPath = libraryPath;

            if (string.IsNullOrWhiteSpace(LibraryPath))
            {
                throw new ArgumentException(templatePath, nameof(templatePath));
            }

            Product = new Product(template.product);

            if (template.pages != null)
            {
                _pages.AddRange(template.pages.Select(page => new Page(page)));
            }

            if (template.build != null)
            {
                _build.AddRange(template.build.Select(Task.CreateTask));
            }

            if (template.import != null)
            {
                _import.AddRange(template.import.Select(Task.CreateTask));
            }
        }

        public Configuration(string libraryPath, string templatePath)
        {
            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                throw new ArgumentNullException(nameof(libraryPath));
            }

            if (string.IsNullOrWhiteSpace(templatePath))
            {
                throw new ArgumentNullException(nameof(templatePath));
            }

            LibraryPath = libraryPath;
            TemplatePath = templatePath;
        }

        public string LibraryPath { get; set; }

        public string TemplatePath { get; set; }

        public IList<Page> Pages => _pages;

        public IList<Task> Build => _build;

        public IList<Task> Import => _import;

        public Product Product { get; set; }

        public static explicit operator configurationTemplate(Configuration template)
        {
            return new configurationTemplate
                {
                    product = (product)template.Product,
                    pages = template.Pages.Select(p => (page) p).ToArray(),
                    build = template.Build.Select(t => t.ToTask()).ToArray(),
                    import = template.Import.Select(t => t.ToTask()).ToArray()
                };
        }
    }
}
