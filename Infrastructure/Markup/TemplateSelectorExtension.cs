using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Markup
{
    [MarkupExtensionReturnType(typeof(DataTemplateSelector))]
    public class TemplateSelectorExtension : MarkupExtension
    {
        private Func<object, object> _defaultKeySelector;

        public TemplateSelectorExtension()
        {
            _defaultKeySelector = i => i.GetType().GetProperty(Property).GetValue(i, null);
        }

        public TemplateSelectorExtension(Dictionary<object, DataTemplate> templatePresenter)
            : this()
        {
            TemplateDictionary = templatePresenter;
        }

        public string Property { get; set; }

        public Dictionary<object, DataTemplate> TemplateDictionary { get; set; }

        public Func<object, object> KeySelector { get; set; }

        #region MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (TemplateDictionary == null)
            {
                throw new ArgumentException("TemplateDictionary can not be null");
            }

            if (string.IsNullOrEmpty(Property))
            {
                throw new ArgumentException("Property value can not be null or empty");
            }

            return new TemplateProvider(this);
        }

        #endregion

        public DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var key = KeySelector == null
                ? _defaultKeySelector.Invoke(item)
                : KeySelector.Invoke(item);

            if (TemplateDictionary.ContainsKey(key))
                return TemplateDictionary[key];

            return null;
        }

        class TemplateProvider : DataTemplateSelector
        {
            TemplateSelectorExtension _templateSelector;

            public TemplateProvider(TemplateSelectorExtension extension)
            {
                _templateSelector = extension;
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return _templateSelector.SelectTemplate(item, container);
            }
        }
    }
}
