using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public enum ClauseOperator
    {
        [Description("=")]
        Equal,

        [Description("!=")]
        NotEqual,

        [Description("Is True")]
        IsTrue,

        [Description("Is False")]
        IsFalse,

        [Description("Is Empty")]
        IsEmpty,

        [Description("Is Not Empty")]
        IsNotEmpty,
    }
}
