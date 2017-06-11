using Prism.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Commands
{
    public static class ApplicationCommands
    {
        public static CompositeCommand ShowFlyoutCommand = new CompositeCommand();
        public static CompositeCommand ImportCommand = new CompositeCommand();
        public static CompositeCommand OpenEditorCommand = new CompositeCommand();
        public static CompositeCommand CloseEditorCommand = new CompositeCommand();
        public static CompositeCommand CopyClipboardCommand = new CompositeCommand();
        public static CompositeCommand ViewTableCommand = new CompositeCommand();
    }

    public interface IApplicationCommands
    {
        CompositeCommand ShowFlyoutCommand { get; }

        CompositeCommand ImportCommand { get; }

        CompositeCommand OpenEditorCommand { get; }

        CompositeCommand CloseEditorCommand { get; }

        CompositeCommand CopyClipboardCommand { get; }

        CompositeCommand ViewTableCommand { get; }
    }

    public class ApplicationCommandsProxy : IApplicationCommands
    {
        public CompositeCommand ShowFlyoutCommand => ApplicationCommands.ShowFlyoutCommand;

        public CompositeCommand ImportCommand => ApplicationCommands.ImportCommand;

        public CompositeCommand OpenEditorCommand => ApplicationCommands.OpenEditorCommand;

        public CompositeCommand CloseEditorCommand => ApplicationCommands.CloseEditorCommand;

        public CompositeCommand CopyClipboardCommand => ApplicationCommands.CopyClipboardCommand;

        public CompositeCommand ViewTableCommand => ApplicationCommands.ViewTableCommand;
    }
}
