using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Section : Element
    {
        private List<Option> _options = new List<Option>();

        public Section(section section)
            : base(section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Title = section.title ?? string.Empty;
            Description = section.description ?? string.Empty;
            Icon = section.icon ?? string.Empty;

            if (section.option != null)
            {
                _options.AddRange(section.option.Select(option => new Option(option)));
            }
        }

        public Section(string title, string visible, string description = null, string icon = null)
            : base(visible)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            Title = title;
            Description = description ?? string.Empty;
            Icon = icon ?? string.Empty;
        }

        public string Title { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public IList<Option> Options => _options;

        public static explicit operator section(Section section)
        {
            return new section
            {
                title = section.Title,
                visible = section.Visible,
                description = string.IsNullOrWhiteSpace(section.Description) ? null : section.Description,
                icon = string.IsNullOrWhiteSpace(section.Icon) ? null : section.Icon,
                option = section.Options.Select(o => (option)o).ToArray()
            };
        }
    }
}
