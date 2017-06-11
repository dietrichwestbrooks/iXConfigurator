using System;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class SnippetsFlyoutViewModel : ViewModelBase
    {
        private IEditorViewModelFactory _editorFactory;
        private EditorEnum? _editor;
        private object _currentEditor;

        public SnippetsFlyoutViewModel(IEditorViewModelFactory editorFactory)
        {
            _editorFactory = editorFactory;

            Title = "Snippets";
        }

        public EditorEnum? Editor
        {
            get { return _editor; }
            set { SetProperty(ref _editor, value, () => OnEditorChanged(value)); }
        }

        public object CurrentEditor
        {
            get { return _currentEditor; }
            set { SetProperty(ref _currentEditor, value); }
        }

        private void OnEditorChanged(EditorEnum? value)
        {
            switch (value)
            {
                case EditorEnum.CheckBox:
                    CurrentEditor = _editorFactory.CreateCheckEditor();
                    break;

                case EditorEnum.ComboBox:
                    CurrentEditor = _editorFactory.CreateComboEditor();
                    break;

                case EditorEnum.ListBox:
                    CurrentEditor = _editorFactory.CreateListEditor();
                    break;

                case EditorEnum.RadioButton:
                    CurrentEditor = _editorFactory.CreateRadioEditor();
                    break;

                case EditorEnum.Table:
                    CurrentEditor = _editorFactory.CreateTableEditor();
                    break;

                case EditorEnum.TextBox:
                    CurrentEditor = _editorFactory.CreateTextEditor();
                    break;

                case EditorEnum.SetVariable:
                    CurrentEditor = _editorFactory.CreateSetEditor();
                    break;

                case EditorEnum.UnsetVariable:
                    CurrentEditor = _editorFactory.CreateUnsetEditor();
                    break;

                case EditorEnum.MoveFiles:
                    CurrentEditor = _editorFactory.CreateMoveEditor();
                    break;

                case EditorEnum.CopyFiles:
                    CurrentEditor = _editorFactory.CreateCopyEditor();
                    break;

                case EditorEnum.DeleteFiles:
                    CurrentEditor = _editorFactory.CreateDeleteEditor();
                    break;

                case EditorEnum.WriteXml:
                    CurrentEditor = _editorFactory.CreateWriteXmlEditor();
                    break;

                case EditorEnum.WriteXmlTable:
                    CurrentEditor = _editorFactory.CreateWriteXmlTableEditor();
                    break;

                case EditorEnum.DownloadFile:
                    CurrentEditor = _editorFactory.CreateDownloadEditor();
                    break;

                case EditorEnum.ZipFiles:
                    CurrentEditor = _editorFactory.CreateZipEditor();
                    break;

                case EditorEnum.UnzipFiles:
                    CurrentEditor = _editorFactory.CreateUnzipEditor();
                    break;

                case EditorEnum.Page:
                    CurrentEditor = _editorFactory.CreatePageEditor();
                    break;

                case EditorEnum.Section:
                    CurrentEditor = _editorFactory.CreateSectionEditor();
                    break;

                case null:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
