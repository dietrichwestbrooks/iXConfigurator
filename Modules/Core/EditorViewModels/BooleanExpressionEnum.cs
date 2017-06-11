using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public enum BooleanExpressionEnum
    {
        [Description("True")]
        True,

        [Description("False")]
        False,

        [Description("<Build Expression...>")]
        Build,
    }
}
