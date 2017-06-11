using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class Product
    {
        public Product(product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            Name = product.name ?? string.Empty;
            Version = product.version ?? string.Empty;
            Description = product.Value ?? string.Empty;
        }

        public Product(string name, string version, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            Name = name;
            Version = version;
            Description = description ?? string.Empty;
        }

        public string Name { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public static explicit operator product(Product product)
        {
            return new product
                {
                    Value = product.Description,
                    name = product.Name,
                    version = product.Version
                };
        }
    }
}
