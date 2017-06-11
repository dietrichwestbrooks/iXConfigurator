using Microsoft.Practices.Unity;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class EditorViewModelFactory : IEditorViewModelFactory
    {
        private IUnityContainer _container;

        public EditorViewModelFactory(IUnityContainer container)
        {
            _container = container;
        }

        public AttributeEditorViewModel CreateAttributeEditor(string value)
        {
            return _container.Resolve<AttributeEditorViewModel>(
                new ParameterOverride(nameof(value), value));
        }

        public CheckEditorViewModel CreateCheckEditor()
        {
            return _container.Resolve<CheckEditorViewModel>();
        }

        public ChoiceEditorViewModel CreateChoiceEditor(string value)
        {
            return _container.Resolve<ChoiceEditorViewModel>(
                new ParameterOverride(nameof(value), value));
        }

        public ComboEditorViewModel CreateComboEditor()
        {
            return _container.Resolve<ComboEditorViewModel>();
        }

        public CopyEditorViewModel CreateCopyEditor()
        {
            return _container.Resolve<CopyEditorViewModel>();
        }

        public DeleteEditorViewModel CreateDeleteEditor()
        {
            return _container.Resolve<DeleteEditorViewModel>();
        }

        public ItemEditorViewModel CreateItemEditor(string value)
        {
            return _container.Resolve<ItemEditorViewModel>(
                new ParameterOverride(nameof(value), value));
        }

        public ListEditorViewModel CreateListEditor()
        {
            return _container.Resolve<ListEditorViewModel>();
        }

        public MoveEditorViewModel CreateMoveEditor()
        {
            return _container.Resolve<MoveEditorViewModel>();
        }

        public PageEditorViewModel CreatePageEditor()
        {
            return _container.Resolve<PageEditorViewModel>();
        }

        public RadioEditorViewModel CreateRadioEditor()
        {
            return _container.Resolve<RadioEditorViewModel>();
        }

        public SectionEditorViewModel CreateSectionEditor()
        {
            return _container.Resolve<SectionEditorViewModel>();
        }

        public SetEditorViewModel CreateSetEditor()
        {
            return _container.Resolve<SetEditorViewModel>();
        }

        public TableEditorViewModel CreateTableEditor()
        {
            return _container.Resolve<TableEditorViewModel>();
        }

        public TextEditorViewModel CreateTextEditor()
        {
            return _container.Resolve<TextEditorViewModel>();
        }

        public UnsetEditorViewModel CreateUnsetEditor()
        {
            return _container.Resolve<UnsetEditorViewModel>();
        }

        public WriteXmlEditorViewModel CreateWriteXmlEditor()
        {
            return _container.Resolve<WriteXmlEditorViewModel>();
        }

        public WriteXmlTableEditorViewModel CreateWriteXmlTableEditor()
        {
            return _container.Resolve<WriteXmlTableEditorViewModel>();
        }

        public DownloadEditorViewModel CreateDownloadEditor()
        {
            return _container.Resolve<DownloadEditorViewModel>();
        }

        public ZipEditorViewModel CreateZipEditor()
        {
            return _container.Resolve<ZipEditorViewModel>();
        }

        public UnzipEditorViewModel CreateUnzipEditor()
        {
            return _container.Resolve<UnzipEditorViewModel>();
        }
    }
}
