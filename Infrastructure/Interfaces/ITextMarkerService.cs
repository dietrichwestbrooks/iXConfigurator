using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface ITextMarkerService : IBackgroundRenderer, IVisualLineTransformer
    {
        void Clear();

        void Create(int offset, int length, string message);

        IEnumerable<TextMarker> GetMarkersAtOffset(int offset);

        void SetTextEditor(TextEditor textEditor);
    }
}
