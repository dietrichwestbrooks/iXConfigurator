namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public interface IEditorViewModelFactory
    {
        AttributeEditorViewModel CreateAttributeEditor(string value);

        CheckEditorViewModel CreateCheckEditor();

        ChoiceEditorViewModel CreateChoiceEditor(string value);

        ComboEditorViewModel CreateComboEditor();

        CopyEditorViewModel CreateCopyEditor();

        DeleteEditorViewModel CreateDeleteEditor();

        ItemEditorViewModel CreateItemEditor(string value);

        ListEditorViewModel CreateListEditor();

        MoveEditorViewModel CreateMoveEditor();

        PageEditorViewModel CreatePageEditor();

        RadioEditorViewModel CreateRadioEditor();

        SectionEditorViewModel CreateSectionEditor();

        SetEditorViewModel CreateSetEditor();

        TableEditorViewModel CreateTableEditor();

        TextEditorViewModel CreateTextEditor();

        UnsetEditorViewModel CreateUnsetEditor();

        WriteXmlEditorViewModel CreateWriteXmlEditor();

        WriteXmlTableEditorViewModel CreateWriteXmlTableEditor();

        DownloadEditorViewModel CreateDownloadEditor();

        ZipEditorViewModel CreateZipEditor();

        UnzipEditorViewModel CreateUnzipEditor();
    }
}
