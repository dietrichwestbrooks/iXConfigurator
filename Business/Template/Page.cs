using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Page : Element
    {
        private List<Section> _sections = new List<Section>();
        private List<Option> _options = new List<Option>();

        public Page(page page)
            : base(page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            Title = page.title ?? string.Empty;
            Icon = page.icon ?? string.Empty;
            Description = page.description ?? string.Empty;
            Summary = page.summary ?? string.Empty;

            if (page.Items != null)
            {
                _sections.AddRange(page.Items.OfType<section>().Select(section => new Section(section)));
                _options.AddRange(page.Items.OfType<option>().Select(option => new Option(option)));
            }
        }

        public Page(string title, string visible, string description = null, string summary = null, string icon = null)
            : base(visible)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            Title = title;
            Icon = icon ?? string.Empty;
            Description = description ?? string.Empty;
            Summary = summary ?? string.Empty;
        }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public IList<Section> Sections => _sections;

        public IList<Option> Options => _options;

        public static explicit operator page(Page page)
        {
            return new page
                {
                    description = string.IsNullOrWhiteSpace(page.Description) ? null : page.Description,
                    icon = string.IsNullOrWhiteSpace(page.Icon) ? null : page.Icon,
                    summary = string.IsNullOrWhiteSpace(page.Summary) ? null : page.Summary,
                    title = page.Title,
                    visible = page.Visible,
                    Items = page.Options.Select(o => (option) o)
                            .Concat<element>(page.Sections.Select(s => (section) s))
                            .ToArray()
                };
        }
    }
}
