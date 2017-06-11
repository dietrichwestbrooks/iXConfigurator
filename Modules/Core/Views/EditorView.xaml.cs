using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Services;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView : IEditorView
    {
        private ITextMarkerService _textMarkerService;
        private ToolTip _toolTip;
        private FoldingManager _foldingManager;
        private XmlFoldingStrategy _foldingStrategy;

        public static RoutedCommand ValidateCommand { get; set; } = new RoutedCommand("Validate", typeof(EditorView));

        public EditorView(ITextMarkerService textMarkerService)
        {
            _textMarkerService = textMarkerService;

            InitializeComponent();

            ErrorsRow.Height = new GridLength(0);

            var rowStyle = new Style(typeof(DataGridRow), (Style)FindResource("MetroDataGridRow"));
            rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent,
                         new MouseButtonEventHandler(OnErrorsGridMouseDoubleClick)));
            ErrorsGrid.RowStyle = rowStyle;

            _foldingManager = FoldingManager.Install(TextEditor.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();

            TextEditor.TextChanged += (sender, args) =>
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, TextEditor.Document);
            };

            _textMarkerService.SetTextEditor(TextEditor);

            var textView = TextEditor.TextArea.TextView;
            textView.BackgroundRenderers.Add(_textMarkerService);
            textView.LineTransformers.Add(_textMarkerService);
            textView.Services.AddService(typeof(ITextMarkerService), _textMarkerService);

            textView.MouseHover += MouseHover;
            textView.MouseHoverStopped += TextEditorMouseHoverStopped;
            textView.VisualLinesChanged += VisualLinesChanged;
        }

        private void MouseHover(object sender, MouseEventArgs e)
        {
            var pos = TextEditor.TextArea.TextView.GetPositionFloor(e.GetPosition(TextEditor.TextArea.TextView) + TextEditor.TextArea.TextView.ScrollOffset);
            var inDocument = pos.HasValue;
            if (inDocument)
            {
                var logicalPosition = pos.Value.Location;
                var offset = TextEditor.Document.GetOffset(logicalPosition);

                var markersAtOffset = _textMarkerService.GetMarkersAtOffset(offset);
                TextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (_toolTip == null)
                    {
                        _toolTip = new ToolTip();
                        _toolTip.Closed += ToolTipClosed;
                        _toolTip.PlacementTarget = this;
                        _toolTip.Content = new TextBlock
                        {
                            Text = markerWithToolTip.ToolTip,
                            TextWrapping = TextWrapping.Wrap
                        };
                        _toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        void ToolTipClosed(object sender, RoutedEventArgs e)
        {
            _toolTip = null;
        }

        void TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (_toolTip != null)
            {
                _toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void VisualLinesChanged(object sender, EventArgs e)
        {
            if (_toolTip != null)
            {
                _toolTip.IsOpen = false;
            }
        }

        private void Validate(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Validate();
            }
            catch
            {
                // Ignore
            }
        }

        public bool Validate()
        {
            IServiceProvider sp = TextEditor;
            var markerService = (TextMarkerService)sp.GetService(typeof(ITextMarkerService));
            markerService.Clear();

            try
            {
                ValidationErrors.Clear();

                var schema = XmlSchema.Read(File.OpenText("template.xsd"), ValidateErrors);

                var settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    ValidationFlags =
                            XmlSchemaValidationFlags.ProcessInlineSchema |
                            XmlSchemaValidationFlags.ProcessSchemaLocation |
                            XmlSchemaValidationFlags.ReportValidationWarnings,
                };

                settings.Schemas.Add(schema);
                settings.ValidationEventHandler += ValidateErrors;

                var tr = new XmlTextReader(TextEditor.Document.Text, XmlNodeType.Document, null);

                var xr = XmlReader.Create(tr, settings);

                while (xr.Read())
                {
                }

                if (!ValidationErrors.Any())
                {
                    ErrorsRow.Height = new GridLength(0);
                }
            }
            catch (XmlException ex)
            {
                DisplayValidationError(ex.Message, ex.LinePosition, ex.LineNumber);
            }
            catch (Exception)
            {
                // Do Nothing
            }

            return !ValidationErrors.Any();
        } 

        private void ValidateErrors(object sender, ValidationEventArgs e)
        {
            DisplayValidationError(e.Message, e.Exception.LinePosition, e.Exception.LineNumber);
        }

        public ObservableCollection<ValidationError> ValidationErrors { get; } = new ObservableCollection<ValidationError>();

        private void DisplayValidationError(string message, int linePosition, int lineNumber)
        {
            ErrorsRow.Height = new GridLength(1, GridUnitType.Star);

            ValidationErrors.Add(new ValidationError
                {
                    Message = message,
                    LineNumber = lineNumber,
                    LinePosition = linePosition
                });

            if (lineNumber < 1 || lineNumber > TextEditor.Document.LineCount)
                return;

            var offset = TextEditor.Document.GetOffset(new TextLocation(lineNumber, linePosition));
            var endOffset = TextUtilities.GetNextCaretPosition(TextEditor.Document, offset,
                System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);

            if (endOffset < 0)
            {
                endOffset = TextEditor.Document.TextLength;
            }

            var length = endOffset - offset;

            if (length < 2)
            {
                length = Math.Min(2, TextEditor.Document.TextLength - offset);
            }

            _textMarkerService.Create(offset, length, message);
        }

        private void OnErrorsGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;

            var error = row?.Item as ValidationError;

            if (error == null)
            {
                return;
            }

            var offset = TextEditor.Document.GetOffset(new TextLocation(error.LineNumber, error.LinePosition));

            var endOffset = TextUtilities.GetNextCaretPosition(TextEditor.Document, offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);

            TextEditor.CaretOffset = endOffset;

            TextEditor.ScrollTo(error.LineNumber, error.LinePosition);
        }
    }

    public class ValidationError
    {
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
    }
}
