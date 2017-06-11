// ReSharper disable InconsistentNaming

using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public enum TextRestrictionEnum
    {
        [Description("None")]
        None,

        [Description("IPv4")]
        IPv4,

        [Description("IPv6")]
        IPv6,

        [Description("Port")]
        Port,

        [Description("Number")]
        Number
    }
}
