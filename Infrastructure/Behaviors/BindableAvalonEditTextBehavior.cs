using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Behaviors
{
    public class BindableAvalonEditTextBehavior : Behavior<TextEditor>
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BindableAvalonEditTextBehavior),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var text = e.NewValue.ToString();

            var behavior = sender as BindableAvalonEditTextBehavior;

            var textEditor = behavior?.AssociatedObject;

            if (textEditor?.Document == null)
                return;

            var caretOffset = textEditor.CaretOffset;
            textEditor.Document.Text = text;
            if (!textEditor.IsReadOnly && caretOffset <= textEditor.Text.Length)
                textEditor.CaretOffset = caretOffset;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            var textEditor = AssociatedObject as ITextEditorComponent;

            if (textEditor?.Document == null)
                return;

            if (Text != (string)TextProperty.DefaultMetadata.DefaultValue)
                textEditor.Document.Text = Text;

            textEditor.Document.TextChanged += OnTextEditorTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            var textEditor = AssociatedObject as ITextEditorComponent;

            if (textEditor?.Document != null)
                textEditor.Document.TextChanged -= OnTextEditorTextChanged;
        }

        private void OnTextEditorTextChanged(object sender, EventArgs eventArgs)
        {
            var document = sender as TextDocument;
            Text = document?.Text;
        }
    }
}
