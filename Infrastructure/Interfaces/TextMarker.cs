using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public sealed class TextMarker : TextSegment
    {
        public TextMarker(int startOffset, int length)
        {
            StartOffset = startOffset;
            Length = length;
        }

        public Color? BackgroundColor { get; set; }
        public Color MarkerColor { get; set; }
        public string ToolTip { get; set; }
    }
}
