using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public enum WriteXmlCreateEnum
    {
        [Description("Do Not Create")]
        None,

        [Description("Create File and XPath")]
        File,

        [Description("Only Create XPath")]
        XPath,
    }
}
