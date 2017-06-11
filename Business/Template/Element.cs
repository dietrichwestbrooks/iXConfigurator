using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public abstract class Element
    {
        protected Element(element element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            Visible = element.visible ?? string.Empty;
        }

        protected Element(string visible = "true")
        {
            if (string.IsNullOrWhiteSpace(visible))
            {
                throw new ArgumentNullException(nameof(visible));
            }

            Visible = visible;
        }

        public string Visible { get; set; }
    }
}
