using System.Windows;
using System.Windows.Controls;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Resources
{
    public class EditorTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            var element = container as FrameworkElement;

            if (element == null)
            {
                return null;
            }

            DataTemplate template = null;

            if (item is CheckEditorViewModel)
            {
                template = element.FindResource("CheckEditorTemplate") as DataTemplate;
            }
            else if (item is ComboEditorViewModel)
            {
                template = element.FindResource("ComboEditorTemplate") as DataTemplate;
            }
            else if (item is ListEditorViewModel)
            {
                template = element.FindResource("ListEditorTemplate") as DataTemplate;
            }
            else if (item is RadioEditorViewModel)
            {
                template = element.FindResource("RadioEditorTemplate") as DataTemplate;
            }
            else if (item is TableEditorViewModel)
            {
                template = element.FindResource("TableEditorTemplate") as DataTemplate;
            }
            else if (item is TextEditorViewModel)
            {
                template = element.FindResource("TextEditorTemplate") as DataTemplate;
            }
            else if (item is SetEditorViewModel)
            {
                template = element.FindResource("SetEditorTemplate") as DataTemplate;
            }
            else if (item is UnsetEditorViewModel)
            {
                template = element.FindResource("UnsetEditorTemplate") as DataTemplate;
            }
            else if (item is MoveEditorViewModel)
            {
                template = element.FindResource("MoveEditorTemplate") as DataTemplate;
            }
            else if (item is CopyEditorViewModel)
            {
                template = element.FindResource("CopyEditorTemplate") as DataTemplate;
            }
            else if (item is DeleteEditorViewModel)
            {
                template = element.FindResource("DeleteEditorTemplate") as DataTemplate;
            }
            else if (item is WriteXmlEditorViewModel)
            {
                template = element.FindResource("WriteXmlEditorTemplate") as DataTemplate;
            }
            else if (item is WriteXmlTableEditorViewModel)
            {
                template = element.FindResource("WriteXmlTableEditorTemplate") as DataTemplate;
            }
            else if (item is PageEditorViewModel)
            {
                template = element.FindResource("PageEditorTemplate") as DataTemplate;
            }
            else if (item is SectionEditorViewModel)
            {
                template = element.FindResource("SectionEditorTemplate") as DataTemplate;
            }
            else if (item is DownloadEditorViewModel)
            {
                template = element.FindResource("DownloadEditorTemplate") as DataTemplate;
            }
            else if (item is ZipEditorViewModel)
            {
                template = element.FindResource("ZipEditorTemplate") as DataTemplate;
            }
            else if (item is UnzipEditorViewModel)
            {
                template = element.FindResource("UnzipEditorTemplate") as DataTemplate;
            }

            return template;
        }
    }
}
