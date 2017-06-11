using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public enum EditorEnum
    {
        [Description("Check Box")]
        CheckBox,

        [Description("Copy Files Task")]
        CopyFiles,

        [Description("Delete Files Task")]
        DeleteFiles,

        [Description("Download File Task")]
        DownloadFile,

        [Description("Drop-Down List Box")]
        ComboBox,

        [Description("List Box")]
        ListBox,

        [Description("Move Files Task")]
        MoveFiles,

        [Description("Page")]
        Page,

        [Description("Radio Button List")]
        RadioButton,

        [Description("Section")]
        Section,

        [Description("Set Varaible Task")]
        SetVariable,

        [Description("Table")]
        Table,

        [Description("Text Box")]
        TextBox,

        [Description("Unset Varaible Task")]
        UnsetVariable,

        [Description("Unzip Files Task")]
        UnzipFiles,

        [Description("Write XML Table Task")]
        WriteXmlTable,

        [Description("Write XML Task")]
        WriteXml,

        [Description("Zip Files Task")]
        ZipFiles,
    }
}
